using fields;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackModeNeighbourComponent : MonoBehaviour
{
    [SerializeField] private GameObject addButton;
    [SerializeField] private GameObject subtractButton;
    [SerializeField] private TextMeshProUGUI fieldNameLabel;
    [SerializeField] private TextMeshProUGUI selectedUnitsCountLabel;

    private Button _addButton;
    private Button _subtractButton;
    private TextMeshProUGUI _addButtonLabel;
    private TextMeshProUGUI _subtractButtonLabel;

    private FieldsParameters _parameters;
    private int _selectedUnitsCount;

    public void OnClickAddButton()
    {
        if (_selectedUnitsCount == _parameters.AvailableUnits) return;
        selectedUnitsCountLabel.text = ++_selectedUnitsCount + "/" + _parameters.AvailableUnits;
        AttackModeManager.AllChosenUnits++;
    }

    public void OnClickSubtractButton()
    {
        if (_selectedUnitsCount == 0) return;
        selectedUnitsCountLabel.text = --_selectedUnitsCount + "/" + _parameters.AvailableUnits;
        AttackModeManager.AllChosenUnits--;
    }

    public void Enable(string fieldName)
    {
        _parameters = FieldsParameters.LookupTable[fieldName];
        _selectedUnitsCount = 0;
        
        fieldNameLabel.text = fieldName;
        selectedUnitsCountLabel.text =  "0/" + _parameters.AvailableUnits;
        
        _addButton.image.enabled = true;
        _addButtonLabel.enabled = true;
        _subtractButton.image.enabled = true;
        _subtractButton.enabled = true;
        fieldNameLabel.enabled = true;
        selectedUnitsCountLabel.enabled = true;
    }

    public void Disable()
    {
        _addButton.image.enabled = false;
        _addButtonLabel.enabled = false;
        _subtractButton.image.enabled = false;
        _subtractButtonLabel.enabled = false;
        fieldNameLabel.enabled = false;
        selectedUnitsCountLabel.enabled = false;
    }

    private void Start()
    {
        _addButton = addButton.GetComponent<Button>();
        _subtractButton = subtractButton.GetComponent<Button>();
        _addButtonLabel = addButton.GetComponentInChildren<TextMeshProUGUI>();
        _subtractButtonLabel = subtractButton.GetComponentInChildren<TextMeshProUGUI>();
    }
}
