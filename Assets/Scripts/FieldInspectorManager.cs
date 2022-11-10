using fields;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FieldInspectorManager : MonoBehaviour
{
    [SerializeField] private ImageColorManager unitColorManager;
    [SerializeField] private TextMeshProUGUI incomeLabel;
    [SerializeField] private TextMeshProUGUI ownerLabel;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI unitsCountLabel;
    [FormerlySerializedAs("thisCanvas")] [SerializeField] private Canvas canvas;

    private FieldsParameters _parameters;
    
    public void ShowFieldInspector(ref FieldsParameters parameters, string fieldName)
    {
        _parameters = parameters;
        unitColorManager.EnableAppropriateImage(
            Players.PlayersList.FindIndex(player => player.Name == _parameters.Owner));
        incomeLabel.text = "Income: " + _parameters.Income;
        nameLabel.text = fieldName;
        ownerLabel.text = "Owner: " + (_parameters.Owner ?? "unconquered land");
        unitsCountLabel.text = _parameters.Owner != null ? 
            "x " + _parameters.AvailableUnits + "/" + _parameters.AllUnits
            : "";
        canvas.enabled = true;
    }

    public void HideFieldInspector()
    {
        canvas.enabled = false;
    }
}
