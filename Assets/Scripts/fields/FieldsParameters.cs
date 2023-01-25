using System.Collections.Generic;
using UnityEngine;

namespace fields
{
    public class FieldsParameters
    {
        public static Dictionary<string, FieldsParameters> LookupTable = new()
        {
            { "argentyna", new FieldsParameters(14, new Vector2(-40f, -26f)) },
            { "brazylia", new FieldsParameters(12, new Vector2(-36f, -14f)) },
            { "peru", new FieldsParameters(12, new Vector2(-40f, -13f)) },
            { "wenezuela", new FieldsParameters(12, new Vector2(-41f, -4f)) },
            { "ameryka centralna", new FieldsParameters(12, new Vector2(-50, 2.5f)) },
            { "stany zjednoczone zachodnie", new FieldsParameters(12, new Vector2(-50, 12.5f)) },
            { "stany zjednoczone wschodnie", new FieldsParameters(12, new Vector2(-44.5f, 9)) },
            { "kanada wschodnia", new FieldsParameters(12, new Vector2(-36.5f, 20f)) },
            { "ontario", new FieldsParameters(12, new Vector2(-46f, 21f)) },
            { "alberta", new FieldsParameters(12, new Vector2(-56f, 22f)) },
            { "grenlandia", new FieldsParameters(12, new Vector2(-31f, 32.5f)) },
            { "obszar północno zachodni", new FieldsParameters(12, new Vector2(-51f, 29f)) },
            { "alaska", new FieldsParameters(12, new Vector2(-63.5f, 28f)) },
            { "islandia", new FieldsParameters(12, new Vector2(-18f, 24.5f)) },
            { "skandynawia", new FieldsParameters(12, new Vector2(-10.5f, 23f)) },
            { "wielka brytania", new FieldsParameters(12, new Vector2(-24f, 15f)) },
            { "ukraina", new FieldsParameters(15, new Vector2(-3.5f, 16f)) },
            { "europa północna", new FieldsParameters(12, new Vector2(-12.5f, 12f)) },
            { "europa zachodnia", new FieldsParameters(12, new Vector2(-19f, 4f)) },
            { "europa południowa", new FieldsParameters(12, new Vector2(-11f, 4.5f)) },
            { "afryka północna", new FieldsParameters(12, new Vector2(-15f, -12f)) },
            { "egipt", new FieldsParameters(12, new Vector2(-8f, -8.5f)) },
            { "afryka wschodnia", new FieldsParameters(12, new Vector2(-2f, -20f)) },
            { "kongo", new FieldsParameters(12, new Vector2(-7.5f, -23f)) },
            { "afryka południowa", new FieldsParameters(12, new Vector2(-6f, -32f)) },
            { "madagaskar", new FieldsParameters(12, new Vector2(3f, -34f)) },
            { "środkowy wschód", new FieldsParameters(12, new Vector2(3f, -4f)) },
            { "afganistan", new FieldsParameters(12, new Vector2(10f, 8.5f)) },
            { "ural", new FieldsParameters(12, new Vector2(12.5f, 21.5f)) },
            { "syberia", new FieldsParameters(14, new Vector2(16f, 24f)) },
            { "jakuck", new FieldsParameters(12, new Vector2(30f, 30f)) },
            { "kamczatka", new FieldsParameters(12, new Vector2(38f, 23f)) },
            { "irkuck", new FieldsParameters(12, new Vector2(27.5f, 19.5f)) },
            { "japonia", new FieldsParameters(12, new Vector2(39.5f, 10.5f)) },
            { "mongolia", new FieldsParameters(12, new Vector2(29f, 11f)) },
            { "chiny", new FieldsParameters(12, new Vector2(27.5f, 5.5f)) },
            { "indie", new FieldsParameters(12, new Vector2(18f, -5f)) },
            { "syjam", new FieldsParameters(12, new Vector2(29f, -9f)) },
            { "indonezja", new FieldsParameters(12, new Vector2(30f, -20.5f)) },
            { "nowa gwinea", new FieldsParameters(12, new Vector2(39.5f, -17.5f)) },
            { "australia wschodnia", new FieldsParameters(12, new Vector2(37.5f, -31.5f)) },
            { "australia zachodnia", new FieldsParameters(12, new Vector2(45.5f, -31.5f)) }
        };

