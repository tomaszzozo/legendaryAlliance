using TMPro;
using UnityEngine;

public class ScenePlayerLeftGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    
    public static string PlayerThatLeftNickname;
    private void Start()
    {
        label.text = $"{PlayerThatLeftNickname} ({Players.DescribeNameAsColor(PlayerThatLeftNickname)})has left the game!";
    }
}
