using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;

public class SceneJoiningRoom : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject labelTitle;
    private TextMeshProUGUI _labelTitle;

    void Start()
    {
        _labelTitle = labelTitle.GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(AnimateText), 0, 0.5f);
        Connect();
    }

    private void AnimateText()
    {
        if (!_labelTitle.text.StartsWith("Joining")) return;
         _labelTitle.text += '.';
        if (_labelTitle.text.EndsWith("....."))
        {
            _labelTitle.text = "Joining room.";
        }
    }

    // networking
    private void Connect()
    {
        PhotonNetwork.NickName = GlobalVariables.GetUsername();
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
        PhotonNetwork.JoinRoom(GlobalVariables.GetRoomToJoin());
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        _labelTitle.text = $"Could not join room! ({message})";
        StartCoroutine(GoBackToEnterRoomId());
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(IfNoResponseGoBack());
    }

    void IOnEventCallback.OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (int)Events.EventTypes.UpdateRoomUI)
        {
            GlobalVariables.SharedData = (object[])photonEvent.CustomData;
            gameObject.AddComponent<SceneLoader>().LoadScene("SceneJoinGame");
        }
    }

    private IEnumerator IfNoResponseGoBack()
    {
        yield return new WaitForSeconds(10);

        _labelTitle.text = "No response from room!";

        yield return new WaitForSeconds(3);
        PhotonNetwork.Disconnect();
        gameObject.AddComponent<SceneLoader>().LoadScene("SceneEnterRoomId");
    }
}
