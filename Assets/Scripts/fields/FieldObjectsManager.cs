using fields;
using UnityEngine;

public class FieldObjectsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trenches;

    /// <summary>
    /// Does not disable capitals.
    /// </summary>
    public void DisableAllObjects()
    {
        trenches.enabled = false;
    }

    public void EnableAppropriateObjects(string fieldName)
    {
        var parameters = FieldsParameters.LookupTable[fieldName];
        trenches.enabled = parameters.HasTrenches;
    }
}
