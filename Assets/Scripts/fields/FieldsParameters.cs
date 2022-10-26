using System.Collections.Generic;
using UnityEngine;

namespace fields
{
    public static class FieldsParameters 
    {
        public struct FieldT
        {
            public float CameraSize { get; set; }
            public Vector3 CameraPosition { get; set; }
        }

        public static readonly Dictionary<string, FieldT> LookupTable = new()
        {
            {"argentyna", new FieldT {CameraSize = 12, CameraPosition = new Vector3(-30.89f, -28.67f, -10)}},
            {"brazylia", new FieldT {CameraSize = 12, CameraPosition = new Vector3(-26.52f, -14.1f, -10)}},
            {"peru", new FieldT {CameraSize = 12, CameraPosition = new Vector3(-34.13f, -15.68f, -10)}},
            {"wenezuela", new FieldT {CameraSize = 12, CameraPosition = new Vector3(-32.92f, -6.12f, -10)}}
        };
    }
}
