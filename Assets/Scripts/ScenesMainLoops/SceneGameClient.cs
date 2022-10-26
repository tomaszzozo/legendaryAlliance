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
    public class SceneGameClient : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public struct Globals
        {
            public FieldClient SelectedFieldLocal { get; set; }
            public FieldClient SelectedFieldOnline { get; set; }
        }

        public static Globals GlobalVariables;
        
        public GameObject labelP1;
        public GameObject labelP2;
        public GameObject labelP3;
        public GameObject labelP4;

        private TextMeshProUGUI _labelP1;
        private TextMeshProUGUI _labelP2;
        private TextMeshProUGUI _labelP3;
        private TextMeshProUGUI _labelP4;
        public Button buttonNextTurn;
        public new Camera camera;
        public Canvas canvas;
        public Canvas fieldInspectMode;
        
        public int currentPlayerIndex;
        private const int LabelOffset = 30;

        private Dictionary<int, TextMeshProUGUI> _playerLabelOfIndex;

        private void Start()
        {
            _labelP1 = labelP1.GetComponent<TextMeshProUGUI>();
            _labelP2 = labelP2.GetComponent<TextMeshProUGUI>();
            _labelP3 = labelP3.GetComponent<TextMeshProUGUI>();
            _labelP4 = labelP4.GetComponent<TextMeshProUGUI>();
            buttonNextTurn = buttonNextTurn.GetComponent<Button>();
        
            _labelP1.text = (string)SharedVariables.SharedData[0];
            _labelP2.text = (string)SharedVariables.SharedData[1];
            _labelP3.text = (string)SharedVariables.SharedData[2];
            _labelP4.text = (string)SharedVariables.SharedData[3];

            _playerLabelOfIndex = new Dictionary<int, TextMeshProUGUI>
            {
                {0, _labelP1},
                {1, _labelP2 },
                {2, _labelP3 },
                {3, _labelP4 }
            };

            if (PhotonNetwork.CurrentRoom.PlayerCount < 4) _labelP4.gameObject.SetActive(false);
            if (PhotonNetwork.CurrentRoom.PlayerCount < 3) _labelP3.gameObject.SetActive(false);

            _labelP1.transform.Translate(new Vector2(LabelOffset, 0));
            buttonNextTurn.interactable = false;
        }

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
            NextTurn();
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)EventTypes.NextTurn, null, options, SendOptions.SendReliable);
        }
        
        public void OnClickBackButton()
        {
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

        private void NextTurn()
        {
            _playerLabelOfIndex[currentPlayerIndex].transform.Translate(new Vector2(-LabelOffset, 0));
            if (++currentPlayerIndex == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                currentPlayerIndex = 0;
            }
            _playerLabelOfIndex[currentPlayerIndex].transform.Translate(new Vector2(LabelOffset, 0));
            // if current player matches current player index, set button clickable
            buttonNextTurn.interactable = IsItMyTurn();
        }
        
        public bool IsItMyTurn()
        {
            return currentPlayerIndex == _playerLabelOfIndex.FirstOrDefault(x => x.Value.text == SharedVariables.GetUsername()).Key;
        }
    }
}
