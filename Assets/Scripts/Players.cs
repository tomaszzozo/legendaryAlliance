using System.Collections.Generic;
using UnityEngine;
public class Players
{
    public static readonly List<Players> PlayersList = new()
    {
        new Players {Name = "", Color = new Color(255, 0 ,0), Gold = 0},
        new Players {Name = "", Color = new Color(0, 24 ,255), Gold = 0},
        new Players {Name = "", Color = new Color(255, 246, 0), Gold = 0},
        new Players {Name = "", Color = new Color(162, 0, 255), Gold = 0}
    };

    public static void FillPlayerNames()
    {
        PlayersList[0].Name = SharedVariables.SharedData[0].ToString();
        PlayersList[1].Name = SharedVariables.SharedData[1].ToString();
        PlayersList[2].Name = SharedVariables.SharedData[2].ToString();
        PlayersList[3].Name = SharedVariables.SharedData[3].ToString();
    }

    public static void Init(int gold, int baseIncome)
    {
        foreach (var player in PlayersList)
        {
            player.Gold = gold;
            player.Income = baseIncome;
        }
        PlayersList[0].Gold += baseIncome;
    }

    public string IncomeAsString()
    {
        return "(" + (Income < 0 ? "-" : "+") + Income + ")";
    }

    public string Name;
    public int Gold;
    public Color Color;
    public int Income;
}
