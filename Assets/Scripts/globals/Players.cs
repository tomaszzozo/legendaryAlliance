using System.Collections.Generic;
using System.Linq;
using fields;
using UnityEngine;

public class Players
{
    public static readonly List<Players> PlayersList = new()
    {
        new Players { Name = "", Color = new Color(255 / 255f, 0 / 255f, 0 / 255f), Gold = 0 },
        new Players { Name = "", Color = new Color(0 / 255f, 24 / 255f, 255 / 255f), Gold = 0 },
        new Players { Name = "", Color = new Color(255 / 255f, 246 / 255f, 0 / 255f), Gold = 0 },
        new Players { Name = "", Color = new Color(162 / 255f, 0 / 255f, 255 / 255f), Gold = 0 }
    };

    public Color Color;
    public bool Conquered;
    public int FarmsLimitLevel;
    public int Gold;
    public int Income;
    public bool InDebt;
    public int LabsLimitLevel;
    public string Name;
    public int SciencePoints;
    public int TrenchesLimitLevel;

    public static void FillPlayerNames()
    {
        PlayersList[0].Name = SharedVariables.SharedData[0].ToString();
        PlayersList[1].Name = SharedVariables.SharedData[1].ToString();
        PlayersList[2].Name = SharedVariables.SharedData[2].ToString();
        PlayersList[3].Name = SharedVariables.SharedData[3].ToString();
    }

    public static void Init(int gold)
    {
        foreach (var player in PlayersList)
        {
            player.Gold = gold;
            player.Income = 0;
            player.LabsLimitLevel = 0;
            player.TrenchesLimitLevel = 0;
            player.Conquered = false;
            player.SciencePoints = 0;
            player.InDebt = false;
            player.FarmsLimitLevel = 0;
        }
    }

    public static int NameToIndex(string name)
    {
        return PlayersList.FindIndex(player => player.Name == name);
    }

    public string IncomeAsString()
    {
        return Income >= 0 ? $"(+{Income})" : $"({Income})";
    }

    public int CalculateIncome()
    {
        var incomeSum = FieldsParameters.LookupTable.Values
            .Where(field => field.Owner == Name)
            .Sum(field => field.Income);
        var capitalBonus = FieldsParameters.LookupTable.Values.Count(field => field.Owner == Name && field.IsCapital) *
                           GameplayConstants.CapitalBonus;
        var trenchesPenalty = FieldsParameters.LookupTable.Values
            .Count(field => field.Owner == Name && field.HasTrenches) * GameplayConstants.TrenchesIncomeCost;
        var labsPenalty = FieldsParameters.LookupTable.Values
            .Where(field => field.Owner == Name)
            .Sum(field => field.Labs) * GameplayConstants.LabIncomeCost;
        var farmsPenalty = FieldsParameters.LookupTable.Values.Where(f => f.Owner == Name).Sum(f => f.Farms) *
                           GameplayConstants.FarmIncomeCost;
        return incomeSum + capitalBonus - trenchesPenalty - labsPenalty - farmsPenalty;
    }

    public int CalculateScienceIncome()
    {
        return FieldsParameters.LookupTable.Values.Where(parameters => parameters.Labs > 0 && parameters.Owner == Name)
            .Sum(parameters => parameters.Labs);
    }

    public string ScienceIncomeAsString()
    {
        return $"(+{CalculateScienceIncome()})";
    }

    public int CalculateMaxUnits()
    {
        var unitsFromCapitals = FieldsParameters.LookupTable.Values.Count(p => p.IsCapital && p.Owner == Name) *
                                GameplayConstants.FarmCapitalBonus;
        var unitsFromFarms = FieldsParameters.LookupTable.Values.Where(p => p.Owner == Name).Sum(p => p.Farms) *
                             GameplayConstants.AvailableUnitsPerFarm;
        return unitsFromCapitals + unitsFromFarms < 3 ? 3 : unitsFromCapitals + unitsFromFarms;
    }

    public int GetUnits()
    {
        return FieldsParameters.LookupTable.Values.Where(p => p.Owner == Name).Sum(p => p.AllUnits);
    }

    public string MaxUnitsAsString()
    {
        var farmsCount = FieldsParameters.LookupTable.Values.Where(p => p.Owner == Name).Sum(p => p.Farms);
        return $"{farmsCount} = {GetUnits()}/{CalculateMaxUnits()}";
    }

    /// <summary>
    ///     Returns a string that can be used in notifications instead of players name.
    ///     <example>DescribeNameAsColor("tomaszzozo") returns "Red Player" if this players index is 0</example>
    /// </summary>
    /// <param name="name">Players nickname</param>
    /// <returns>string that describes player by his color</returns>
    public static string DescribeNameAsColor(string name)
    {
        return PlayersList.FindIndex(player => player.Name == name) switch
        {
            0 => "Red Player",
            1 => "Blue Player",
            2 => "Yellow Player",
            3 => "Violet Player",
            _ => "Player not found!"
        };
    }

    public void DestroyUnitsDueToLackOfFarms()
    {
        if (GetUnits() <= CalculateMaxUnits()) return;
        var unitsLeftToDestroy = GetUnits() - CalculateMaxUnits();

        // GO THROUGH ALL FIELDS STARTING FROM ONES WITH LEAST FARMS
        foreach (var p in FieldsParameters.LookupTable.Values.Where(p =>
                     p.Owner == Name && p.AllUnits > 0).OrderBy(p => p.Farms))
        {
            if (p.AllUnits > unitsLeftToDestroy)
            {
                p.AllUnits -= unitsLeftToDestroy;
                if (p.AvailableUnits > p.AllUnits)
                    p.AvailableUnits = p.AllUnits;
                p.Instance.unitsManager.EnableAppropriateSprites(p.AllUnits, PlayersList.IndexOf(this));
                FieldInspectorManager.RaiseEventSellUnits(p.Instance.name, unitsLeftToDestroy);
                return;
            }

            unitsLeftToDestroy -= p.AllUnits;
            FieldInspectorManager.RaiseEventSellUnits(p.Instance.name, p.AllUnits);
            p.AllUnits = 0;
            p.AvailableUnits = 0;
            p.Instance.unitsManager.EnableAppropriateSprites(0, PlayersList.IndexOf(this));
        }
    }
}