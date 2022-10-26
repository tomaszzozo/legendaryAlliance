using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using UnityEngine;

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
            if (_redSprite.enabled) return;
            
            _redSprite.enabled = true;
            if (mainLoop.IsItMyTurn()) return;
            _blueSprite.enabled = false;
            _yellowSprite.enabled = false;
            _violetSprite.enabled = false;
        }

        private void OnMouseExit()
        {
            if (SceneGameHost.GlobalVariables.SelectedFieldLocal == this) return;
            _redSprite.enabled = false;
            if (SceneGameHost.GlobalVariables.SelectedFieldOnline != name) return;
            switch (mainLoop.currentPlayerIndex)
            {
                case 1: _blueSprite.enabled = true; break;
                case 2: _yellowSprite.enabled = true; break;
                case 3: _violetSprite.enabled = true; break;
            }
        }

        private void OnMouseDown()
        {
            if (SceneGameHost.GlobalVariables.SelectedFieldLocal == this)
            {
                SceneGameHost.GlobalVariables.SelectedFieldLocal = null;
            }
            else
            {
                if (SceneGameHost.GlobalVariables.SelectedFieldLocal != null)
                {
                    SceneGameHost.GlobalVariables.SelectedFieldLocal._redSprite.enabled = false;
                }
                
                _blueSprite.enabled = false;
                _yellowSprite.enabled = false;
                _violetSprite.enabled = false;
                _redSprite.enabled = true;
                SceneGameHost.GlobalVariables.SelectedFieldLocal = this;
                if (mainLoop.IsItMyTurn())
                {
                    SceneGameHost.GlobalVariables.SelectedFieldOnline = name;
                    OnlineSelectedFieldChange data = new(name);
                    RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
                    PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
                }
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == (int)EventTypes.OnlineSelectedFieldChange 
                && OnlineSelectedFieldChange.Deserialize((object[])photonEvent.CustomData).FieldName == name)
            {
                switch (mainLoop.currentPlayerIndex)
                {
                    case 1: _blueSprite.enabled = true; break;
                    case 2: _yellowSprite.enabled = true; break;
                    case 3: _violetSprite.enabled = true; break;
                }

                SceneGameHost.GlobalVariables.SelectedFieldOnline =
                    OnlineSelectedFieldChange.Deserialize((object[])photonEvent.CustomData).FieldName;
            }
        }
    }
}
