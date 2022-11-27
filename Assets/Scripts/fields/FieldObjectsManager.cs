using fields;
using UnityEngine;

public class FieldObjectsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trenches;
    [SerializeField] private SpriteRenderer lab;

    public FieldsParameters Parameters { private get; set; }

    /// <summary>
    ///     Does not disable capitals.
    /// </summary>
    public void DisableAllObjects()
    {
        trenches.enabled = false;
        lab.enabled = false;
    }

    public void EnableAppropriateObjects()
    {
        trenches.enabled = Parameters.HasTrenches;
        lab.enabled = Parameters.Labs > 0;
    }
}