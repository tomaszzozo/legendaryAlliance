using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace ScenesMainLoops
{
    public class SceneConnecting : MonoBehaviourPunCallbacks
    {
        public GameObject labelTitle;
        private TextMeshProUGUI _labelTitle;
        private int _counter = 0;

        void Start()
        {
            _labelTitle = labelTitle.GetComponent<TextMeshProUGUI>();
            InvokeRepeating(nameof(AnimateText), 0, 0.5f);
            Connect();
        }

        private void AnimateText()
        {
            if (!_labelTitle.text.StartsWith("Connecting")) return;
            _labelTitle.text += '.';
            if (_labelTitle.text.EndsWith("....."))
            {
                _labelTitle.text = "Connecting to servers.";
            }
        }

        // networking
        private void Connect()
        {
            PhotonNetwork.NickName = SharedVariables.GetUsername();
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private IEnumerator GoBackToMainMenu()
        {
            yield return new WaitForSeconds(3);

            PhotonNetwork.Disconnect();
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneLoggedInMenu");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _labelTitle.text = $"Could not connect to servers! ({cause})";
            StartCoroutine(GoBackToMainMenu());
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            CreateRoom();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            if (++_counter < 10)
            {
                CreateRoom();
                return;
            }
            _labelTitle.text = $"Could not create room! ({message})";
            StartCoroutine(GoBackToMainMenu());
        }

        public override void OnCreatedRoom()
        {
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneCreateGame");
        }

        private void CreateRoom()
        {
            string roomId = new System.Random().Next(100, 999).ToString();
            PhotonNetwork.CreateRoom(roomId, new RoomOptions() { MaxPlayers = 4 });
        }
    }
}
