using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopStatsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelUsername;
    [SerializeField] private Image decorationBar;

    // COINS
    [SerializeField] private TextMeshProUGUI labelCoins;
    [SerializeField] private TextMeshProUGUI labelIncome;

    // SCIENCE
    [SerializeField] private TextMeshProUGUI labelSciencePoints;
    [SerializeField] private TextMeshProUGUI labelScienceIncome;

    private Players _player;

    public void Init(Players player)
    {
        _player = player;
        decorationBar.color = _player.Color;
        RefreshValues();
    }

    public void RefreshValues()
    {
        labelCoins.text = _player.Gold.ToString();
        labelUsername.text = _player.Name;
        labelIncome.text = _player.IncomeAsString();
        labelScienceIncome.text = _player.ScienceIncomeAsString();
        labelSciencePoints.text = _player.SciencePoints.ToString();
    }
}