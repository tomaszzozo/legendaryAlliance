using Photon.Pun;

public class SceneLostGame : MonoBehaviourPunCallbacks
{
    private static SceneLoader _sceneLoader;

    private void Start()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        _sceneLoader = gameObject.AddComponent<SceneLoader>();
    }

    public void OnButtonClick()
    {
        _sceneLoader.LoadScene("SceneLoggedInMenu");
    }
}