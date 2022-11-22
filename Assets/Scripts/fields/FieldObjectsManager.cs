using fields;
using UnityEngine;

public class FieldObjectsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trenches;
    [SerializeField] private SpriteRenderer lab;

    /// <summary>
    /// Does not disable capitals.
    /// </summary>
    public void DisableAllObjects()
    {
        trenches.enabled = false;
        lab.enabled = false;
    }

    public void EnableAppropriateObjects(string fieldName)
    {
        var parameters = FieldsParameters.LookupTable[fieldName];
        trenches.enabled = parameters.HasTrenches;
        lab.enabled = parameters.Labs > 0;
    }
}
