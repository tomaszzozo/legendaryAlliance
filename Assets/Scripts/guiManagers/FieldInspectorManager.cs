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

    public static bool RegroupMode;
    
    private TextMeshProUGUI _buyUnitButtonLabel;
    private TextMeshProUGUI _attackButtonLabel;
    private Button _buyUnitButton;
    private Button _attackButton;
    private FieldsParameters _parameters;
    private string _fieldName;
    
    
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
        unitsCountLabel.text = _parameters.Owner != null ? 
            "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits
            : "";
        _buyUnitButton.image.enabled = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        _buyUnitButton.interactable = _buyUnitButton.image.enabled && SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;
        _buyUnitButtonLabel.enabled = _buyUnitButton.image.enabled;
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
        SceneGame.GlobalVariables.SelectedFieldLocal.unitsManager.EnableAppropriateSprites(_parameters.AllUnits, SceneGame.CurrentPlayerIndex);
        unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
        AudioPlayer.PlayBuyUnit();
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

    private void Start()
    {
        _buyUnitButtonLabel = buyUnitButton.GetComponentInChildren<TextMeshProUGUI>();
        _attackButtonLabel = attackButton.GetComponentInChildren<TextMeshProUGUI>();
        _buyUnitButton = buyUnitButton.GetComponent<Button>();
        _attackButton = attackButton.GetComponent<Button>();
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
