using Photon.Pun;
using TMPro;
using UnityEngine;

namespace ScenesMainLoops
{
    public class SceneLoggedInMenu : MonoBehaviour
    {
        public GameObject labelLoggedIn;

        private void Start()
        {
            PhotonNetwork.Disconnect();

            labelLoggedIn.GetComponent<TextMeshProUGUI>().text = $"Logged in as: {SharedVariables.GetUsername()}";
        }
    }
}