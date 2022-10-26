using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using UnityEngine;

namespace fields
{
    public class FieldClient : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public SpriteRenderer redSprite;
        public SpriteRenderer blueSprite;
        public SpriteRenderer yellowSprite;
        public SpriteRenderer violetSprite;
        public SpriteRenderer graySprite;
        public SceneGameClient mainLoop;
        private FieldsParameters.FieldT _parameters;
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)EventTypes.OnlineDeselectField)
            {
                if (SceneGameClient.GlobalVariables.SelectedFieldOnline != this) return;
                DisableAllSprites();
                SceneGameClient.GlobalVariables.SelectedFieldOnline = null;
            }
            else if (photonEvent.Code == (int)EventTypes.OnlineSelectedFieldChange)
            {
                var data = OnlineSelectedFieldChange.Deserialize((object[])photonEvent.CustomData);
                if (name != data.FieldName) return;
                EnableAppropriateSprite();
                SceneGameClient.GlobalVariables.SelectedFieldOnline = this;
            }
        }

        public void DisableAllSprites()
        {
            redSprite.enabled = false;
            blueSprite.enabled = false;
            yellowSprite.enabled = false;
            violetSprite.enabled = false;
            graySprite.enabled = false;
        }

        private void EnableAppropriateSprite()
        {
            if (_parameters.Owner == null) graySprite.enabled = true;
            else if (_parameters.Owner == SharedVariables.SharedData[0].ToString()) redSprite.enabled = true;
            else if (_parameters.Owner == SharedVariables.SharedData[1].ToString()) blueSprite.enabled = true;
            else if (_parameters.Owner == SharedVariables.SharedData[2].ToString()) yellowSprite.enabled = true;
            else if (_parameters.Owner == SharedVariables.SharedData[3].ToString()) violetSprite.enabled = true;
        }
        
        private void Start()
        {
            _parameters = FieldsParameters.LookupTable[name];
        }

        private void OnMouseEnter()
        {
            if (!mainLoop.IsItMyTurn()) return;
            if (SceneGameClient.GlobalVariables.SelectedFieldLocal != null) return;
            EnableAppropriateSprite();
        }

        private void OnMouseExit()
        {
            if (!mainLoop.IsItMyTurn()) return;
            if (SceneGameClient.GlobalVariables.SelectedFieldLocal != null) return;
            DisableAllSprites();
        }

        private void OnMouseDown()
        {
            if (SceneGameClient.GlobalVariables.SelectedFieldLocal != null || !mainLoop.IsItMyTurn()) return;
            
            mainLoop.canvas.enabled = false;
            mainLoop.fieldInspectMode.enabled = true;
            
            CameraController.MovementEnabled = false;
            
            mainLoop.camera.transform.position = _parameters.CameraPosition;
            mainLoop.camera.orthographicSize = _parameters.CameraSize;
            
            SceneGameClient.GlobalVariables.SelectedFieldOnline = this;
            SceneGameClient.GlobalVariables.SelectedFieldLocal = this;
            
            OnlineSelectedFieldChange data = new(name);
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        }
    }
}
