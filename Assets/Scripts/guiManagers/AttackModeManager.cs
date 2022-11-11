using fields;
using ScenesMainLoops;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackModeManager : MonoBehaviour
{
    [FormerlySerializedAs("manager")] [SerializeField] private AttackModeNeighbourManager attackModeManager;
    [SerializeField] private Canvas canvas;
    [SerializeField] private FieldInspectorManager fieldInspectorManager;
    [SerializeField] private TextMeshProUGUI theirUnitsLabel;
    [SerializeField] private TextMeshProUGUI ourUnitsLabel;
    [SerializeField] private ImageColorManager ourColorManager;
    [SerializeField] private ImageColorManager theirColorManager;
    [SerializeField] private TextMeshProUGUI vsLabel;
    

    public static int AllChosenUnits;
    private static int _allChosenUnitsHistory;

    private string _fieldName;
    
    public void OnClickCancelButton()
    {
        fieldInspectorManager.EnableFieldInspector(_fieldName);
        canvas.enabled = false;
    }

    public void EnableAttackModule(string fieldName)
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        _fieldName = fieldName;
        canvas.enabled = true;
        theirUnitsLabel.text = FieldsParameters.LookupTable[fieldName].AllUnits.ToString();
        ourUnitsLabel.text = "0";
        attackModeManager.EnableAppropriateComponents(fieldName);
        ourColorManager.EnableAppropriateImage(SceneGame.CurrentPlayerIndex);
        vsLabel.enabled = FieldsParameters.LookupTable[fieldName].Owner != null;
        theirUnitsLabel.enabled = vsLabel.enabled;
        if (FieldsParameters.LookupTable[fieldName].Owner != null) theirColorManager.DisableImages();
        else theirColorManager.EnableAppropriateImage(Players.PlayersList.FindIndex(player => player.Name == FieldsParameters.LookupTable[fieldName].Owner));
    }

    private void Update()
    {
        if (AllChosenUnits == _allChosenUnitsHistory) return;
        _allChosenUnitsHistory = AllChosenUnits;
        ourUnitsLabel.text = AllChosenUnits.ToString();
    }

    private void Start()
    {
        AllChosenUnits = 0;
        _allChosenUnitsHistory = 0;
        canvas.enabled = false;
    }
}
