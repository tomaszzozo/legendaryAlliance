using System.Collections.Generic;
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

    public void EnableAppropriateComponents(string fieldName)
    {
        var neighbours = FieldsParameters.Neighbours[fieldName];
        var c = 0;
        foreach (var neighbour in neighbours)
        {
            if (FieldsParameters.LookupTable[neighbour].Owner == SceneGame.GetCurrentPlayer().Name)
            {
                _components[c++].Enable(neighbour);
            }
        }
        for (; c < 6; c++)
        {
            _components[c].Disable();
        }
    }

    private void Start()
    {
        _components = new List<AttackModeNeighbourComponent> { component1, component2, component3, component4, component5, component6 };
    }
}