        public static readonly Dictionary<string, List<string>> Neighbours = new()
        {
            { "argentyna", new List<string> { "brazylia", "peru" } },
            { "brazylia", new List<string> { "wenezuela", "peru", "argentyna" } },
            { "peru", new List<string> { "brazylia", "wenezuela", "argentyna" } },
            { "wenezuela", new List<string> { "brazylia", "peru" } },
            {
                "ameryka centralna",
                new List<string> { "wenezuela", "stany zjednoczone wschodnie", "stany zjednoczone zachodnie" }
            },
            {
                "stany zjednoczone zachodnie",
                new List<string>
                {
                    "ameryka centralna", "stany zjednoczone wschodnie", "stany zjednoczone zachodnie", "ontario",
                    "alberta"
                }
            },
            {
                "stany zjednoczone wschodnie",
                new List<string> { "stany zjednoczone zachodnie", "kanada wschodnia", "ontario", "ameryka centralna" }
            },
            { "kanada wschodnia", new List<string> { "stany zjednoczone wschodnie", "ontario", "grenlandia", } },
            {
                "ontario",
                new List<string>
                {
                    "kanada wschodnia", "grenlandia", "alberta", "obszar północno zachodni",
                    "stany zjednoczone wschodnie", "stany zjednoczone zachodnie"
                }
            },
            {
                "alberta",
                new List<string> { "ontario", "obszar północno zachodni", "alaska", "stany zjednoczone zachodnie" }
            },
            {
                "grenlandia", new List<string> { "obszar północno zachodni", "ontario", "kanada wschodnia", "islandia" }
            },
            { "obszar północno zachodni", new List<string> { "grenlandia", "ontario", "alberta", "alaska" } },
            { "alaska", new List<string> { "obszar północno zachodni", "alberta", "kamczatka" } },
            { "islandia", new List<string> { "grenlandia", "skandynawia", "wielka brytania" } },
            { "skandynawia", new List<string> { "islandia", "wielka brytania", "europa północna", "ukraina" } },
            {
                "wielka brytania", new List<string> { "islandia", "skandynawia", "europa północna", "europa zachodnia" }
            },
            {
                "ukraina",
                new List<string>
                    { "skandynawia", "europa północna", "europa południowa", "ural", "afganistan", "środkowy wschód", }
            },
            {
                "europa północna",
                new List<string>
                    { "ukraina", "europa zachodnia", "europa południowa", "skandynawia", "wielka brytania" }
            },
            {
                "europa zachodnia",
                new List<string> { "europa północna", "europa południowa", "wielka brytania", "afryka północna" }
            },
            {
                "europa południowa",
                new List<string>
                    { "europa północna", "europa zachodnia", "ukraina", "afryka północna", "egipt", "środkowy wschód" }
            },
            {
                "afryka północna",
                new List<string>
                    { "brazylia", "egipt", "afryka wschodnia", "kongo", "europa południowa", "europa zachodnia" }
            },
            {
                "egipt",
                new List<string> { "afryka północna", "środkowy wschód", "afryka wschodnia", "europa południowa" }
            },
            {
                "afryka wschodnia",
                new List<string>
                    { "egipt", "środkowy wschód", "madagaskar", "afryka południowa", "kongo", "afryka północna" }
            },
            { "kongo", new List<string> { "afryka północna", "afryka południowa", "afryka wschodnia" } },
            { "afryka południowa", new List<string> { "kongo", "afryka wschodnia", "madagaskar" } },
            { "madagaskar", new List<string> { "afryka południowa", "afryka wschodnia" } },
            {
                "środkowy wschód",
                new List<string> { "egipt", "afryka wschodnia", "indie", "afganistan", "ural", "europa południowa" }
            },
            { "afganistan", new List<string> { "środkowy wschód", "indie", "chiny", "ural", "ukraina" } },
            { "ural", new List<string> { "afganistan", "chiny", "syberia", "ukraina" } },
            { "syberia", new List<string> { "ural", "chiny", "mongolia", "irkuck", "jakuck" } },
            { "jakuck", new List<string> { "syberia", "irkuck", "kamczatka" } },
            { "kamczatka", new List<string> { "jakuck", "irkuck", "mongolia", "japonia", "alaska" } },
            { "irkuck", new List<string> { "kamczatka", "mongolia", "syberia", "jakuck" } },
            { "japonia", new List<string> { "kamczatka", "mongolia" } },
            { "mongolia", new List<string> { "japonia", "chiny", "syberia", "irkuck", "kamczatka" } },
            { "chiny", new List<string> { "mongolia", "syberia", "ural", "afganistan", "indie", "syjam" } },
            { "indie", new List<string> { "środkowy wschód", "afganistan", "chiny", "syjam" } },
            { "syjam", new List<string> { "indie", "chiny", "indonezja" } },
            { "indonezja", new List<string> { "syjam", "nowa gwinea", "australia zachodnia" } },
            { "nowa gwinea", new List<string> { "indonezja", "australia wschodnia", "australia zachodnia" } },
            { "australia wschodnia", new List<string> { "nowa gwinea", "australia zachodnia" } },
            { "australia zachodnia", new List<string> { "australia wschodnia", "nowa gwinea", "indonezja" } }
        };

        public int AllUnits;
        public int AvailableUnits;
        public int Farms;
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
            Farms = 0;
        }

        public float CameraSize { get; }
        public Vector3 CameraPosition { get; }
        public string Owner { get; set; }
        public Field Instance { get; set; }

        public static void ResetAllFields()
        {
            foreach (var v in LookupTable.Values)
            {
                v.Owner = null;
                v.Income = 5;
                v.AvailableUnits = 0;
                v.AllUnits = 0;
                v.Labs = 0;
                v.Farms = 0;
                v.Owner = null;
            }
        }

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
                _ => "none (0)"
            };
        }

        public static string BuildingCountDescription(int count)
        {
            return count switch
            {
                > 59 => "monopoly (60+)",
                > 49 => "hectares (50-59)",
                > 39 => "acres (40-49)",
                > 29 => "lots (30-39)",
                > 19 => "manufactures (20-29)",
                > 14 => "pack (15-19)",
                > 9 => "several (10-14)",
                > 4 => "couple (5-9)",
                > 0 => "few (1-4)",
                _ => "none (0)"
            };
        }

        public int CalculateCostOfBuyingLab()
        {
            return GameplayConstants.LabBaseCost + GameplayConstants.ScienceLabCostIncrement * Labs;
        }

        public int CalculateCostOfBuyingFarm()
        {
            return GameplayConstants.FarmBaseCost + GameplayConstants.FarmCostIncrement * Farms;
        }

        public int CalculateCostOfSellingLab()
        {
            return (GameplayConstants.LabBaseCost +
                    GameplayConstants.ScienceLabCostIncrement * (Labs - 1)) / 2;
        }

        public int CalculateCostOfSellingFarm()
        {
            return (GameplayConstants.FarmBaseCost +
                    GameplayConstants.FarmCostIncrement * (Farms - 1)) / 2;
        }
    }
}