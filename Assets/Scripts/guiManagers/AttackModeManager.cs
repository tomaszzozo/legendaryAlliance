using ExitGames.Client.Photon;
using fields;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackModeManager : MonoBehaviour
{
    [FormerlySerializedAs("attackModeManager")] 
    [FormerlySerializedAs("manager")] 
    [SerializeField] private AttackModeNeighbourManager attackModeNeighbourManager;
    [SerializeField] private Canvas canvas;
    [SerializeField] private FieldInspectorManager fieldInspectorManager;
    [SerializeField] private TextMeshProUGUI theirUnitsLabel;
    [SerializeField] private TextMeshProUGUI ourUnitsLabel;
    [SerializeField] private ImageColorManager ourColorManager;
    [SerializeField] private ImageColorManager theirColorManager;
    [SerializeField] private TextMeshProUGUI vsLabel;
    [SerializeField] private TopStatsManager topStatsManager;
    [SerializeField] private TextMeshProUGUI fieldNameLabel;
    [SerializeField] private TextMeshProUGUI attackButtonLabel;

    public static int AllChosenUnits;
    private static int _allChosenUnitsHistory;

    private string _fieldName;
    
    public void OnClickCancelButton()
    {
        SharedVariables.IsOverUi = false;
        fieldInspectorManager.EnableFieldInspector(_fieldName);
        canvas.enabled = false;
    }

    public void OnClickAttackButton()
    {
        if (AllChosenUnits == 0) return;
        var parameters = FieldsParameters.LookupTable[_fieldName];
        if (parameters.Owner == null)
        {
            parameters.Owner = SceneGame.GetCurrentPlayer().Name;
            parameters.Instance.EnableAppropriateBorderSprite();
            parameters.AllUnits = AllChosenUnits;
            parameters.AvailableUnits = 0;
            
            var updatedData = attackModeNeighbourManager.UpdateNeighbours();
            
            updatedData.Insert(0, new AfterAttackUpdateFields.FieldUpdatedData
            {
                AllUnits = parameters.AllUnits,
                AvailableUnits = parameters.AvailableUnits,
                FieldName = parameters.Instance.name,
                NewOwner = parameters.Owner
            });
            AfterAttackUpdateFields newEvent = new(updatedData);
            RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                SendOptions.SendReliable);
            NotificationsBarManager.SendNotification($"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} is now in control of {Translator.TranslateField(parameters.Instance.name)}");

            parameters.Instance.EnableAppropriateGlowSprite();
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits, SceneGame.CurrentPlayerIndex);
            
            SceneGame.GetCurrentPlayer().Income = SceneGame.GetCurrentPlayer().CalculateIncome();
            topStatsManager.RefreshValues();
            
            OnClickCancelButton();
        }
        else if (FieldInspectorManager.RegroupMode)
        {
            parameters.AllUnits += AllChosenUnits;
            var updatedData = attackModeNeighbourManager.UpdateNeighbours();
            updatedData.Insert(0, new AfterAttackUpdateFields.FieldUpdatedData
            {
                AllUnits = parameters.AllUnits,
                AvailableUnits = parameters.AvailableUnits,
                FieldName = parameters.Instance.name,
                NewOwner = parameters.Owner
            });
            AfterAttackUpdateFields newEvent = new(updatedData);
            RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                SendOptions.SendReliable);
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits, SceneGame.CurrentPlayerIndex);
            OnClickCancelButton();
        }
        else if (AllChosenUnits == parameters.AllUnits)
        {
            parameters.AllUnits = 0;
            NotificationsBarManager.SendNotification($"{Players.DescribeNameAsColor(parameters.Owner)} lost control of {Translator.TranslateField(parameters.Instance.name)} due to lack of units after {Players.DescribeNameAsColor(PhotonNetwork.NickName)} attack!");
            parameters.Owner = null;
            parameters.Instance.DisableAllBorderSprites();
            parameters.Instance.EnableAppropriateGlowSprite();
            
            var updatedData = attackModeNeighbourManager.UpdateNeighbours();
            updatedData.Insert(0, new AfterAttackUpdateFields.FieldUpdatedData
            {
                AllUnits = 0,
                AvailableUnits = 0,
                FieldName = parameters.Instance.name,
                NewOwner = null
            });
            AfterAttackUpdateFields newEvent = new(updatedData);
            RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                SendOptions.SendReliable);
            parameters.Instance.unitsManager.EnableAppropriateSprites(0, Players.NameToIndex(PhotonNetwork.NickName));
            OnClickCancelButton();
        }
        else if (AllChosenUnits < parameters.AllUnits)
        {
            parameters.AllUnits -= AllChosenUnits;
            if (parameters.AvailableUnits > parameters.AllUnits) parameters.AvailableUnits = parameters.AllUnits;

            var updatedData = attackModeNeighbourManager.UpdateNeighbours();
            updatedData.Insert(0, new AfterAttackUpdateFields.FieldUpdatedData
            {
                AllUnits = parameters.AllUnits,
                AvailableUnits = parameters.AvailableUnits,
                FieldName = parameters.Instance.name,
                NewOwner = parameters.Owner
            });
            
            AfterAttackUpdateFields newEvent = new(updatedData);
            RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                SendOptions.SendReliable);
            NotificationsBarManager.SendNotification($"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} attacked {Players.DescribeNameAsColor(parameters.Owner)} in {Translator.TranslateField(parameters.Instance.name)} but failed to gain control!");

            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits, Players.NameToIndex(parameters.Owner));
            OnClickCancelButton();
        }
        else
        {
            parameters.Owner = SceneGame.GetCurrentPlayer().Name;
            parameters.Instance.EnableAppropriateBorderSprite();
            parameters.AllUnits = AllChosenUnits - parameters.AllUnits;
            parameters.AvailableUnits = 0;
            
            var updatedData = attackModeNeighbourManager.UpdateNeighbours();
            updatedData.Insert(0, new AfterAttackUpdateFields.FieldUpdatedData
            {
                AllUnits = parameters.AllUnits,
                AvailableUnits = parameters.AvailableUnits,
                FieldName = parameters.Instance.name,
                NewOwner = parameters.Owner
            });
         
            AfterAttackUpdateFields newEvent = new(updatedData);
            RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                SendOptions.SendReliable);
            NotificationsBarManager.SendNotification($"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} attacked {Players.DescribeNameAsColor(parameters.Owner)} and now controls {Translator.TranslateField(parameters.Instance.name)}!");

            parameters.Instance.EnableAppropriateGlowSprite();
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits, SceneGame.CurrentPlayerIndex);
            
            SceneGame.GetCurrentPlayer().Income = SceneGame.GetCurrentPlayer().CalculateIncome();
            topStatsManager.RefreshValues();
            
            OnClickCancelButton();
        }
    }

    public void EnableAttackModule(string fieldName)
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        _fieldName = fieldName;
        fieldNameLabel.text = Translator.TranslateField(fieldName);
        canvas.enabled = true;
        theirUnitsLabel.text = FieldsParameters.LookupTable[fieldName].AllUnits.ToString();
        ourUnitsLabel.text = "0";
        attackModeNeighbourManager.EnableAppropriateComponents(fieldName);
        ourColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);
        vsLabel.enabled = FieldsParameters.LookupTable[fieldName].Owner != null;
        vsLabel.text = FieldInspectorManager.RegroupMode ? "=>" : "vs";
        theirUnitsLabel.enabled = vsLabel.enabled;
        if (FieldsParameters.LookupTable[fieldName].Owner == null) theirColorManager.DisableImages();
        else theirColorManager.EnableAppropriateImage(Players.PlayersList.FindIndex(player => player.Name == FieldsParameters.LookupTable[fieldName].Owner));
        SharedVariables.IsOverUi = true;
        attackButtonLabel.text = FieldInspectorManager.RegroupMode ? "move" : "to glory!";
    }

    private void Update()
    {
        if (AllChosenUnits == _allChosenUnitsHistory) return;
        _allChosenUnitsHistory = AllChosenUnits;
        ourUnitsLabel.text = AllChosenUnits.ToString();
    }

    private void Start()
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        canvas.enabled = false;
    }
}
