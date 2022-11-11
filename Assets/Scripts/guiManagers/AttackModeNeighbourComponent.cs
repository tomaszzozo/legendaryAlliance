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
    
    public int SelectedUnitsCount { get; private set; }
    public FieldsParameters Parameters { get; private set; }

    public void OnClickAddButton()
    {
        if (SelectedUnitsCount == Parameters.AvailableUnits) return;
        selectedUnitsCountLabel.text = ++SelectedUnitsCount + "/" + Parameters.AvailableUnits;
        AttackModeManager.AllChosenUnits++;
    }

    public void OnClickSubtractButton()
    {
        if (SelectedUnitsCount == 0) return;
        selectedUnitsCountLabel.text = --SelectedUnitsCount + "/" + Parameters.AvailableUnits;
        AttackModeManager.AllChosenUnits--;
    }

    public void Enable(string fieldName)
    {
        Parameters = FieldsParameters.LookupTable[fieldName];
        SelectedUnitsCount = 0;
        
        fieldNameLabel.text = Translator.TranslateField(fieldName);
        selectedUnitsCountLabel.text =  "0/" + Parameters.AvailableUnits;
        
        _addButton.image.enabled = true;
        _addButtonLabel.enabled = true;
        _subtractButton.image.enabled = true;
        _subtractButtonLabel.enabled = true;
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
        Parameters = null;
    }

    private void Start()
    {
        _addButton = addButton.GetComponent<Button>();
        _subtractButton = subtractButton.GetComponent<Button>();
        _addButtonLabel = addButton.GetComponentInChildren<TextMeshProUGUI>();
        _subtractButtonLabel = subtractButton.GetComponentInChildren<TextMeshProUGUI>();
    }
}
