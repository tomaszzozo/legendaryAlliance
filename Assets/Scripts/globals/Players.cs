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
    public int Gold;
    public int Income;
    public int LabsLimitLevel;
    public string Name;
    public int SciencePoints;
    public int TrenchesLimitLevel;
    public bool Conquered;

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
        return incomeSum + capitalBonus - trenchesPenalty - labsPenalty;
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
}