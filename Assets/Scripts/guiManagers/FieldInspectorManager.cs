using System;
using System.Linq;
using ExitGames.Client.Photon;
using fields;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FieldInspectorManager : MonoBehaviourPunCallbacks
{
    public static bool RegroupMode;

    [SerializeField] private Canvas canvas;

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
    [SerializeField] private GameObject sellUnitMockButton;

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
    private Vector3 _iconStartingPosition;
    private Vector3 _labelStartingPosition;
    private Vector3 _buyButtonStartingPosition;
    private Vector3 _sellButtonStartingPosition;
    private float _iconDifference;
    private float _labelDifference;
    private float _buttonDifference;

    private void Start()
    {
        _buyUnitButton = new ButtonWrapper(buyUnitButton);
        _attackModeButton = new ButtonWrapper(attackButton);
        _buyTrenchesButton = new ButtonWrapper(buyTrenchesButtonGameObject);
        _buyLabButton = new ButtonWrapper(buyLabButtonGameObject);
        _sellTrenchesButton = new ButtonWrapper(sellTrenchesButtonGameObject);
        _sellLabButton = new ButtonWrapper(sellLabButtonGameObject);
        _iconStartingPosition = unitColorManager.blueImage.transform.position;
        _labelStartingPosition = unitsCountLabel.transform.position;
        _buyButtonStartingPosition = buyUnitButton.transform.position;
        _sellButtonStartingPosition = sellUnitMockButton.transform.position;
        _iconDifference = Math.Abs(_iconStartingPosition.y - trenchesImage.transform.position.y);
        _labelDifference = Math.Abs(_iconStartingPosition.y - trenchesCountLabel.transform.position.y);
        _buttonDifference = Math.Abs(_buyButtonStartingPosition.y - buyTrenchesButtonGameObject.transform.position.y);
        sellUnitMockButton.SetActive(false);
    }


    public void EnableFieldInspector(string fieldName)
    {
        _parameters = FieldsParameters.LookupTable[fieldName];
        _fieldName = fieldName;
        RegroupMode = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        Refresh();
        CorrectPlacement();
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
        CorrectPlacement();
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

    private void Refresh()
    {
        // VARIABLES
        var maxLabs = GameplayConstants.ScienceLabLimits[SceneGame.GetCurrentPlayer().LabsLimitLevel];
        var currentPlayer = SceneGame.GetCurrentPlayer();

        if (_parameters.Owner == currentPlayer.Name)
        {
            // FIELD BELONGS TO CURRENT PLAYER
            // LABELS
            trenchesCountLabel.enabled = true;
            trenchesCountLabel.text = _parameters.HasTrenches || _parameters.IsCapital ? "x 1/1" : "x 0/1";
            labCountLabel.enabled = true;
            labCountLabel.text = $"x {_parameters.Labs}/{maxLabs}";
            incomeLabel.text = Translator.TranslateIncome(_parameters.Income);
            nameLabel.text = Translator.TranslateField(_parameters.Instance.name);
            ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
            unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;

            // IMAGES
            trenchesImage.enabled = true;
            labImage.enabled = true;
            unitColorManager.enabled = true;
            unitColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);

            // BUTTONS ACTIVE/INTERACTABLE
            sellTrenchesButtonGameObject.SetActive(!_parameters.IsCapital && _parameters.HasTrenches);
            sellLabButtonGameObject.SetActive(_parameters.Labs > 0);
            buyUnitButton.SetActive(true);
            buyTrenchesButtonGameObject.SetActive(!_parameters.IsCapital);
            buyLabButtonGameObject.SetActive(true);

            EnableAttackButtonIfAbleToAttack();
            EnableMoveButtonIfAbleToMove();

            _buyLabButton.Button.interactable = _parameters.Labs < maxLabs &&
                                                currentPlayer.Gold >= GameplayConstants.LabBaseCost +
                                                GameplayConstants.ScienceLabCostIncrement * _parameters.Labs;
            var boughtTrenches = FieldsParameters.LookupTable.Values.Count(parameters =>
                parameters.Owner == currentPlayer.Name && !parameters.IsCapital && parameters.HasTrenches);
            var trenchesLimitForThisPlayer = GameplayConstants.TrenchesLimits[currentPlayer.TrenchesLimitLevel];
            _buyTrenchesButton.Button.interactable = boughtTrenches < trenchesLimitForThisPlayer &&
                                                     currentPlayer.Gold >= GameplayConstants.TrenchesBaseCost;
            _buyUnitButton.Button.interactable = _parameters.HasTrenches &&
                                                 currentPlayer.Gold >= GameplayConstants.UnitBaseCost;
            _sellTrenchesButton.Button.interactable = _parameters.HasTrenches;
            _sellLabButton.Button.interactable = _parameters.Labs > 0;
            _sellTrenchesButton.Button.interactable = _parameters.HasTrenches;

            // BUTTONS TEXT
            _buyUnitButton.Label.text = $"BUY ({GameplayConstants.UnitBaseCost})";
            _buyTrenchesButton.Label.text = $"BUY ({GameplayConstants.TrenchesBaseCost})";
            _buyLabButton.Label.text =
                $"BUY ({GameplayConstants.LabBaseCost + GameplayConstants.ScienceLabCostIncrement * _parameters.Labs})";
            _sellTrenchesButton.Label.text = $"SELL ({GameplayConstants.TrenchesBaseCost / 2})";
            _sellLabButton.Label.text = _parameters.Labs > 0
                ? $"SELL ({(GameplayConstants.LabBaseCost + GameplayConstants.ScienceLabCostIncrement * (_parameters.Labs - 1)) / 2})"
                : "";
        }
        else if (_parameters.Owner == null)
        {
            // FIELD BELONGS TO NONE
            // LABELS
            trenchesCountLabel.enabled = _parameters.HasTrenches;
            trenchesCountLabel.text = "x 1/1";
            labCountLabel.enabled = _parameters.Labs > 0;
            labCountLabel.text = $"x {_parameters.Labs}";
            incomeLabel.text = Translator.TranslateIncome(_parameters.Income);
            nameLabel.text = Translator.TranslateField(_parameters.Instance.name);
            ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
            unitsCountLabel.enabled = false;

            // IMAGES
            trenchesImage.enabled = _parameters.HasTrenches;
            labImage.enabled = _parameters.Labs > 0;
            unitColorManager.gameObject.SetActive(_parameters.AllUnits > 0);
            unitColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);

            // BUTTONS ACTIVE/INTERACTABLE
            sellTrenchesButtonGameObject.SetActive(false);
            sellLabButtonGameObject.SetActive(false);
            EnableAttackButtonIfAbleToAttack();
            EnableMoveButtonIfAbleToMove();
            buyTrenchesButtonGameObject.SetActive(false);
            buyLabButtonGameObject.SetActive(false);
            buyUnitButton.SetActive(false);
        }
        else
        {
            // FIELD BELONGS TO SOMEONE ELSE
            // LABELS
            trenchesCountLabel.enabled = _parameters.HasTrenches;
            trenchesCountLabel.text = "x 1/1";
            labCountLabel.enabled = _parameters.Labs > 0;
            labCountLabel.text = FieldsParameters.BuildingCountDescription(_parameters.Labs);
            incomeLabel.text = Translator.TranslateIncome();
            nameLabel.text = Translator.TranslateField(_parameters.Instance.name);
            ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
            unitsCountLabel.enabled = _parameters.AllUnits > 0;
            unitsCountLabel.text = _parameters.UnitsCountDescription();

            // IMAGES
            trenchesImage.enabled = _parameters.HasTrenches;
            labImage.enabled = _parameters.Labs > 0;
            unitColorManager.gameObject.SetActive(_parameters.AllUnits > 0);
            unitColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);

            // BUTTONS ACTIVE/INTERACTABLE
            sellTrenchesButtonGameObject.SetActive(false);
            sellLabButtonGameObject.SetActive(false);
            EnableAttackButtonIfAbleToAttack();
            EnableMoveButtonIfAbleToMove();
            buyTrenchesButtonGameObject.SetActive(false);
            buyLabButtonGameObject.SetActive(false);
            buyUnitButton.SetActive(false);
        }

        unitColorManager.EnableAppropriateImage(
            Players.PlayersList.FindIndex(player => player.Name == _parameters.Owner));
        TopStatsManager.Instance.RefreshValues();
    }

    private void CorrectPlacement()
    {
        var currentPlayer = SceneGame.GetCurrentPlayer();
        var takenSpacesCounter = 0;

        if (_parameters.AllUnits > 0 || _parameters.Owner == currentPlayer.Name)
        {
            unitColorManager.transform.position = _iconStartingPosition;
            unitsCountLabel.transform.position = _labelStartingPosition;
            buyUnitButton.transform.position = _buyButtonStartingPosition;
            takenSpacesCounter++;
        }

        if (_parameters.HasTrenches || _parameters.Owner == currentPlayer.Name)
        {
            trenchesImage.transform.position = _iconStartingPosition;
            trenchesImage.transform.Translate(0, -_iconDifference * takenSpacesCounter, 0);

            trenchesCountLabel.transform.position = _labelStartingPosition;
            trenchesCountLabel.transform.Translate(0, -_labelDifference * takenSpacesCounter, 0);

            buyTrenchesButtonGameObject.transform.position = _buyButtonStartingPosition;
            buyTrenchesButtonGameObject.transform.Translate(0, -_buttonDifference * takenSpacesCounter, 0);

            sellTrenchesButtonGameObject.transform.position = _sellButtonStartingPosition;
            sellTrenchesButtonGameObject.transform.Translate(0, -_buttonDifference * takenSpacesCounter, 0);
            
            takenSpacesCounter++;
        }

        if (_parameters.Labs > 0 || _parameters.Owner == currentPlayer.Name)
        {
            labImage.transform.position = _iconStartingPosition;
            labImage.transform.Translate(0, -_iconDifference * takenSpacesCounter, 0);

            labCountLabel.transform.position = _labelStartingPosition;
            labCountLabel.transform.Translate(0, -_labelDifference * takenSpacesCounter, 0);
            
            buyLabButtonGameObject.transform.position = _buyButtonStartingPosition;
            buyLabButtonGameObject.transform.Translate(0, -_buttonDifference * takenSpacesCounter, 0);

            sellLabButtonGameObject.transform.position = _sellButtonStartingPosition;
            sellLabButtonGameObject.transform.Translate(0, -_buttonDifference * takenSpacesCounter, 0);
            
            takenSpacesCounter++;
        }
    }
}