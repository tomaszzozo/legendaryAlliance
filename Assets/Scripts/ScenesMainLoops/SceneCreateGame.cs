using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SceneCreateGame : MonoBehaviourPunCallbacks, IOnEventCallback
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

    private TextMeshProUGUI _labelRoomId;
    private TextMeshProUGUI _labelAdminUsername;
    private TextMeshProUGUI _labelP2;
    private TextMeshProUGUI _labelP3;
    private TextMeshProUGUI _labelP4;
    private TextMeshProUGUI _labelStatus1;
    private TextMeshProUGUI _labelStatus2;
    private TextMeshProUGUI _labelStatus3;
    private TextMeshProUGUI _labelStatus4;
    private TextMeshProUGUI _labelButtonReady;

    private bool _disconnectedIntentionally = false;

    void Start()
    {
        _labelRoomId = labelRoomId.GetComponent<TextMeshProUGUI>();
        _labelAdminUsername = labelAdminUsername.GetComponent<TextMeshProUGUI>();
        _labelRoomId.text = $"Room id: {PhotonNetwork.CurrentRoom.Name}";
        _labelAdminUsername.text = GlobalVariables.GetUsername();
        _labelP2 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelP3 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelP4 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelStatus1 = labelStatus1.GetComponent<TextMeshProUGUI>();
        _labelStatus2 = labelStatus2.GetComponent<TextMeshProUGUI>();
        _labelStatus3 = labelStatus3.GetComponent<TextMeshProUGUI>();
        _labelStatus4 = labelStatus4.GetComponent<TextMeshProUGUI>();
        _labelButtonReady = labelButtonReady.GetComponent<TextMeshProUGUI>();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (_disconnectedIntentionally) return;
        gameObject.AddComponent<SceneLoader>().LoadScene("SceneDisconnected");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer.NickName == GlobalVariables.GetUsername()) return;

        Debug.Log($"{newPlayer.NickName} has joined the room");

        if (_labelP2.text.Equals("Not connected"))
        {
            _labelP2.text = newPlayer.NickName;
        }
        else if (_labelP3.text.Equals("Not connected"))
        {
            _labelP3.text = newPlayer.NickName;
        }
        else
        {
            _labelP4.text = newPlayer.NickName;
        }

        RaiseEventUpdateRoomUi();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.NickName == GlobalVariables.GetUsername()) return;

        Debug.Log($"{otherPlayer.NickName} has left the room");

        if (_labelP2.text.Equals(otherPlayer.NickName))
        {
            _labelP2.text = "Not connected";
            _labelStatus2.text = "Not ready";
        }
        else if (_labelP3.text.Equals(otherPlayer.NickName))
        {
            _labelP3.text = "Not connected";
            _labelStatus3.text = "Not ready";
        }
        else
        {
            _labelP4.text = "Not connected";
            _labelStatus4.text = "Not ready";
        }

        RaiseEventUpdateRoomUi();
    }

    public void Disconnect()
    {
        _disconnectedIntentionally = true;
        Debug.Log("Disconnected");
        PhotonNetwork.Disconnect();
    }

    public void OnClickReadyButton()
    {
        _labelStatus1.text = _labelStatus1.text.Equals("Not ready") ? "Ready" : "Not ready";
        _labelButtonReady.text = _labelStatus1.text.Equals("Not ready") ? "Ready" : "Not ready";
        RaiseEventUpdateRoomUi();
    }

    public void OnClickStartGameButton()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            MessageBoxFactory.ShowAlertDialog("You need to invite at least one player!", gameObject);
            return;
        }

        if (_labelStatus1.text.Equals("Not ready")
            || (!_labelP2.text.Equals("Not connected") && _labelStatus2.text.Equals("Not ready"))
            || (PhotonNetwork.CurrentRoom.PlayerCount > 2 && !_labelP3.text.Equals("Not connected") && _labelStatus3.text.Equals("Not ready"))
            || (PhotonNetwork.CurrentRoom.PlayerCount == 4 && !_labelP4.text.Equals("Not connected") && _labelStatus4.text.Equals("Not ready")))
        {
            MessageBoxFactory.ShowAlertDialog("All players must be ready!", gameObject);
            return;
        }

        RaiseEventGoToGameScene();
        GlobalVariables.SharedData = new object[] { _labelAdminUsername.text, _labelP2.text, _labelP3.text, _labelP4.text };
        gameObject.AddComponent<SceneLoader>().LoadScene("SceneGameHost");
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (int)Events.EventTypes.ClientClickedReady)
        {
            string nickName = Events.ClientClickedReady.Deserialize((object[])photonEvent.CustomData).nickName;
            if (_labelP2.text.Equals(nickName))
            {
                _labelStatus2.text = _labelStatus2.text.Equals("Not ready") ? "Ready" : "Not ready";
            }
            else if (_labelP3.text.Equals(nickName))
            {
                _labelStatus3.text = _labelStatus3.text.Equals("Not ready") ? "Ready" : "Not ready";
            }
            else
            {
                _labelStatus4.text = _labelStatus4.text.Equals("Not ready") ? "Ready" : "Not ready";
            }
            RaiseEventUpdateRoomUi();
        }
    }

    private void RaiseEventUpdateRoomUi()
    {
        Events.UpdateRoomUi data = new(
            int.Parse(PhotonNetwork.CurrentRoom.Name),
            new string[] { _labelAdminUsername.text, _labelP2.text, _labelP3.text, _labelP4.text },
            new string[] { _labelStatus1.text, _labelStatus2.text, _labelStatus3.text, _labelStatus4.text });
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
    }

    private void RaiseEventGoToGameScene()
    {
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent((byte)Events.EventTypes.GoToGameScene, null, options, SendOptions.SendReliable);
    }
}
