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
        
        // var brazylia = FieldsParameters.LookupTable["brazylia"];
        // var peru = FieldsParameters.LookupTable["peru"];
        // var argentyna = FieldsParameters.LookupTable["argentyna"];
        // var wenezuela = FieldsParameters.LookupTable["wenezuela"];
        //
        // Players.PlayersList[1].Name = "mock1";
        // Players.PlayersList[2].Name = "mock2";
        // Players.PlayersList[3].Name = "mock3";
        // brazylia.Owner = "mock1";
        // peru.Owner = "mock2";
        // wenezuela.Owner = "mock3";
        //
        // peru.AllUnits = 100;
        // brazylia.Farms = 100;
        // brazylia.AllUnits = 100;
        // wenezuela.AllUnits = 100;
        //
        // brazylia.Instance.objectsManager.EnableAppropriateObjects();
        // brazylia.Instance.EnableAppropriateBorderSprite();
        // wenezuela.Instance.EnableAppropriateBorderSprite();
        // peru.Instance.EnableAppropriateBorderSprite();
        // peru.Instance.unitsManager.EnableAppropriateSprites(100, 1);
        // brazylia.Instance.unitsManager.EnableAppropriateSprites(100, 2);
        // wenezuela.Instance.unitsManager.EnableAppropriateSprites(100, 3);
        //

        SceneGame.GetCurrentPlayer().Gold = 1001;
        TopStatsManager.Instance.RefreshValues();
    }
}
