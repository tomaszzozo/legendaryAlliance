using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace fields
{
    public class FieldHost : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public GameObject glowRed;
        public GameObject glowBlue;
        public GameObject glowYellow;
        public GameObject glowViolet;
        [SerializeField] SceneGameHost mainLoop;
        
        private SpriteRenderer _redSprite;
        private SpriteRenderer _blueSprite;
        private SpriteRenderer _yellowSprite;
        private SpriteRenderer _violetSprite;

        private void Start()
        {
            _redSprite = glowRed.GetComponent<SpriteRenderer>();
            _blueSprite = glowBlue.GetComponent<SpriteRenderer>();
            _yellowSprite = glowYellow.GetComponent<SpriteRenderer>();
            _violetSprite = glowViolet.GetComponent<SpriteRenderer>();
            _redSprite.sortingOrder = 11;
        }

        private void OnMouseEnter()
        {
            
        }

        private void OnMouseExit()
        {
            
        }

        private void OnMouseDown()
        {
            if (SceneGameHost.GlobalVariables.SelectedFieldLocal != null || !mainLoop.IsItMyTurn()) return;

            mainLoop.canvas.enabled = false;
            mainLoop.fieldInspectMode.enabled = true;

            SharedVariables.SetCameraSize(mainLoop.camera.orthographicSize);
            SharedVariables.SetCameraPosition(mainLoop.camera.transform.position);
            CameraController.MovementEnabled = false;
            
            var parameters = FieldsParameters.LookupTable[name];
            mainLoop.camera.transform.position = parameters.CameraPosition;
            mainLoop.camera.orthographicSize = parameters.CameraSize;
            
            SceneGameHost.GlobalVariables.SelectedFieldOnline = this;
            SceneGameHost.GlobalVariables.SelectedFieldLocal = this;

            // OnlineSelectedFieldChange data = new(name);
            // RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            // PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            
        }
    }
}
