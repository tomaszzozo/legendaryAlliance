using Photon.Pun;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
        _labelP2 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelP3 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelP4 = labelP2.GetComponent<TextMeshProUGUI>();
        _labelStatus1 = labelStatus1.GetComponent<TextMeshProUGUI>();
        _labelStatus2 = labelStatus2.GetComponent<TextMeshProUGUI>();
        _labelStatus3 = labelStatus3.GetComponent<TextMeshProUGUI>();
        _labelStatus4 = labelStatus4.GetComponent<TextMeshProUGUI>();
        _labelButtonReady = labelButtonReady.GetComponent<TextMeshProUGUI>();

        UpdateUi(Events.UpdateRoomUi.Deserialize(GlobalVariables.sharedData));
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (_disconnectedIntentionally) return;
        gameObject.AddComponent<SceneLoader>().LoadScene("SceneDisconnected");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.NickName.Equals(_labelAdminUsername.text))
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void Disconnect()
    {
        _disconnectedIntentionally = true;
        Debug.Log("Disconnected");
        PhotonNetwork.Disconnect();
    }

    public void OnClickReady()
    {
        _labelButtonReady.text = _labelButtonReady.text.Equals("Not ready") ? "Ready" : "Not ready";
        Events.ClientClickedReady data = new(PhotonNetwork.NickName);
        RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (int)Events.EventTypes.UPDATE_ROOM_UI)
        {
            UpdateUi(Events.UpdateRoomUi.Deserialize((object[])photonEvent.CustomData));
        }
    }

    private void UpdateUi(Events.UpdateRoomUi data)
    {
        _labelAdminUsername.text = data.usernames[0];
        _labelP2.text = data.usernames[1];
        _labelP3.text = data.usernames[2];
        _labelP4.text = data.usernames[3];
        _labelStatus1.text = data.statuses[0];
        _labelStatus2.text = data.statuses[1];
        _labelStatus3.text = data.statuses[2];
        _labelStatus4.text = data.statuses[3];
    }
}
