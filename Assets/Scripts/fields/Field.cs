using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ScenesMainLoops;
using UnityEngine;
using UnityEngine.Serialization;

namespace fields
{
    public class Field : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        public SpriteRenderer redSprite;
        public SpriteRenderer blueSprite;
        public SpriteRenderer yellowSprite;
        public SpriteRenderer violetSprite;
        public SpriteRenderer graySprite;
        [FormerlySerializedAs("RedBorder")] public SpriteRenderer redBorder;
        [FormerlySerializedAs("BlueBorder")] public SpriteRenderer blueBorder;
        [FormerlySerializedAs("VioletBorder")] public SpriteRenderer violetBorder;
        [FormerlySerializedAs("YellowBorder")] public SpriteRenderer yellowBorder;
        public SceneGame mainLoop;
        public SpriteRenderer capitalRed;
        public SpriteRenderer capitalBlue;
        public SpriteRenderer capitalYellow;
        public SpriteRenderer capitalViolet;
        public UnitsManager unitsManager;
        public FieldObjectsManager objectsManager;

        private FieldsParameters _parameters;

        private void Start()
        {
            _parameters = FieldsParameters.LookupTable[name];
            _parameters.Instance = this;
            redBorder.sortingOrder = 10;
            blueBorder.sortingOrder = 10;
            yellowBorder.sortingOrder = 10;
            violetBorder.sortingOrder = 10;
            redSprite.sortingOrder = 11;
            blueSprite.sortingOrder = 11;
            yellowSprite.sortingOrder = 11;
            violetSprite.sortingOrder = 11;
            graySprite.sortingOrder = 11;
            unitsManager.EnableAppropriateSprites(0, 0);
            objectsManager.Parameters = _parameters;
            objectsManager.DisableAllObjects();
            EnableAppropriateCapitalSprite();
        }

