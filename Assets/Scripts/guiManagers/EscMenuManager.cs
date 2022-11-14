using Photon.Pun;
using ScenesMainLoops;
using UnityEngine;
using UnityEngine.UI;

public class EscMenuManager : MonoBehaviour
{
    [SerializeField] private Button buttonNextTurn;
    
    public static EscMenuManager Instance { get; private set; }
    private Canvas _canvas;
    private SceneLoader _sceneLoader;

    public void Show()
    {
        buttonNextTurn.interactable = false;
        _canvas.enabled = true;
        SharedVariables.IsOverUi = true;
    }

    public void Hide()
    {
        buttonNextTurn.interactable = SceneGame.RoundCounter != 0;
        _canvas.enabled = false;
        SharedVariables.IsOverUi = false;
    }

    public void OnClickQuitButton()
    {
        Hide();
        PhotonNetwork.Disconnect();
        _sceneLoader.LoadScene("SceneLoggedInMenu");
    }

    public bool IsVisible()
    {
        return _canvas.enabled;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);   
        }
        Instance = this;
    }

    private void Start()
    {
        _canvas = gameObject.GetComponent<Canvas>();
        _canvas.enabled = false;
        _sceneLoader = gameObject.AddComponent<SceneLoader>();
    }
}
