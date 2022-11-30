using System.Linq;
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
    public static int AllChosenUnits;
    private static int _allChosenUnitsHistory;

    [FormerlySerializedAs("attackModeManager")] [FormerlySerializedAs("manager")] [SerializeField]
    private AttackModeNeighbourManager attackModeNeighbourManager;

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

    private string _fieldName;

    private void Start()
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        canvas.enabled = false;
    }

    private void Update()
    {
        if (AllChosenUnits == _allChosenUnitsHistory) return;
        _allChosenUnitsHistory = AllChosenUnits;
        ourUnitsLabel.text = AllChosenUnits.ToString();
    }

    public void OnClickCancelButton()
    {
        SharedVariables.IsOverUi = false;
        fieldInspectorManager.EnableFieldInspector(_fieldName);
        canvas.enabled = false;
        AudioPlayer.PlayButtonClick();
    }

    public void OnClickAttackButton()
    {
        if (AllChosenUnits == 0) return;
        var parameters = FieldsParameters.LookupTable[_fieldName];
        var battleOutcome = CalculateBattleResult(parameters);
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
            NotificationsBarManager.SendNotification(
                $"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} is now in control of {Translator.TranslateField(parameters.Instance.name)}");

            parameters.Instance.EnableAppropriateGlowSprite();
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits,
                SceneGame.CurrentPlayerIndex);

            SellObjectIfOverLimit(parameters);
            parameters.Instance.objectsManager.EnableAppropriateObjects();

            SceneGame.GetCurrentPlayer().Income = SceneGame.GetCurrentPlayer().CalculateIncome();
            topStatsManager.RefreshValues();

            OnClickCancelButton();
            AudioPlayer.PlayRegroup();
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
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits,
                SceneGame.CurrentPlayerIndex);
            OnClickCancelButton();
            AudioPlayer.PlayRegroup();
        }
        else if (battleOutcome == 0)
        {
            parameters.AllUnits = 0;
            NotificationsBarManager.SendNotification(
                $"{Players.DescribeNameAsColor(parameters.Owner)} lost control of {Translator.TranslateField(parameters.Instance.name)} due to lack of units after {Players.DescribeNameAsColor(PhotonNetwork.NickName)} attack!");
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
            AudioPlayer.PlayAttack();
        }
        else if (battleOutcome < 0)
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
            NotificationsBarManager.SendNotification(
                $"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} attacked {Players.DescribeNameAsColor(parameters.Owner)} in {Translator.TranslateField(parameters.Instance.name)} but failed to gain control!");

            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits,
                Players.NameToIndex(parameters.Owner));
            OnClickCancelButton();
            AudioPlayer.PlayAttack();
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
            NotificationsBarManager.SendNotification(
                $"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} attacked {Players.DescribeNameAsColor(parameters.Owner)} and now controls {Translator.TranslateField(parameters.Instance.name)}!");

            parameters.Instance.EnableAppropriateGlowSprite();
            parameters.Instance.unitsManager.EnableAppropriateSprites(parameters.AllUnits,
                SceneGame.CurrentPlayerIndex);

            SellObjectIfOverLimit(parameters);
            parameters.Instance.objectsManager.EnableAppropriateObjects();

            SceneGame.GetCurrentPlayer().Income = SceneGame.GetCurrentPlayer().CalculateIncome();
            topStatsManager.RefreshValues();

            OnClickCancelButton();
            AudioPlayer.PlayAttack();
        }
    }

    public void EnableAttackModule(string fieldName)
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        _fieldName = fieldName;
        fieldNameLabel.text = Translator.TranslateField(fieldName);
        canvas.enabled = true;
        theirUnitsLabel.text = FieldInspectorManager.RegroupMode
            ? FieldsParameters.LookupTable[fieldName].AllUnits.ToString()
            : FieldsParameters.LookupTable[fieldName].UnitsCountDescription();
        ourUnitsLabel.text = "0";
        attackModeNeighbourManager.EnableAppropriateComponents(fieldName);
        ourColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);
        vsLabel.enabled = FieldsParameters.LookupTable[fieldName].Owner != null;
        vsLabel.text = FieldInspectorManager.RegroupMode ? "=>" : "vs";
        theirUnitsLabel.enabled = vsLabel.enabled;
        if (FieldsParameters.LookupTable[fieldName].Owner == null) theirColorManager.DisableImages();
        else
            theirColorManager.EnableAppropriateImage(Players.PlayersList.FindIndex(player =>
                player.Name == FieldsParameters.LookupTable[fieldName].Owner));
        SharedVariables.IsOverUi = true;
        attackButtonLabel.text = FieldInspectorManager.RegroupMode ? "move" : "to glory!";
    }

    /**
     * <summary>Calculates the outcome of a battle</summary>
     * <returns>More than zero if attacker won, less than zero if defender won, zero if tie</returns>
     */
    private static int CalculateBattleResult(FieldsParameters parameters)
    {
        return AllChosenUnits - parameters.AllUnits;
    }

    /**
     * <summary>This methods sells objects that player has just acquired through attack but is not able to maintain.</summary>
     * <param name="parameters">Parameters of a field that was just conquered by the player</param>
     */
    private static void SellObjectIfOverLimit(FieldsParameters parameters)
    {
        var player = Players.PlayersList[Players.NameToIndex(parameters.Owner)];

        // SELL LABS
        var maxLabs = GameplayConstants.ScienceLabLimits[player.LabsLimitLevel];
        if (parameters.Labs > maxLabs)
        {
            var difference = parameters.Labs - maxLabs;
            for (var i = 0; i < difference; i++)
            {
                player.Gold += parameters.CalculateCostOfSellingLab();
                parameters.Labs--;
            }

            ObjectChanged eventData = new(parameters.Instance.name, ObjectChanged.ObjectType.Lab, true, difference);
            PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            NotificationsBarManager.EnqueueNotification(
                $"Some labs in {Translator.TranslateField(parameters.Instance.name)} had to be sold");
        }

        // SELL TRENCHES
        var maxTrenches = GameplayConstants.TrenchesLimits[player.TrenchesLimitLevel];
        var countTrenches =
            FieldsParameters.LookupTable.Values.Count(p => p.Owner == player.Name && p.HasTrenches && !p.IsCapital);
        if (countTrenches > maxTrenches)
        {
            player.Gold += GameplayConstants.TrenchesBaseCost / 2;
            parameters.HasTrenches = false;
            ObjectChanged eventData = new(parameters.Instance.name, ObjectChanged.ObjectType.Trenches, true);
            PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            NotificationsBarManager.EnqueueNotification(
                $"Trenches in {Translator.TranslateField(parameters.Instance.name)} had to be sold");
        }
        
        // SELL FARMS
        var maxFarms = GameplayConstants.FarmLimits[player.FarmsLimitLevel];
        if (parameters.Farms > maxFarms)
        {
            var difference = parameters.Farms - maxFarms;
            for (var i = 0; i < difference; i++)
            {
                player.Gold += parameters.CalculateCostOfSellingFarm();
                parameters.Farms--;
            }
            
            ObjectChanged eventData = new(parameters.Instance.name, ObjectChanged.ObjectType.Farm, true, difference);
            PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            NotificationsBarManager.EnqueueNotification(
                $"Some farms in {Translator.TranslateField(parameters.Instance.name)} had to be sold");
        }
    }
}