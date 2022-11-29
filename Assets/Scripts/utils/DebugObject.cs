using System.Collections;
using fields;
using ScenesMainLoops;
using UnityEngine;

public class DebugObject : MonoBehaviour
{
    [SerializeField] private bool enableDebugMode;
    private void Update()
    {
        if (!enableDebugMode) return;
        enableDebugMode = false;
        StartCoroutine(AfterCapitalSelected());
    }

    private static IEnumerator AfterCapitalSelected()
    {
        while (SceneGame.RoundCounter == 0) yield return new WaitForSeconds(0.1f);

        // MOCK TRENCHES
        // FieldsParameters.LookupTable["peru"].HasTrenches = true;
        // FieldsParameters.LookupTable["brazylia"].HasTrenches = true;
        // FieldsParameters.LookupTable["wenezuela"].HasTrenches = true;
        //
        // FieldsParameters.LookupTable["peru"].Instance.objectsManager.EnableAppropriateObjects();
        // FieldsParameters.LookupTable["brazylia"].Instance.objectsManager.EnableAppropriateObjects();
        // FieldsParameters.LookupTable["wenezuela"].Instance.objectsManager.EnableAppropriateObjects();

        // MOCK PLAYER
        Players.PlayersList[1].Name = "mock player";
        Players.PlayersList[0].Gold = 1000;
        
        // MOCK FIELDS
        FieldsParameters.LookupTable["peru"].Owner = Players.PlayersList[1].Name;
        FieldsParameters.LookupTable["peru"].Instance.EnableAppropriateBorderSprite();
        FieldsParameters.LookupTable["wenezuela"].Owner = Players.PlayersList[1].Name;
        FieldsParameters.LookupTable["wenezuela"].Instance.EnableAppropriateBorderSprite();
        //
        // FieldsParameters.LookupTable["brazylia"].Owner = Players.PlayersList[0].Name;
        // FieldsParameters.LookupTable["brazylia"].Instance.EnableAppropriateBorderSprite();
        
        // MOCK LABS
        FieldsParameters.LookupTable["peru"].Labs = 3;
        FieldsParameters.LookupTable["peru"].Instance.objectsManager.EnableAppropriateObjects();
        FieldsParameters.LookupTable["wenezuela"].Labs = 4;
        FieldsParameters.LookupTable["wenezuela"].Instance.objectsManager.EnableAppropriateObjects();
        FieldsParameters.LookupTable["brazylia"].Labs = 1;
        FieldsParameters.LookupTable["brazylia"].Instance.objectsManager.EnableAppropriateObjects();
        
        // MOCK UNITS
        FieldsParameters.LookupTable["wenezuela"].AllUnits = 10;
        FieldsParameters.LookupTable["wenezuela"].Instance.unitsManager.EnableAppropriateSprites(10, 1);

        TopStatsManager.Instance.RefreshValues();
    }
}
