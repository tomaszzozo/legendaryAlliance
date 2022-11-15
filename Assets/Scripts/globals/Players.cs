using System.Collections.Generic;
using System.Linq;
using fields;
using UnityEngine;
public class Players
{
    public static readonly List<Players> PlayersList = new()
    {
        new Players {Name = "", Color = new Color(255/255f, 0/255f ,0/255f), Gold = 0},
        new Players {Name = "", Color = new Color(0/255f, 24/255f ,255/255f), Gold = 0},
        new Players {Name = "", Color = new Color(255/255f, 246/255f, 0/255f), Gold = 0},
        new Players {Name = "", Color = new Color(162/255f, 0/255f, 255/255f), Gold = 0}
    };

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
        }
    }

    public static int NameToIndex(string name)
    {
        return PlayersList.FindIndex(player => player.Name == name);
    }

    public string IncomeAsString()
    {
        return "(" + (Income < 0 ? "-" : "+") + Income + ")";
    }
    
    public int CalculateIncome()
    {
        return FieldsParameters.LookupTable.Values
            .Where(field => field.Owner == Name)
            .Sum(field => field.Income);
    }

    /// <summary>
    /// Returns a string that can be used in notifications instead of players name.
    /// <example>DescribeNameAsColor("tomaszzozo") returns "Red Player" if this players index is 0</example>
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

    public string Name;
    public int Gold;
    public Color Color;
    public int Income;
}
