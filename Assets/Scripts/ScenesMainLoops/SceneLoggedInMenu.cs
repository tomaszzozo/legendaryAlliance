using UnityEngine;
using TMPro;

public class SceneLoggedInMenu : MonoBehaviour
{
    public GameObject labelLoggedIn;

    void Start()
    {
        labelLoggedIn.GetComponent<TextMeshProUGUI>().text = $"Logged in as: {GlobalVariables.GetUsername()}";
    }
}
