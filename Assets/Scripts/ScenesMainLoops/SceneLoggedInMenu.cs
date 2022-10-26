using TMPro;
using UnityEngine;

namespace ScenesMainLoops
{
    public class SceneLoggedInMenu : MonoBehaviour
    {
        public GameObject labelLoggedIn;

        void Start()
        {
            labelLoggedIn.GetComponent<TextMeshProUGUI>().text = $"Logged in as: {SharedVariables.GetUsername()}";
        }
    }
}
