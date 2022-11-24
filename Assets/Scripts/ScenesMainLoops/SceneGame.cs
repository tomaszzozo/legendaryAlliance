using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using fields;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScenesMainLoops
{
    public class SceneGame : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private const int LabelOffset = 30;
        public static Globals GlobalVariables;

        public Button buttonNextTurn;
        public new Camera camera;
        public Canvas canvas;
        public TextMeshProUGUI labelP1;
        public TextMeshProUGUI labelP2;
        public TextMeshProUGUI labelP3;
        public TextMeshProUGUI labelP4;
        public Image clockIcon;
        public TextMeshProUGUI labelButtonNextTurn;
        public FieldInspectorManager fieldInspectorManager;
        public TopStatsManager topStatsManager;
        private Players _player;
        private Dictionary<int, TextMeshProUGUI> _playerLabelOfIndex;

        public static int RoundCounter { get; private set; }
        public static int CurrentPlayerIndex { get; private set; }

        private void Start()
        {
            if (PhotonNetwork.InRoom)
            {
                labelP1.text = (string)SharedVariables.SharedData[0];
                labelP2.text = (string)SharedVariables.SharedData[1];
                labelP3.text = (string)SharedVariables.SharedData[2];
                labelP4.text = (string)SharedVariables.SharedData[3];
                Players.FillPlayerNames();
                if (PhotonNetwork.CurrentRoom.PlayerCount < 4) labelP4.enabled = false;
                if (PhotonNetwork.CurrentRoom.PlayerCount < 3) labelP3.enabled = false;
                _player = Players.PlayersList.Find(x => x.Name == SharedVariables.GetUsername());
            }
            else // TODO: delete on release
            {
                Players.PlayersList[0].Name = "UnityTest";
                _player = Players.PlayersList[0];
                SharedVariables.SetUsername("UnityTest");
                SharedVariables.SetIsAdmin(true);
                labelP2.enabled = false;
                labelP3.enabled = false;
                labelP4.enabled = false;
                labelP1.text = Players.PlayersList[0].Name;
            }

            // VARIABLES INIT
            ResetAllStaticVariables();
            Players.Init(GameplayConstants.StartingGold);
            FieldsParameters.ResetAllFields();
            _playerLabelOfIndex = new Dictionary<int, TextMeshProUGUI>
            {
                { 0, labelP1 },
                { 1, labelP2 },
                { 2, labelP3 },
                { 3, labelP4 }
            };

            // LABELS INIT
            if (BackgroundImage.Instance) BackgroundImage.Instance.Destroy();

            topStatsManager.Init(_player);

            labelP1.transform.Translate(new Vector2(LabelOffset, 0));

            buttonNextTurn.interactable = false;
            if (!SharedVariables.GetIsAdmin())
            {
                clockIcon.enabled = true;
                labelButtonNextTurn.enabled = false;
            }

            fieldInspectorManager.HideFieldInspector(true);
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)EventTypes.CapitalSelected:
                    var eventData = CapitalSelected.Deserialize(photonEvent.CustomData as object[]);
                    FieldsParameters.LookupTable[eventData.FieldName].Owner = eventData.Owner;
                    NextTurn();
                    break;
                case (int)EventTypes.NextTurn:
                    NextTurn();
                    break;
                case (int)EventTypes.AfterAttackUpdateFields:
                {
                    var data = AfterAttackUpdateFields.Deserialize((object[])photonEvent.CustomData);
                    for (var i = 0; i < data.FieldsUpdatedData.Count; i++)
                    {
                        var dataElement = data.FieldsUpdatedData[i];
                        var field = FieldsParameters.LookupTable[dataElement.FieldName];
                        field.Owner = dataElement.NewOwner;
                        field.Instance.EnableAppropriateBorderSprite();
                        if (i == 0) field.Instance.EnableAppropriateGlowSprite();
                        field.AllUnits = dataElement.AllUnits;
                        field.AvailableUnits = dataElement.AvailableUnits;
                        field.Instance.unitsManager.EnableAppropriateSprites(field.AllUnits,
                            Players.NameToIndex(field.Owner));
                    }

                    var thisPlayer = Players.PlayersList[Players.NameToIndex(SharedVariables.GetUsername())];
                    thisPlayer.Income = thisPlayer.CalculateIncome();
                    topStatsManager.RefreshValues();
                    break;
                }
                case (int)EventTypes.SomeoneWon:
                {
                    var sceneLoader = gameObject.AddComponent<SceneLoader>();
                    var whoWon = SomeoneWon.Deserialize(photonEvent.CustomData as object[]).WinnerNickName;
                    if (whoWon == "")
                        sceneLoader.LoadScene("SceneTie");
                    else if (whoWon == PhotonNetwork.NickName)
                        sceneLoader.LoadScene("SceneWonGame");
                    else
                        sceneLoader.LoadScene("SceneLostGame");
                    break;
                }
                case (int)EventTypes.SellEverything:
                {
                    var playerName = SellEverything.Deserialize(photonEvent.CustomData as object[]).PlayerName;
                    SellEveryAsset(playerName);
                    Players.PlayersList[Players.NameToIndex(playerName)].Conquered = true;
                    NextTurn();
                    break;
                }
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected! Implement save game screen here!");
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneDisconnected");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            RaiseEventOptions options = new() { TargetActors = new[] { newPlayer.ActorNumber } };
            PhotonNetwork.RaiseEvent((byte)EventTypes.RoomAlreadyInGameSignal, null, options, SendOptions.SendReliable);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (Players.PlayersList[Players.NameToIndex(otherPlayer.NickName)].Conquered)
            {
                NotificationsBarManager.EnqueueNotification(
                    $"{Players.DescribeNameAsColor(otherPlayer.NickName)} has left the game");
                return;
            }

            Debug.Log("Player left room! Implement save game screen here!");
            ScenePlayerLeftGame.PlayerThatLeftNickname = otherPlayer.NickName;
            gameObject.AddComponent<SceneLoader>().LoadScene("ScenePlayerLeftGame");
        }

        public void OnClickNextTurnButton()
        {
            AudioPlayer.PlayButtonClick();
            NextTurn();
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)EventTypes.NextTurn, null, options, SendOptions.SendReliable);
        }

        public void OnClickBackButton()
        {
            AudioPlayer.PlayButtonClick();

            fieldInspectorManager.HideFieldInspector();
            canvas.enabled = true;

            camera.orthographicSize += 4;
            CameraController.MovementEnabled = true;

            GlobalVariables.SelectedFieldLocal.DisableAllGlowSprites();
            GlobalVariables.SelectedFieldLocal = null;
            GlobalVariables.SelectedFieldOnline = null;

            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)EventTypes.OnlineDeselectField, null, options, SendOptions.SendReliable);
        }

        public void SetUiHover(bool set)
        {
            SharedVariables.IsOverUi = set;
        }

        public bool IsItMyTurn()
        {
            if (!PhotonNetwork.InRoom) return true; // TODO: delete on release
            return Players.PlayersList[CurrentPlayerIndex] == _player;
        }

        public static Players GetCurrentPlayer()
        {
            return Players.PlayersList[CurrentPlayerIndex];
        }

        public void NextTurn()
        {
            _playerLabelOfIndex[CurrentPlayerIndex].transform.Translate(new Vector2(-LabelOffset, 0));
            if (!PhotonNetwork.InRoom) // TODO: delete on release
            {
                CurrentPlayerIndex = 0;
                RoundCounter++;
            }
            else if (++CurrentPlayerIndex == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                CurrentPlayerIndex = 0;
                RoundCounter++;
            }

            _playerLabelOfIndex[CurrentPlayerIndex].transform.Translate(new Vector2(LabelOffset, 0));
            buttonNextTurn.interactable = IsItMyTurn() && RoundCounter != 0;
            clockIcon.enabled = !IsItMyTurn();
            labelButtonNextTurn.enabled = IsItMyTurn();

            if (PhotonNetwork.InRoom && IsTie())
            {
                SomeoneWon eventData = new("");
                PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                return;
            }

            if (PhotonNetwork.InRoom)
            {
                if (GetCurrentPlayer().Conquered)
                {
                    NextTurn();
                    return;
                }

                if (IsThisPlayerConquered())
                {
                    NotificationsBarManager.EnqueueNotification(GetCurrentPlayer().Name == _player.Name
                        ? "You got conquered!"
                        : $"{Players.DescribeNameAsColor(GetCurrentPlayer().Name)} got conquered!");

                    GetCurrentPlayer().Conquered = true;
                    NextTurn();
                    return;
                }
            }

            if (!IsItMyTurn()) return;

            _player.Income = _player.CalculateIncome();
            _player.Gold += _player.Income;
            _player.SciencePoints += _player.CalculateScienceIncome();

            if (_player.Gold < 0)
            {
                if (_player.InDebt)
                {
                    _player.Conquered = true;
                    SellEveryAsset(_player.Name);

                    SellEverything eventData = new(_player.Name);
                    PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                        new RaiseEventOptions { Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);

                    NotificationsBarManager.EnqueueNotification("You lost the game due to your debt!");
                    NotificationsBarManager.SendNotification(
                        $"{Players.DescribeNameAsColor(_player.Name)} lost the game due to his debt!");
                    
                    NextTurn();
                    return;
                }

                _player.InDebt = true;
                NotificationsBarManager.EnqueueNotification(
                    "You need to get out of debt that you have, otherwise you will loose in your next turn!");
            }

            if (PhotonNetwork.InRoom && DidThisPlayerWin())
            {
                SomeoneWon eventData = new(_player.Name);
                PhotonNetwork.RaiseEvent(eventData.GetEventType(), eventData.Serialize(),
                    new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                return;
            }

            SharedVariables.IsOverUi = false;
            foreach (var (_, parameters) in FieldsParameters.LookupTable)
                if (parameters.Owner == GetCurrentPlayer().Name)
                    parameters.AvailableUnits = parameters.AllUnits;


            topStatsManager.RefreshValues();
            AudioPlayer.PlayYourTurn();
        }

        private static bool IsThisPlayerConquered()
        {
            return RoundCounter >= 1 &&
                   FieldsParameters.LookupTable.Values.All(parameters => parameters.Owner != GetCurrentPlayer().Name);
        }

        private static bool IsTie()
        {
            return RoundCounter >= 1 && FieldsParameters.LookupTable.Values.All(parameters => parameters.Owner == null);
        }

        private bool DidThisPlayerWin()
        {
            if (RoundCounter < 1 || _player.InDebt) return false;
            return FieldsParameters.LookupTable.Values.All(parameters =>
                parameters.Owner == _player.Name || parameters.Owner == null);
        }

        private static void SellEveryAsset(string playerName)
        {
            foreach (var parameters in FieldsParameters.LookupTable.Values.Where(parameters =>
                         parameters.Owner == playerName))
            {
                parameters.Owner = null;
                parameters.Labs = 0;
                parameters.AllUnits = 0;
                parameters.AvailableUnits = 0;
                parameters.HasTrenches = false;
                parameters.Instance.EnableAppropriateBorderSprite();
                parameters.Instance.EnableAppropriateGlowSprite();
                parameters.Instance.unitsManager.EnableAppropriateSprites(0, 0);
            }
        }

        private static void ResetAllStaticVariables()
        {
            GlobalVariables.SelectedFieldLocal = null;
            GlobalVariables.SelectedFieldOnline = null;
            RoundCounter = 0;
            CurrentPlayerIndex = 0;
        }

        public struct Globals
        {
            public Field SelectedFieldLocal { get; set; }
            public Field SelectedFieldOnline { get; set; }
        }
    }
}