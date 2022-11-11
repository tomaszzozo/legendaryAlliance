using System.Collections.Generic;
using UnityEngine;

namespace fields
{
    public class FieldsParameters 
    {
        public float CameraSize { get; }
        public Vector3 CameraPosition { get; }
        public string Owner { get; set; }
        public int Income;
        public int AvailableUnits;
        public int AllUnits;
        public Field Instance { get; set; }
        
        private FieldsParameters(float camSize, Vector2 camPosition, int income = 5)
        {
            CameraSize = camSize;
            CameraPosition = new Vector3(camPosition.x, camPosition.y, -10);
            Owner = null;
            Income = income;
            AvailableUnits = 0;
            AllUnits = 0;
        }

        public static readonly Dictionary<string, FieldsParameters> LookupTable = new()
        {
            {"argentyna", new FieldsParameters(14, new Vector2(-40f, -26f))},
            {"brazylia", new FieldsParameters(12, new Vector2(-36f, -14f))},
            {"peru", new FieldsParameters(12, new Vector2(-40f, -13f))},
            {"wenezuela", new FieldsParameters(12, new Vector2(-41f, -4f))}
        };

        public static readonly Dictionary<string, List<string>> Neighbours = new()
        {
            { "argentyna", new List<string> { "brazylia", "peru" } },
            { "brazylia", new List<string> { "wenezuela", "peru", "argentyna" } },
            { "peru", new List<string> { "brazylia", "wenezuela", "argentyna" } },
            { "wenezuela", new List<string> { "brazylia", "peru" } },
        };
    }
}
