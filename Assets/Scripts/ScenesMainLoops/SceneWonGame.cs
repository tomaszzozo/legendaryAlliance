using Photon.Pun;

public class SceneWonGame : MonoBehaviourPunCallbacks
{
    private static SceneLoader _sceneLoader;
    
    public void OnButtonClick()
    {
        _sceneLoader.LoadScene("SceneLoggedInMenu");
    }

    private void Start()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        _sceneLoader = gameObject.AddComponent<SceneLoader>();
    }
}