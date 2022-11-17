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
    [SerializeField] private ImageColorManager unitColorManager;
    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI ownerLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI unitsCountLabel;
    [FormerlySerializedAs("thisCanvas")] [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buyUnitButton;
    [SerializeField] private GameObject attackButton;
    [SerializeField] private AttackModeManager attackModeManager;
    [SerializeField] private Button backButton;
    [SerializeField] private Image trenchesImage;
    [SerializeField] private TextMeshProUGUI trenchesCountLabel;
    [SerializeField] private GameObject buyTrenchesButtonGameObject;

    public static bool RegroupMode;
    
    private TextMeshProUGUI _buyUnitButtonLabel;
    private TextMeshProUGUI _attackButtonLabel;
    private Button _buyUnitButton;
    private Button _attackButton;
    private FieldsParameters _parameters;
    private string _fieldName;
    private Button _buyTrenchesButton;

    public void EnableFieldInspector(string fieldName)
    {
        _parameters = FieldsParameters.LookupTable[fieldName];
        _fieldName = fieldName;
        RegroupMode = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
 
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
            unitsCountLabel.text ="x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
        }
        else
        {
            unitsCountLabel.text = _parameters.UnitsCountDescription() == "0" ? "x 0" : _parameters.UnitsCountDescription();
        }
        buyUnitButton.SetActive(_parameters.Owner == SceneGame.GetCurrentPlayer().Name);
        _buyUnitButton.interactable = _parameters.HasTrenches && SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;

        trenchesImage.enabled = _parameters.Owner != null;
        trenchesCountLabel.enabled = trenchesImage.enabled;
        trenchesCountLabel.text = _parameters.HasTrenches ? "x 1/1" : "x 0/1";
        buyTrenchesButtonGameObject.SetActive(_parameters.Owner == SceneGame.GetCurrentPlayer().Name);
        _buyTrenchesButton.interactable = !_parameters.HasTrenches &&
                                          SceneGame.GetCurrentPlayer().Gold >=
                                          SceneGame.TrenchesBaseCost;
        
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
        
        BuyUnits data = new(_parameters.Instance.name, _parameters.AvailableUnits, _parameters.AllUnits, _parameters.Owner, SceneGame.GetCurrentPlayer().Gold);
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        SharedVariables.IsOverUi = false;
        backButton.interactable = false;
    }

    public void OnClickBuyUnitButton()
    {
        _parameters.AllUnits++;
        SceneGame.GetCurrentPlayer().Gold -= SceneGame.UnitBaseCost;
        _buyUnitButton.interactable = SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;
        _parameters.Instance.unitsManager.EnableAppropriateSprites(_parameters.AllUnits, SceneGame.CurrentPlayerIndex);
        unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
        AudioPlayer.PlayBuy();
    }

    public void OnClickAttackButton()
    {
        BuyUnits data = new(name, _parameters.AvailableUnits, _parameters.AllUnits, _parameters.Owner, SceneGame.GetCurrentPlayer().Gold);
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        attackModeManager.EnableAttackModule(_fieldName);
        HideFieldInspector();
        AudioPlayer.PlayButtonClick();
    }

    public void OnClickBuyTrenchesButton()
    {
        _parameters.HasTrenches = true;
        SceneGame.GetCurrentPlayer().Gold -= SceneGame.TrenchesBaseCost;
        _buyTrenchesButton.interactable = false;
        _parameters.Instance.objectsManager.EnableAppropriateObjects(_parameters.Instance.name);
        _buyUnitButton.interactable = SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;
        trenchesCountLabel.text = "x 1/1";
        TrenchesBought eventData = new(_parameters.Instance.name);
        PhotonNetwork.RaiseEvent(
            eventData.GetEventType(),
            eventData.Serialize(),
            new RaiseEventOptions { Receivers = ReceiverGroup.Others },
            SendOptions.SendReliable);
        NotificationsBarManager.SendNotification($"{Players.DescribeNameAsColor(SceneGame.GetCurrentPlayer().Name)} has build trenches in {Translator.TranslateField(_parameters.Instance.name)}");
        AudioPlayer.PlayBuy();
    }

    private void Start()
    {
        _buyUnitButtonLabel = buyUnitButton.GetComponentInChildren<TextMeshProUGUI>();
        _attackButtonLabel = attackButton.GetComponentInChildren<TextMeshProUGUI>();
        _buyUnitButton = buyUnitButton.GetComponent<Button>();
        _attackButton = attackButton.GetComponent<Button>();
        _buyTrenchesButton = buyTrenchesButtonGameObject.GetComponent<Button>();
    }

    private void EnableAttackButtonIfAbleToAttack()
    {
        var enable = _parameters.Owner != SceneGame.GetCurrentPlayer().Name 
                     && FieldsParameters.Neighbours[_fieldName].Any(neighbourFieldName => 
                         FieldsParameters.LookupTable[neighbourFieldName].Owner == SceneGame.GetCurrentPlayer().Name);
        _attackButton.image.enabled = enable;
        _attackButtonLabel.enabled = enable;
    }

    private void EnableMoveButtonIfAbleToMove()
    {
        if (RegroupMode && FieldsParameters.Neighbours[_fieldName].Any(neighbourFieldName => 
                FieldsParameters.LookupTable[neighbourFieldName].Owner == SceneGame.GetCurrentPlayer().Name))
        {
            _attackButton.image.enabled = true;
            _attackButtonLabel.enabled = true;
            _attackButtonLabel.text = "Move";
        }
        else
        {
            _attackButtonLabel.text = "Attack";
        }
    }
}
