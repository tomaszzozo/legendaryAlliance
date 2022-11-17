using UnityEngine;

public class FieldObjectsManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer trenches;

    /// <summary>
    /// Does not disables capital.
    /// </summary>
    public void DisableAllObjects()
    {
        trenches.enabled = false;
    }
}