        private void OnMouseDown()
        {
            if (SharedVariables.IsOverUi) return;
            if (SceneGame.GlobalVariables.SelectedFieldLocal != null || !mainLoop.IsItMyTurn()) return;

            AudioPlayer.PlayButtonClick();

            if (SceneGame.RoundCounter == 0)
            {
                if (_parameters.Owner != null)
                {
                    MessageBoxFactory.ShowAlertDialog("This area is already settled!", gameObject);
                    return;
                }

                if (FieldsParameters.Neighbours[name].Any(neighbourField =>
                        FieldsParameters.LookupTable[neighbourField].Owner != null))
                {
                    MessageBoxFactory.ShowAlertDialog("Can not settle so close to other players!", gameObject);
                    return;
                }

                DisableAllGlowSprites();
                _parameters.Owner = SceneGame.GetCurrentPlayer().Name;
                _parameters.AllUnits = 1;
                _parameters.IsCapital = true;
                unitsManager.EnableAppropriateSprites(1, SceneGame.CurrentPlayerIndex);
                _parameters.HasTrenches = true;
                EnableAppropriateBorderSprite();
                EnableAppropriateCapitalSprite();
                objectsManager.EnableAppropriateObjects();
                mainLoop.NextTurn();
                CapitalSelected newEvent = new(name, _parameters.Owner);
                RaiseEventOptions eventOptions = new() { Receivers = ReceiverGroup.Others };
                PhotonNetwork.RaiseEvent(newEvent.GetEventType(), newEvent.Serialize(), eventOptions,
                    SendOptions.SendReliable);
                NotificationsBarManager.SendNotification(
                    $"{Players.DescribeNameAsColor(PhotonNetwork.NickName)} settled in {Translator.TranslateField(name)}");
                return;
            }

            mainLoop.fieldInspectorManager.EnableFieldInspector(name);
            mainLoop.canvas.enabled = false;

            CameraController.MovementEnabled = false;

            mainLoop.camera.transform.position = _parameters.CameraPosition;
            mainLoop.camera.orthographicSize = _parameters.CameraSize;

            SceneGame.GlobalVariables.SelectedFieldOnline = this;
            SceneGame.GlobalVariables.SelectedFieldLocal = this;

            OnlineSelectedFieldChange data = new(name);
            RaiseEventOptions options = new() { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(data.GetEventType(), data.Serialize(), options, SendOptions.SendReliable);
        }

        private void OnMouseEnter()
        {
            if (!mainLoop.IsItMyTurn()) return;
            if (SharedVariables.IsOverUi) return;
            if (SceneGame.GlobalVariables.SelectedFieldLocal != null) return;
            EnableAppropriateGlowSprite();
            AudioPlayer.PlayFieldHover();
        }

        private void OnMouseExit()
        {
            if (!mainLoop.IsItMyTurn()) return;
            if (SceneGame.GlobalVariables.SelectedFieldLocal != null) return;
            DisableAllGlowSprites();
        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case (int)EventTypes.OnlineDeselectField when SceneGame.GlobalVariables.SelectedFieldOnline != this:
                    return;
                case (int)EventTypes.OnlineDeselectField:
                    DisableAllGlowSprites();
                    SceneGame.GlobalVariables.SelectedFieldOnline = null;
                    break;
                case (int)EventTypes.OnlineSelectedFieldChange:
                {
                    var data = OnlineSelectedFieldChange.Deserialize((object[])photonEvent.CustomData);
                    if (name != data.FieldName) return;
                    EnableAppropriateGlowSprite();
                    SceneGame.GlobalVariables.SelectedFieldOnline = this;
                    break;
                }
                case (int)EventTypes.CapitalSelected:
                {
                    var data = CapitalSelected.Deserialize((object[])photonEvent.CustomData);
                    if (name != data.FieldName) return;
                    _parameters.Owner = data.Owner;
                    _parameters.HasTrenches = true;
                    _parameters.AllUnits = 1;
                    unitsManager.EnableAppropriateSprites(1, Players.NameToIndex(_parameters.Owner));
                    EnableAppropriateBorderSprite();
                    EnableAppropriateCapitalSprite();
                    objectsManager.EnableAppropriateObjects();
                    _parameters.IsCapital = true;
                    break;
                }
                case (int)EventTypes.BuyUnits:
                {
                    var data = BuyUnits.Deserialize((object[])photonEvent.CustomData);
                    if (name != data.FieldName) return;
                    _parameters.AllUnits = data.AllUnits;
                    _parameters.AvailableUnits = data.AvailableUnits;
                    Players.PlayersList.Find(player => player.Name == data.Owner).Gold = data.Gold;
                    unitsManager.EnableAppropriateSprites(_parameters.AllUnits, Players.NameToIndex(_parameters.Owner));
                    break;
                }

                case (int)EventTypes.ObjectChanged:
                {
                    var eventData = ObjectChanged.Deserialize(photonEvent.CustomData as object[]);
                    if (eventData.FieldName != name) return;
                    switch (eventData.ObjectName)
                    {
                        case ObjectChanged.ObjectType.Trenches:
                            _parameters.HasTrenches = !eventData.Sold;
                            break;
                        case ObjectChanged.ObjectType.Lab:
                            _parameters.Labs = eventData.Sold
                                ? _parameters.Labs - eventData.Count
                                : _parameters.Labs + eventData.Count;
                            break;
                        case ObjectChanged.ObjectType.Farm:
                            _parameters.Farms = eventData.Sold
                                ? _parameters.Farms - eventData.Count
                                : _parameters.Farms + eventData.Count;
                            break;
                        case ObjectChanged.ObjectType.Unit:
                            _parameters.AllUnits = eventData.Sold
                                ? _parameters.AllUnits - eventData.Count
                                : _parameters.AllUnits + eventData.Count;
                            if (_parameters.AllUnits < _parameters.AvailableUnits)
                                _parameters.AvailableUnits = _parameters.AllUnits;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    unitsManager.EnableAppropriateSprites(_parameters.AllUnits, Players.NameToIndex(_parameters.Owner));
                    objectsManager.EnableAppropriateObjects();
                    EnableAppropriateBorderSprite();
                    break;
                }
            }
        }

        public void DisableAllGlowSprites()
        {
            redSprite.enabled = false;
            blueSprite.enabled = false;
            yellowSprite.enabled = false;
            violetSprite.enabled = false;
            graySprite.enabled = false;
        }

        public void EnableAppropriateBorderSprite()
        {
            DisableAllBorderSprites();
            if (_parameters.Owner == Players.PlayersList[0].Name) redBorder.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[1].Name) blueBorder.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[2].Name) yellowBorder.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[3].Name) violetBorder.enabled = true;
        }

        public void DisableAllBorderSprites()
        {
            redBorder.enabled = false;
            blueBorder.enabled = false;
            yellowBorder.enabled = false;
            violetBorder.enabled = false;
        }

        public void EnableAppropriateGlowSprite()
        {
            DisableAllGlowSprites();
            if (_parameters.Owner == null) graySprite.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[0].Name) redSprite.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[1].Name) blueSprite.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[2].Name) yellowSprite.enabled = true;
            else if (_parameters.Owner == Players.PlayersList[3].Name) violetSprite.enabled = true;
        }

        private void EnableAppropriateCapitalSprite()
        {
            if (_parameters.Owner == Players.PlayersList[0].Name)
            {
                capitalRed.enabled = true;
            }
            else if (_parameters.Owner == Players.PlayersList[1].Name)
            {
                capitalBlue.enabled = true;
            }
            else if (_parameters.Owner == Players.PlayersList[2].Name)
            {
                capitalYellow.enabled = true;
            }
            else if (_parameters.Owner == Players.PlayersList[3].Name)
            {
                capitalViolet.enabled = true;
            }
            else
            {
                capitalRed.enabled = false;
                capitalBlue.enabled = false;
                capitalYellow.enabled = false;
                capitalViolet.enabled = false;
            }
        }
    }
}