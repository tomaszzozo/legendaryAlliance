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
        public struct Globals
        {
            public Field SelectedFieldLocal { get; set; }
            public Field SelectedFieldOnline { get; set; }
        }
        public static Globals GlobalVariables;

        public static bool IsOverUi = false;
        public void SetUiHover(bool value) { IsOverUi = value; Debug.Log(value); }

        public Button buttonNextTurn;
        public int currentPlayerIndex;
        public new Camera camera;
        public Canvas canvas;
        public Canvas fieldInspectMode;
        public TextMeshProUGUI labelP1;
        public TextMeshProUGUI labelP2;
        public TextMeshProUGUI labelP3;
        public TextMeshProUGUI labelP4;
        
        private const int LabelOffset = 30;
        private Dictionary<int, TextMeshProUGUI> _playerLabelOfIndex;
        
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
        
        public bool IsItMyTurn()
        {
            if (!PhotonNetwork.InRoom) return true; // TODO: delete on release
            return currentPlayerIndex == _playerLabelOfIndex.FirstOrDefault(x => x.Value.text == SharedVariables.GetUsername()).Key;
        }

        
        private void Start()
        {
            labelP1.text = (string)SharedVariables.SharedData[0];
            labelP2.text = (string)SharedVariables.SharedData[1];
            labelP3.text = (string)SharedVariables.SharedData[2];
            labelP4.text = (string)SharedVariables.SharedData[3];

            _playerLabelOfIndex = new Dictionary<int, TextMeshProUGUI>
            {
                {0, labelP1},
                {1, labelP2 },
                {2, labelP3 },
                {3, labelP4 }
            };

            if (PhotonNetwork.CurrentRoom.PlayerCount < 4) labelP4.gameObject.SetActive(false);
            if (PhotonNetwork.CurrentRoom.PlayerCount < 3) labelP3.gameObject.SetActive(false);

            labelP1.transform.Translate(new Vector2(LabelOffset, 0));

            if (!SharedVariables.GetIsAdmin()) buttonNextTurn.interactable = false;
        }
        
        private void NextTurn()
        {
            _playerLabelOfIndex[currentPlayerIndex].transform.Translate(new Vector2(-LabelOffset, 0));
            if (++currentPlayerIndex == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                currentPlayerIndex = 0;
            }
            _playerLabelOfIndex[currentPlayerIndex].transform.Translate(new Vector2(LabelOffset, 0));
            buttonNextTurn.interactable = IsItMyTurn();
            
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent((byte)EventTypes.OnlineDeselectField, null, options, SendOptions.SendReliable);
        }
    }
}
