using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopStatsManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI labelCoins;
    [SerializeField] private TextMeshProUGUI labelUsername;
    [SerializeField] private TextMeshProUGUI labelIncome;
    [SerializeField] private Image decorationBar;
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
    }
}
