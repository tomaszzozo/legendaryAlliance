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

        var brazylia = FieldsParameters.LookupTable["brazylia"];
        var peru = FieldsParameters.LookupTable["peru"];
        var argentyna = FieldsParameters.LookupTable["argentyna"];

        Players.PlayersList[1].Name = "mock";
        brazylia.Owner = "mock";
        peru.Owner = "mock";

        argentyna.AllUnits = 3;
        argentyna.AvailableUnits = 2;

        peru.AllUnits = 100;
        brazylia.Farms = 100;
        
        brazylia.Instance.objectsManager.EnableAppropriateObjects();
        brazylia.Instance.EnableAppropriateBorderSprite();
        peru.Instance.EnableAppropriateBorderSprite();
        peru.Instance.unitsManager.EnableAppropriateSprites(100, 1);

        TopStatsManager.Instance.RefreshValues();
    }
}
