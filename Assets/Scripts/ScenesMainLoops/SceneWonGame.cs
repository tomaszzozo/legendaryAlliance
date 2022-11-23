using Photon.Pun;

public class SceneWonGame : MonoBehaviourPunCallbacks
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