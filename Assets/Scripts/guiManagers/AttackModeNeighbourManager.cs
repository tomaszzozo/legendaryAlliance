using System.Collections.Generic;
using System.Linq;
using fields;
using ScenesMainLoops;
using UnityEngine;

public class AttackModeNeighbourManager : MonoBehaviour
{
    [SerializeField] private AttackModeNeighbourComponent component1;
    [SerializeField] private AttackModeNeighbourComponent component2;
    [SerializeField] private AttackModeNeighbourComponent component3;
    [SerializeField] private AttackModeNeighbourComponent component4;
    [SerializeField] private AttackModeNeighbourComponent component5;
    [SerializeField] private AttackModeNeighbourComponent component6;

    private List<AttackModeNeighbourComponent> _components;
    private string _fieldName;

    private void Start()
    {
        _components = new List<AttackModeNeighbourComponent>
            { component1, component2, component3, component4, component5, component6 };
    }

    public void EnableAppropriateComponents(string fieldName)
    {
        _fieldName = fieldName;
        var neighbours = FieldsParameters.Neighbours[fieldName];
        var c = 0;
        foreach (var neighbour in neighbours.Where(neighbour => FieldsParameters.LookupTable[neighbour].Owner == SceneGame.GetCurrentPlayer().Name))
        {
            _components[c++].Enable(neighbour);
        }

        for (; c < 6; c++)
        {
            _components[c].Disable();
        }
    }

    /// <summary>
    /// Sets units count in neighbour fields. Refreshes sprites. Returns list with updated parameters.
    /// </summary>
    public List<AfterAttackUpdateFields.FieldUpdatedData> UpdateNeighbours()
    {
        var updatedData = new List<AfterAttackUpdateFields.FieldUpdatedData>();
        foreach (var component in _components.Where(component => component.Parameters != null))
        {
            component.Parameters.AllUnits -= component.SelectedUnitsCount;
            component.Parameters.AvailableUnits -= component.SelectedUnitsCount;
            component.Parameters.Instance.unitsManager.EnableAppropriateSprites(component.Parameters.AllUnits,
                Players.NameToIndex(component.Parameters.Owner));
            updatedData.Add(new AfterAttackUpdateFields.FieldUpdatedData
            {
                FieldName = component.Parameters.Instance.name, AllUnits = component.Parameters.AllUnits,
                AvailableUnits = component.Parameters.AvailableUnits
            });
        }

        return updatedData;
    }
}