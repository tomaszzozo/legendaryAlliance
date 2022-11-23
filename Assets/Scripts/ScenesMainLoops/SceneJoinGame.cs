using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace ScenesMainLoops
{
    public class SceneJoinGame : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public GameObject labelRoomId;
        public GameObject labelAdminUsername;
        public GameObject labelP2;
        public GameObject labelP3;
        public GameObject labelP4;
        public GameObject labelStatus1;
        public GameObject labelStatus2;
        public GameObject labelStatus3;
        public GameObject labelStatus4;
        public GameObject labelButtonReady;

        private readonly bool _disconnectedIntentionally = false;
        private TextMeshProUGUI _labelAdminUsername;
        private TextMeshProUGUI _labelButtonReady;
        private TextMeshProUGUI _labelP2;
        private TextMeshProUGUI _labelP3;
        private TextMeshProUGUI _labelP4;

        private TextMeshProUGUI _labelRoomId;
        private TextMeshProUGUI _labelStatus1;
        private TextMeshProUGUI _labelStatus2;
        private TextMeshProUGUI _labelStatus3;
        private TextMeshProUGUI _labelStatus4;

        private void Start()
        {
            _labelRoomId = labelRoomId.GetComponent<TextMeshProUGUI>();
            _labelAdminUsername = labelAdminUsername.GetComponent<TextMeshProUGUI>();
            _labelRoomId.text = $"Room id: {PhotonNetwork.CurrentRoom.Name}";
            _labelP2 = labelP2.GetComponent<TextMeshProUGUI>();
            _labelP3 = labelP3.GetComponent<TextMeshProUGUI>();
            _labelP4 = labelP4.GetComponent<TextMeshProUGUI>();
            _labelStatus1 = labelStatus1.GetComponent<TextMeshProUGUI>();
            _labelStatus2 = labelStatus2.GetComponent<TextMeshProUGUI>();
            _labelStatus3 = labelStatus3.GetComponent<TextMeshProUGUI>();
            _labelStatus4 = labelStatus4.GetComponent<TextMeshProUGUI>();
            _labelButtonReady = labelButtonReady.GetComponent<TextMeshProUGUI>();

            PhotonNetwork.RaiseEvent((byte)EventTypes.RequestRoomData, null,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient }, SendOptions.SendReliable);
            // UpdateUi(UpdateRoomUi.Deserialize(SharedVariables.SharedData));
        }

        void IOnEventCallback.OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)EventTypes.UpdateRoomUI)
            {
                UpdateUi(UpdateRoomUi.Deserialize((object[])photonEvent.CustomData));
            }
            else if (photonEvent.Code == (int)EventTypes.GoToGameScene)
            {
                SharedVariables.SharedData = new object[]
                    { _labelAdminUsername.text, _labelP2.text, _labelP3.text, _labelP4.text };
                SharedVariables.SetIsAdmin(false);
                AudioMainTheme.Instance.Stop();
                gameObject.AddComponent<SceneLoader>().LoadScene("SceneGame");
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            gameObject.AddComponent<SceneLoader>()
                .LoadScene(_disconnectedIntentionally ? "SceneLoggedInMenu" : "SceneDisconnected");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.NickName.Equals(_labelAdminUsername.text)) PhotonNetwork.Disconnect();
        }

        // public void Disconnect()
        // {
        //     _disconnectedIntentionally = true;
        //     Debug.Log("Disconnected");
        //     PhotonNetwork.Disconnect();
        // }

        public void OnClickReady()
        {
            _labelButtonReady.text = _labelButtonReady.text.Equals("Not ready") ? "Ready" : "Not ready";
            ClientClickedReady data = new(PhotonNetwork.NickName);
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        }

        private void UpdateUi(UpdateRoomUi data)
        {
            _labelAdminUsername.text = data.Usernames[0];
            _labelP2.text = data.Usernames[1];
            _labelP3.text = data.Usernames[2];
            _labelP4.text = data.Usernames[3];
            _labelStatus1.text = data.Statuses[0];
            _labelStatus2.text = data.Statuses[1];
            _labelStatus3.text = data.Statuses[2];
            _labelStatus4.text = data.Statuses[3];
        }
    }
}