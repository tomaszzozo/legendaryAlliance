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
        
        public int startingGold;
        public int RoundCounter { get; private set; }
        public static int CurrentPlayerIndex { get; private set; }
        public const int UnitBaseCost = 15;
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
        
        public void OnEvent(EventData photonEvent)
        {
            if (new List<int> {(int)EventTypes.NextTurn, (int)EventTypes.CapitalSelected}.Contains(photonEvent.Code))
            {
                NextTurn();
            }
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
            buttonNextTurn.interactable = IsItMyTurn();
            clockIcon.enabled = !IsItMyTurn();
            labelButtonNextTurn.enabled = IsItMyTurn();

            if (!IsItMyTurn()) return;
            
            foreach (var (_, parameters) in FieldsParameters.LookupTable)
            {
                if (parameters.Owner == GetCurrentPlayer().Name) parameters.AvailableUnits = parameters.AllUnits;
            }
            
            _player.Income = _player.CalculateIncome();
            _player.Gold += _player.Income;
            
            topStatsManager.RefreshValues();
            AudioPlayer.PlayYourTurn();
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
            Players.Init(startingGold);
            _playerLabelOfIndex = new Dictionary<int, TextMeshProUGUI>
            {
                {0, labelP1},
                {1, labelP2},
                {2, labelP3},
                {3, labelP4}
            };
            
            // LABELS INIT
            if (BackgroundImage.Instance) BackgroundImage.Instance.Destroy();
            if (BackgroundImage.Instance2) BackgroundImage.Instance2.Destroy();
            
            topStatsManager.Init(_player);
            
            labelP1.transform.Translate(new Vector2(LabelOffset, 0));

            if (!SharedVariables.GetIsAdmin())
            {
                buttonNextTurn.interactable = false;
                clockIcon.enabled = true;
                labelButtonNextTurn.enabled = false;
            }
            
            fieldInspectorManager.HideFieldInspector(true);
        }
    }
}
