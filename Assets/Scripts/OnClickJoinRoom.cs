using UnityEngine;
using TMPro;

public class OnClickJoinRoom : MonoBehaviour
{
    public GameObject inputField;

    private TMP_InputField _inputField;

    public void Start()
    {
        _inputField = inputField.GetComponent<TMP_InputField>();
    }

    public void OnClick()
    {
        GlobalVariables.SetRoomToJoin(_inputField.text);
        gameObject.AddComponent<SceneLoader>().LoadScene("SceneTryToJoinRoom");
    }
}
