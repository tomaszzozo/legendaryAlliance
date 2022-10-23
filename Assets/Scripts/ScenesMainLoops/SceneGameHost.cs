using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScenesMainLoops
{
    public class SceneGameHost : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public GameObject labelP1;
        public GameObject labelP2;
        public GameObject labelP3;
        public GameObject labelP4;
        public GameObject buttonNextTurn;

        private TextMeshProUGUI _labelP1;
        private TextMeshProUGUI _labelP2;
        private TextMeshProUGUI _labelP3;
        private TextMeshProUGUI _labelP4;
        private Button _buttonNextTurn;

        private int _currentPlayerIndex;
        private const int LabelOffset = 30;

        private Dictionary<int, TextMeshProUGUI> _playerLabelOfIndex;

        void Start()
        {
            _labelP1 = labelP1.GetComponent<TextMeshProUGUI>();
            _labelP2 = labelP2.GetComponent<TextMeshProUGUI>();
            _labelP3 = labelP3.GetComponent<TextMeshProUGUI>();
            _labelP4 = labelP4.GetComponent<TextMeshProUGUI>();
            _buttonNextTurn = buttonNextTurn.GetComponent<Button>();
        
            _labelP1.text = (string)GlobalVariables.SharedData[0];
            _labelP2.text = (string)GlobalVariables.SharedData[1];
            _labelP3.text = (string)GlobalVariables.SharedData[2];
            _labelP4.text = (string)GlobalVariables.SharedData[3];

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
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Disconnected! Implement save game screen here!");
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneDisconnected");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            RaiseEventOptions options = new() { TargetActors =  new[] { newPlayer.ActorNumber } };
            PhotonNetwork.RaiseEvent((byte)Events.EventTypes.RoomAlreadyInGameSignal, null, options, SendOptions.SendReliable);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Player left room! Implement save game screen here!");
        }

        public void OnClickNextTurnButton()
        {
            NextTurn();
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent((byte)Events.EventTypes.NextTurn, null, options, SendOptions.SendReliable);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)Events.EventTypes.NextTurn)
            {
                NextTurn();
            }
        }
        
        private void NextTurn()
        {
            _playerLabelOfIndex[_currentPlayerIndex].transform.Translate(new Vector2(-LabelOffset, 0));
            if (++_currentPlayerIndex == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                _currentPlayerIndex = 0;
            }
            _playerLabelOfIndex[_currentPlayerIndex].transform.Translate(new Vector2(LabelOffset, 0));
            _buttonNextTurn.interactable = IsItMyTurn();
        }

        private bool IsItMyTurn()
        {
            return _currentPlayerIndex == 0;
        }
    }
}
