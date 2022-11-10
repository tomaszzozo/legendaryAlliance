using System.Linq;
using fields;
using Photon.Pun;
using ScenesMainLoops;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FieldInspectorManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private ImageColorManager unitColorManager;
    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI ownerLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI unitsCountLabel;
    [FormerlySerializedAs("thisCanvas")] [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject buyUnitButton;
    [SerializeField] private GameObject attackButton;
    
    private TextMeshProUGUI _buyUnitButtonLabel;
    private TextMeshProUGUI _attackButtonLabel;
    private Button _buyUnitButton;
    private Button _attackButton;
    private FieldsParameters _parameters;
    private string _fieldName;
    
    public void ShowFieldInspector(FieldsParameters parameters, string fieldName)
    {
        _parameters = parameters;
        _fieldName = fieldName;
        
        unitColorManager.EnableAppropriateImage(
            Players.PlayersList.FindIndex(player => player.Name == _parameters.Owner));
        incomeLabel.text = Translator.TranslateIncome(_parameters.Income);
        nameLabel.text = Translator.TranslateField(fieldName);
        ownerLabel.text = Translator.TranslateOwner(_parameters.Owner);
        unitsCountLabel.text = _parameters.Owner != null ? 
            "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits
            : "";
        _buyUnitButton.image.enabled = _parameters.Owner == SceneGame.GetCurrentPlayer().Name;
        _buyUnitButton.interactable = _buyUnitButton.image.enabled && SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;
        _buyUnitButtonLabel.enabled = _buyUnitButton.image.enabled;
        EnableAttackButtonIfAbleToAttack();
        canvas.enabled = true;
    }

    public void HideFieldInspector()
    {
        canvas.enabled = false;
        // buy units event
    }

    public void OnBuyUnitButtonClick()
    {
        _parameters.AllUnits++;
        SceneGame.GetCurrentPlayer().Gold -= SceneGame.UnitBaseCost;
        _buyUnitButton.interactable = SceneGame.GetCurrentPlayer().Gold >= SceneGame.UnitBaseCost;
        SceneGame.GlobalVariables.SelectedFieldLocal.unitsManager.EnableAppropriateSprites(_parameters.AllUnits, SceneGame.CurrentPlayerIndex);
        unitsCountLabel.text = "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits;
    }

    private void Start()
    {
        _buyUnitButtonLabel = buyUnitButton.GetComponentInChildren<TextMeshProUGUI>();
        _attackButtonLabel = attackButton.GetComponentInChildren<TextMeshProUGUI>();
        _buyUnitButton = buyUnitButton.GetComponent<Button>();
        _attackButton = attackButton.GetComponent<Button>();
    }

    private void EnableAttackButtonIfAbleToAttack()
    {
        var enable = _parameters.Owner != SceneGame.GetCurrentPlayer().Name 
                     && FieldsParameters.Neighbours[_fieldName].Any(neighbourFieldName => 
                         FieldsParameters.LookupTable[neighbourFieldName].Owner == SceneGame.GetCurrentPlayer().Name);
        _attackButton.image.enabled = enable;
        _attackButtonLabel.enabled = enable;
    }
}
