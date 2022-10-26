using System.Collections.Generic;
using UnityEngine;

namespace fields
{
    public static class FieldsParameters 
    {
        public struct FieldT
        {
            public FieldT(float camSize, Vector2 camPosition)
            {
                CameraSize = camSize;
                CameraPosition = new Vector3(camPosition.x, camPosition.y, -10);
                Owner = null;
            }
            
            public float CameraSize { get; }
            public Vector3 CameraPosition { get; }
            public string Owner { get; set; }
        }

        public static readonly Dictionary<string, FieldT> LookupTable = new()
        {
            {"argentyna", new FieldT(12, new Vector2(-30.89f, -28.67f))},
            {"brazylia", new FieldT(12, new Vector2(-26.52f, -14.1f))},
            {"peru", new FieldT(12, new Vector2(-34.13f, -15.68f))},
            {"wenezuela", new FieldT(12, new Vector2(-32.92f, -6.12f))}
        };
    }
}
