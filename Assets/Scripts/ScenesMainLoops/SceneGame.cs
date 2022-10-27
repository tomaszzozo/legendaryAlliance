using System.Collections.Generic;
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
        public struct Globals
        {
            public Field SelectedFieldLocal { get; set; }
            public Field SelectedFieldOnline { get; set; }
        }
        public static Globals GlobalVariables;

        public static bool IsOverUi;
        public void SetUiHover(bool value) { IsOverUi = value; Debug.Log(value); }

        public Button buttonNextTurn;
        public new Camera camera;
        public Canvas canvas;
        public Canvas fieldInspectMode;
        public TextMeshProUGUI labelP1;
        public TextMeshProUGUI labelP2;
        public TextMeshProUGUI labelP3;
        public TextMeshProUGUI labelP4;
        public TextMeshProUGUI labelCoins;
        public TextMeshProUGUI labelUsername;
        public Image decorationBar;
        public Image clockIcon;
        public TextMeshProUGUI labelButtonNextTurn;
        public TextMeshProUGUI labelIncome;
        
        public int startingGold;
        public int unitCost;
        public int baseIncome;
        private int CurrentPlayerIndex { get; set; }
        private const int LabelOffset = 30;
        private Dictionary<int, TextMeshProUGUI> _playerLabelOfIndex;
        private Players _player;
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected! Implement save game screen here!");
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneDisconnected");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            RaiseEventOptions options = new() { TargetActors =  new[] { newPlayer.ActorNumber } };
            PhotonNetwork.RaiseEvent((byte)EventTypes.RoomAlreadyInGameSignal, null, options, SendOptions.SendReliable);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Player left room! Implement save game screen here!");
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
            
            fieldInspectMode.enabled = false;
            canvas.enabled = true;
            
            camera.orthographicSize += 2;
            CameraController.MovementEnabled = true;
            
            GlobalVariables.SelectedFieldLocal.DisableAllSprites();
            GlobalVariables.SelectedFieldLocal = null;
            GlobalVariables.SelectedFieldOnline = null;

            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)EventTypes.OnlineDeselectField, null, options, SendOptions.SendReliable);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)EventTypes.NextTurn)
            {
                NextTurn();
            }
        }
        
        public bool IsItMyTurn()
        {
            if (!PhotonNetwork.InRoom) return true; // TODO: delete on release
            return Players.PlayersList[CurrentPlayerIndex] == _player;
        }

        private int CalculateIncome()
        {
            return baseIncome;
        }
        
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
            }
            else // TODO: delete on release
            {
                Players.PlayersList[0].Name = "UnityTest";
                SharedVariables.SetUsername("UnityTest");
                SharedVariables.SetIsAdmin(true);
                labelP2.enabled = false;
                labelP3.enabled = false;
                labelP4.enabled = false;
                labelP1.text = Players.PlayersList[0].Name;
            }
            
            // VARIABLES INIT
            Players.Init(startingGold, baseIncome);
            _player = Players.PlayersList.Find(x => x.Name == SharedVariables.GetUsername());
            _playerLabelOfIndex = new Dictionary<int, TextMeshProUGUI>
            {
                {0, labelP1},
                {1, labelP2},
                {2, labelP3},
                {3, labelP4}
            };
            
            // LABELS INIT
            decorationBar.color = _player.Color;
            
            labelCoins.text = _player.Gold.ToString();
            labelUsername.text = SharedVariables.GetUsername();
            labelIncome.text = _player.IncomeAsString();
            labelP1.transform.Translate(new Vector2(LabelOffset, 0));

            if (!SharedVariables.GetIsAdmin())
            {
                buttonNextTurn.interactable = false;
                clockIcon.enabled = true;
                labelButtonNextTurn.enabled = false;
            }
        }

        private void NextTurn()
        {
            _playerLabelOfIndex[CurrentPlayerIndex].transform.Translate(new Vector2(-LabelOffset, 0));
            if (!PhotonNetwork.InRoom) // TODO: delete on release
            {
                CurrentPlayerIndex = 0;
            }
            else if (++CurrentPlayerIndex == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                CurrentPlayerIndex = 0;
            }

            _playerLabelOfIndex[CurrentPlayerIndex].transform.Translate(new Vector2(LabelOffset, 0));
            buttonNextTurn.interactable = IsItMyTurn();
            clockIcon.enabled = !IsItMyTurn();
            labelButtonNextTurn.enabled = IsItMyTurn();

            RaiseEventOptions options = new() { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)EventTypes.OnlineDeselectField, null, options, SendOptions.SendReliable);

            if (Players.PlayersList[CurrentPlayerIndex].Name != SharedVariables.GetUsername()) return;

            _player.Income = CalculateIncome();
            _player.Gold += _player.Income;
            labelCoins.text = _player.Gold.ToString();
            
            if (IsItMyTurn()) AudioPlayer.PlayYourTurn();
        }
    }
}
