using System.Collections.Generic;
using UnityEngine;

namespace fields
{
    public class FieldsParameters
    {
        public static readonly Dictionary<string, FieldsParameters> LookupTable = new()
        {
            { "argentyna", new FieldsParameters(14, new Vector2(-40f, -26f)) },
            { "brazylia", new FieldsParameters(12, new Vector2(-36f, -14f)) },
            { "peru", new FieldsParameters(12, new Vector2(-40f, -13f)) },
            { "wenezuela", new FieldsParameters(12, new Vector2(-41f, -4f)) }
        };

        public static readonly Dictionary<string, List<string>> Neighbours = new()
        {
            { "argentyna", new List<string> { "brazylia", "peru" } },
            { "brazylia", new List<string> { "wenezuela", "peru", "argentyna" } },
            { "peru", new List<string> { "brazylia", "wenezuela", "argentyna" } },
            { "wenezuela", new List<string> { "brazylia", "peru" } }
        };

        public int AllUnits;
        public int AvailableUnits;
        public bool HasTrenches;
        public int Income;
        public bool IsCapital;
        public int Labs;

        private FieldsParameters(float camSize, Vector2 camPosition, int income = 5)
        {
            CameraSize = camSize;
            CameraPosition = new Vector3(camPosition.x, camPosition.y, -10);
            Owner = null;
            Income = income;
            AvailableUnits = 0;
            AllUnits = 0;
            Labs = 0;
        }

        public float CameraSize { get; }
        public Vector3 CameraPosition { get; }
        public string Owner { get; set; }
        public Field Instance { get; set; }

        public string UnitsCountDescription()
        {
            return AllUnits switch
            {
                > 59 => "legion (60+)",
                > 49 => "zounds (50-59)",
                > 39 => "swarm (40-49)",
                > 29 => "throng (30-39)",
                > 19 => "horde (20-29)",
                > 14 => "lots (15-19)",
                > 9 => "pack (10-14)",
                > 4 => "several (5-9)",
                > 0 => "few (1-4)",
                _ => "0"
            };
        }
    }
}