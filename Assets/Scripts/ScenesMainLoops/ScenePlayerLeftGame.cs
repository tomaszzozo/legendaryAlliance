using TMPro;
using UnityEngine;

public class ScenePlayerLeftGame : MonoBehaviour
{
    public static string PlayerThatLeftNickname;
    [SerializeField] private TextMeshProUGUI label;

    private void Start()
    {
        label.text =
            $"{PlayerThatLeftNickname} ({Players.DescribeNameAsColor(PlayerThatLeftNickname)}) has left the game!";
    }
}