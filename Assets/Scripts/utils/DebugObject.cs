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

        var argentyna = FieldsParameters.LookupTable["argentyna"];
        argentyna.AllUnits = 10;
        argentyna.Farms = 1;
        argentyna.Instance.objectsManager.EnableAppropriateObjects();
        argentyna.Instance.unitsManager.EnableAppropriateSprites(10, 0);

        TopStatsManager.Instance.RefreshValues();
    }
}