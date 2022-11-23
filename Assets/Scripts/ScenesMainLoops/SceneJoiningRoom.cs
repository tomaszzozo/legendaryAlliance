using System.Collections;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace ScenesMainLoops
{
    public class SceneJoiningRoom : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public GameObject labelTitle;
        private bool _executeNoResponse = true;
        private TextMeshProUGUI _labelTitle;

        private void Start()
        {
            _labelTitle = labelTitle.GetComponent<TextMeshProUGUI>();
            InvokeRepeating(nameof(AnimateText), 0, 0.5f);
            Connect();
        }

        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)EventTypes.UpdateRoomUI)
            {
                SharedVariables.SharedData = (object[])photonEvent.CustomData;
                gameObject.AddComponent<SceneLoader>().LoadScene("SceneJoinGame");
            }
        }

        private void AnimateText()
        {
            if (!_labelTitle.text.StartsWith("Joining")) return;
            _labelTitle.text += '.';
            if (_labelTitle.text.EndsWith(".....")) _labelTitle.text = "Joining room.";
        }

        // networking
        private void Connect()
        {
            PhotonNetwork.NickName = SharedVariables.GetUsername();
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.AutomaticallySyncScene = true;
        }

        private IEnumerator GoBackToEnterRoomId()
        {
            yield return new WaitForSeconds(3);

            PhotonNetwork.Disconnect();
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneEnterRoomId");
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            _labelTitle.text = $"Could not connect to servers! ({cause})";
            StartCoroutine(GoBackToEnterRoomId());
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to master");
            Debug.Log(PhotonNetwork.JoinRoom(SharedVariables.GetRoomToJoin()));
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _labelTitle.text = $"Could not join room! ({message})";
            StartCoroutine(GoBackToEnterRoomId());
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Joined room");
            if (PhotonNetwork.CurrentRoom.Players.Values.Count(player =>
                    player.NickName == SharedVariables.GetUsername()) > 1)
            {
                StartCoroutine(SameUsername());
                return;
            }

            StartCoroutine(IfNoResponseGoBack());
        }

        private IEnumerator IfNoResponseGoBack()
        {
            yield return new WaitForSeconds(10);
            if (!_executeNoResponse) yield break;
            _labelTitle.text = "No response from room!";

            yield return new WaitForSeconds(3);
            PhotonNetwork.Disconnect();
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneEnterRoomId");
        }

        private IEnumerator SameUsername()
        {
            PhotonNetwork.Disconnect();
            _executeNoResponse = false;
            _labelTitle.text = "You are already in this room on another device!";

            yield return new WaitForSeconds(5);
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneEnterRoomId");
        }
    }
}