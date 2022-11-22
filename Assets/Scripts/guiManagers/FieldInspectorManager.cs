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
    [FormerlySerializedAs("thisCanvas")] [SerializeField]
    private Canvas canvas;

    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI ownerLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private Button backButton;

    // ATTACK
    [SerializeField] private GameObject attackButton;
    [SerializeField] private AttackModeManager attackModeManager;
    private ButtonWrapper _attackModeButton;

    // UNIT
    [SerializeField] private GameObject buyUnitButton;
    [SerializeField] private TextMeshProUGUI unitsCountLabel;
    [SerializeField] private ImageColorManager unitColorManager;
    private ButtonWrapper _buyUnitButton;

    // TRENCHES
    [SerializeField] private Image trenchesImage;
    [SerializeField] private TextMeshProUGUI trenchesCountLabel;
    [SerializeField] private GameObject buyTrenchesButtonGameObject;
    private ButtonWrapper _buyTrenchesButton;

    // LABS
    [SerializeField] private Image labImage;
    [SerializeField] private TextMeshProUGUI labCountLabel;
    [SerializeField] private GameObject buyLabButtonGameObject;
    private ButtonWrapper _buyLabButton;

    public static bool RegroupMode;

    private FieldsParameters _parameters;
    private string _fieldName;


    public void EnableFieldInspector(string fieldName)
    {
        _parameters = FieldsParameters.LookupTable[fieldName];
        _fieldName = fieldName;
        RegroupMode = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        _buyUnitButton.Label.text = $"BUY ({GameplayConstants.UnitBaseCost})";
        _buyTrenchesButton.Label.text = $"BUY ({GameplayConstants.TrenchesBaseCost})";
        _buyLabButton.Label.text = $"BUY ({GameplayConstants.LabBaseCost})";

        unitColorManager.EnableAppropriateImage(
            Players.PlayersList.FindIndex(player => player.Name == _parameters.Owner));
        incomeLabel.text = Translator.TranslateIncome(_parameters.Income);
        nameLabel.text = Translator.TranslateField(fieldName);
        ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
        if (_parameters.Owner == null)
        {
            unitsCountLabel.text = "";
        }
        else if (_parameters.Owner == SceneGame.GetCurrentPlayer().Name)
        {
            unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
        }
        else
        {
            unitsCountLabel.text =
                _parameters.UnitsCountDescription() == "0" ? "x 0" : _parameters.UnitsCountDescription();
        }

        buyUnitButton.SetActive(_parameters.Owner == SceneGame.GetCurrentPlayer().Name);
        _buyUnitButton.Button.interactable = _parameters.HasTrenches &&
                                             SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.UnitBaseCost;

        var displayBuildings = _parameters.Owner != null;
        var displayButtons = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;

        trenchesImage.enabled = displayBuildings;
        trenchesCountLabel.enabled = displayBuildings;
        trenchesCountLabel.text = _parameters.HasTrenches ? "x 1/1" : "x 0/1";
        buyTrenchesButtonGameObject.SetActive(displayButtons);
        TrenchesButtonSetInteractable();

        var maxLabs = GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel];
        labImage.enabled = displayBuildings;
        labCountLabel.enabled = displayBuildings;
        labCountLabel.text = $"x {_parameters.Labs}/{maxLabs}";
        buyLabButtonGameObject.SetActive(displayButtons);
        _buyLabButton.Button.interactable = _parameters.Labs < maxLabs &&
                                            SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.LabBaseCost;

        EnableAttackButtonIfAbleToAttack();
        EnableMoveButtonIfAbleToMove();

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
        _buyUnitButton.Button.interactable = SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.UnitBaseCost;
        TrenchesButtonSetInteractable();
        _buyLabButton.Button.interactable =
            _parameters.Labs < GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel] &&
            SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.LabBaseCost;
        _parameters.Instance.unitsManager.EnableAppropriateSprites(_parameters.AllUnits, SceneGame.CurrentPlayerIndex);
        unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
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
        _parameters.HasTrenches = true;
        SceneGame.GetCurrentPlayer().Gold -= GameplayConstants.TrenchesBaseCost;
        _buyTrenchesButton.Button.interactable = false;
        _parameters.Instance.objectsManager.EnableAppropriateObjects(_parameters.Instance.name);
        _buyUnitButton.Button.interactable = SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.UnitBaseCost;
        _buyLabButton.Button.interactable =
            _parameters.Labs < GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel] &&
            SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.LabBaseCost;
        trenchesCountLabel.text = "x 1/1";
        SendEventObjectBought(ObjectBought.ObjectType.Trenches);
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(SceneGame.GetCurrentPlayer().Name)} has build trenches in {Translator.TranslateField(_parameters.Instance.name)}");
        AudioPlayer.PlayBuy();
    }

    public void OnClickBuyLabButton()
    {
        _parameters.Labs++;
        SceneGame.GetCurrentPlayer().Gold -= GameplayConstants.LabBaseCost;
        TrenchesButtonSetInteractable();
        _parameters.Instance.objectsManager.EnableAppropriateObjects(_parameters.Instance.name);
        _buyUnitButton.Button.interactable = SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.UnitBaseCost;
        _buyLabButton.Button.interactable =
            _parameters.Labs < GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel] &&
            SceneGame.GetCurrentPlayer().Gold >= GameplayConstants.LabBaseCost;
        var maxLabs = GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel];
        labCountLabel.text = $"x {_parameters.Labs}/{maxLabs}";
        AudioPlayer.PlayBuy();
        SendEventObjectBought(ObjectBought.ObjectType.Lab);
        NotificationsBarManager.SendNotification(
            $"{Players.DescribeNameAsColor(SceneGame.GetCurrentPlayer().Name)} has build a lab in {Translator.TranslateField(_parameters.Instance.name)}");
    }

    private void SendEventObjectBought(ObjectBought.ObjectType objectType)
    {
        ObjectBought eventData = new(_parameters.Instance.name, objectType);
        PhotonNetwork.RaiseEvent(
            eventData.GetEventType(),
            eventData.Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable);
    }

    private void Start()
    {
        _buyUnitButton = new ButtonWrapper(buyUnitButton);
        _attackModeButton = new ButtonWrapper(attackButton);
        _buyTrenchesButton = new ButtonWrapper(buyTrenchesButtonGameObject);
        _buyLabButton = new ButtonWrapper(buyLabButtonGameObject);
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
            FieldsParameters.LookupTable.Values.Count(field => field.HasTrenches && field.Owner == player.Name);
        _buyTrenchesButton.Button.interactable = !_parameters.HasTrenches &&
                                                 player.Gold >= GameplayConstants.TrenchesBaseCost && trenchesCount <
                                                 GameplayConstants.TrenchesLimits[player.TrenchesLimitLevel];
    }
}