using System.Linq;
using ExitGames.Client.Photon;
using fields;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FieldInspectorManager : MonoBehaviourPunCallbacks
{
    public static bool RegroupMode;

    // DEPENDENCIES
    [SerializeField] private TopStatsManager topStatsManager;

    // ITSELF
    [FormerlySerializedAs("thisCanvas")] [SerializeField]
    private Canvas canvas;

    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI ownerLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Button backButton;

    // ATTACK
    [SerializeField] private GameObject attackButton;
    [SerializeField] private AttackModeManager attackModeManager;

    // UNIT
    [SerializeField] private GameObject buyUnitButton;
    [SerializeField] private TextMeshProUGUI unitsCountLabel;
    [SerializeField] private ImageColorManager unitColorManager;

    // TRENCHES
    [SerializeField] private Image trenchesImage;
    [SerializeField] private TextMeshProUGUI trenchesCountLabel;
    [SerializeField] private GameObject buyTrenchesButtonGameObject;
    [SerializeField] private GameObject sellTrenchesButtonGameObject;

    // LABS
    [SerializeField] private Image labImage;
    [SerializeField] private TextMeshProUGUI labCountLabel;
    [SerializeField] private GameObject buyLabButtonGameObject;
    [SerializeField] private GameObject sellLabButtonGameObject;

    private ButtonWrapper _attackModeButton;
    private ButtonWrapper _buyLabButton;
    private ButtonWrapper _buyTrenchesButton;
    private ButtonWrapper _buyUnitButton;
    private ButtonWrapper _sellTrenchesButton;
    private ButtonWrapper _sellLabButton;
    private string _fieldName;

    private FieldsParameters _parameters;

    private void Start()
    {
        _buyUnitButton = new ButtonWrapper(buyUnitButton);
        _attackModeButton = new ButtonWrapper(attackButton);
        _buyTrenchesButton = new ButtonWrapper(buyTrenchesButtonGameObject);
        _buyLabButton = new ButtonWrapper(buyLabButtonGameObject);
        _sellTrenchesButton = new ButtonWrapper(sellTrenchesButtonGameObject);
        _sellLabButton = new ButtonWrapper(sellLabButtonGameObject);
    }


    public void EnableFieldInspector(string fieldName)
    {
        _parameters = FieldsParameters.LookupTable[fieldName];
        _fieldName = fieldName;
        RegroupMode = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        Refresh();
        canvas.enabled = true;
        SharedVariables.IsOverUi = true;
        backButton.interactable = true;
    }

    public void HideFieldInspector(bool onStartup = false)
    {
        canvas.enabled = false;
        if (onStartup || _parameters.Owner != SceneGame.GetCurrentPlayer().Name) return;

        BuyUnits data = new(_parameters.Instance.name, _parameters.AvailableUnits, _parameters.AllUnits,
            _parameters.Owner, SceneGame.GetCurrentPlayer().Gold);
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        SharedVariables.IsOverUi = false;
        backButton.interactable = false;
    }

    public void OnClickBuyUnitButton()
    {
        _parameters.AllUnits++;
        SceneGame.GetCurrentPlayer().Gold -= GameplayConstants.UnitBaseCost;
        Refresh();
        _parameters.Instance.unitsManager.EnableAppropriateSprites(_parameters.AllUnits, SceneGame.CurrentPlayerIndex);
        AudioPlayer.PlayBuy();
    }

    public void OnClickAttackButton()
    {
        BuyUnits data = new(name, _parameters.AvailableUnits, _parameters.AllUnits, _parameters.Owner,
            SceneGame.GetCurrentPlayer().Gold);
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        attackModeManager.EnableAttackModule(_fieldName);
        HideFieldInspector();
        AudioPlayer.PlayButtonClick();
    }

    public void OnClickBuyTrenchesButton()
    {
        var player = SceneGame.GetCurrentPlayer();
        _parameters.HasTrenches = true;
        player.Income = player.CalculateIncome();
        player.Gold -= GameplayConstants.TrenchesBaseCost;
        _parameters.Instance.objectsManager.EnableAppropriateObjects();
        Refresh();
        SendEventObjectChanged(ObjectChanged.ObjectType.Trenches);
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(player.Name)} has build trenches in {Translator.TranslateField(_parameters.Instance.name)}");
        AudioPlayer.PlayBuy();
    }

    public void OnClickSellTrenchesButton()
    {
        var player = SceneGame.GetCurrentPlayer();
        _parameters.HasTrenches = false;
        player.Gold += GameplayConstants.TrenchesBaseCost / 2;
        player.Income = player.CalculateIncome();
        _parameters.Instance.objectsManager.EnableAppropriateObjects();
        Refresh();
        SendEventObjectChanged(ObjectChanged.ObjectType.Trenches, true);
        AudioPlayer.PlayBuy();
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(player.Name)} has sold trenches in {Translator.TranslateField(_parameters.Instance.name)}");
    }

    public void OnClickSellLabButton()
    {
        var player = SceneGame.GetCurrentPlayer();
        player.Gold += _parameters.CalculateCostOfSellingLab();
        _parameters.Labs--;
        player.Income = player.CalculateIncome();
        _parameters.Instance.objectsManager.EnableAppropriateObjects();
        Refresh();
        SendEventObjectChanged(ObjectChanged.ObjectType.Lab, true);
        AudioPlayer.PlayBuy();
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(player.Name)} has sold a lab in {Translator.TranslateField(_parameters.Instance.name)}");
    }

    public void OnClickBuyLabButton()
    {
        var player = SceneGame.GetCurrentPlayer();
        player.Gold -= _parameters.CalculateCostOfBuyingLab();
        _parameters.Labs++;
        _parameters.Instance.objectsManager.EnableAppropriateObjects();
        player.Income = player.CalculateIncome();
        Refresh();
        AudioPlayer.PlayBuy();
        SendEventObjectChanged(ObjectChanged.ObjectType.Lab);
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(player.Name)} has build a lab in {Translator.TranslateField(_parameters.Instance.name)}");
    }

    private void SendEventObjectChanged(ObjectChanged.ObjectType objectType, bool sold = false)
    {
        ObjectChanged eventData = new(_parameters.Instance.name, objectType, sold);
        PhotonNetwork.RaiseEvent(
            eventData.GetEventType(),
            eventData.Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable);
    }

    private void EnableAttackButtonIfAbleToAttack()
    {
        var enable = _parameters.Owner != SceneGame.GetCurrentPlayer().Name
                     && FieldsParameters.Neighbours[_fieldName].Any(neighbourFieldName =>
                         FieldsParameters.LookupTable[neighbourFieldName].Owner == SceneGame.GetCurrentPlayer().Name);
        _attackModeButton.Button.image.enabled = enable;
        _attackModeButton.Label.enabled = enable;
    }

    private void EnableMoveButtonIfAbleToMove()
    {
        if (RegroupMode && FieldsParameters.Neighbours[_fieldName].Any(neighbourFieldName =>
                FieldsParameters.LookupTable[neighbourFieldName].Owner == SceneGame.GetCurrentPlayer().Name))
        {
            _attackModeButton.Button.image.enabled = true;
            _attackModeButton.Label.enabled = true;
            _attackModeButton.Label.text = "Move";
        }
        else
        {
            _attackModeButton.Label.text = "Attack";
        }
    }

    private void TrenchesButtonSetInteractable()
    {
        var player = SceneGame.GetCurrentPlayer();
        var trenchesCount =
            FieldsParameters.LookupTable.Values.Count(field =>
                field.HasTrenches && field.Owner == player.Name && !field.IsCapital);
        _buyTrenchesButton.Button.interactable = !_parameters.HasTrenches &&
                                                 player.Gold >= GameplayConstants.TrenchesBaseCost && trenchesCount <
                                                 GameplayConstants.TrenchesLimits[player.TrenchesLimitLevel];
    }

    private void Refresh()
    {
        // VARIABLES
        var displayBuildings = _parameters.Owner != null;
        var maxLabs = GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel];

        // BUTTONS TEXT
        _buyUnitButton.Label.text = $"BUY ({GameplayConstants.UnitBaseCost})";
        _buyTrenchesButton.Label.text = $"BUY ({GameplayConstants.TrenchesBaseCost})";
        _buyLabButton.Label.text =
            $"BUY ({GameplayConstants.LabBaseCost + GameplayConstants.ScienceLabCostIncrement * _parameters.Labs})";
        _sellTrenchesButton.Label.text = $"SELL ({GameplayConstants.TrenchesBaseCost / 2})";
        _sellLabButton.Label.text = _parameters.Labs > 0
            ? $"SELL ({(GameplayConstants.LabBaseCost + GameplayConstants.ScienceLabCostIncrement * (_parameters.Labs - 1)) / 2})"
            : "";

        // BUTTONS ACTIVE/INTERACTABLE
        sellTrenchesButtonGameObject.SetActive(_parameters.HasTrenches && _parameters.Owner == SceneGame.GetCurrentPlayer().Name && !_parameters.IsCapital);
        sellLabButtonGameObject.SetActive(_parameters.Labs > 0 && _parameters.Owner == SceneGame.GetCurrentPlayer().Name);
        TrenchesButtonSetInteractable();
        EnableAttackButtonIfAbleToAttack();
        EnableMoveButtonIfAbleToMove();
        var displayButtons = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        buyTrenchesButtonGameObject.SetActive(displayButtons && !_parameters.IsCapital);
        buyLabButtonGameObject.SetActive(displayButtons);
        _buyLabButton.Button.interactable = _parameters.Labs < maxLabs &&
                                            SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.LabBaseCost +
                                            GameplayConstants.ScienceLabCostIncrement * _parameters.Labs;
        buyUnitButton.SetActive(_parameters.Owner == SceneGame.GetCurrentPlayer().Name);
        _buyUnitButton.Button.interactable = _parameters.HasTrenches &&
                                             SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.UnitBaseCost;

        // LABELS
        trenchesCountLabel.enabled = displayBuildings;
        trenchesCountLabel.text = _parameters.HasTrenches ? "x 1/1" : "x 0/1";
        labCountLabel.enabled = displayBuildings;
        labCountLabel.text = _parameters.Owner == SceneGame.GetCurrentPlayer().Name
            ? $"x {_parameters.Labs}/{maxLabs}"
            : FieldsParameters.BuildingCountDescription(_parameters.Labs);
        incomeLabel.text = Translator.TranslateIncome(_parameters.Income);
        nameLabel.text = Translator.TranslateField(_parameters.Instance.name);
        ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
        if (_parameters.Owner == null)
            unitsCountLabel.text = "";
        else if (_parameters.Owner == SceneGame.GetCurrentPlayer().Name)
            unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
        else
            unitsCountLabel.text =
                _parameters.UnitsCountDescription() == "0" ? "x 0" : _parameters.UnitsCountDescription();

        // IMAGES
        trenchesImage.enabled = displayBuildings;
        labImage.enabled = displayBuildings;
        unitColorManager.EnableAppropriateImage(
            Players.PlayersList.FindIndex(player => player.Name == _parameters.Owner));

        topStatsManager.RefreshValues();
    }
}