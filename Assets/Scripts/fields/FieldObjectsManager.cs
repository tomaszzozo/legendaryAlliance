using fields;
using UnityEngine;

public class FieldObjectsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trenches;
    [SerializeField] private SpriteRenderer lab;
    [SerializeField] private SpriteRenderer farm;

    public FieldsParameters Parameters { private get; set; }

    /// <summary>
    ///     Does not disable capitals.
    /// </summary>
    public void DisableAllObjects()
    {
        trenches.enabled = false;
        lab.enabled = false;
        farm.enabled = false;
    }

    public void EnableAppropriateObjects()
    {
        trenches.enabled = Parameters.HasTrenches;
        lab.enabled = Parameters.Labs > 0;
        farm.enabled = Parameters.Farms > 0;
    }
}