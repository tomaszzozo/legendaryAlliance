using ScenesMainLoops;
using TMPro;
using UnityEngine;

public class SelectCapitalLabel : MonoBehaviour
{
    [SerializeField] private SceneGame mainLoop;
    [SerializeField] private TextMeshProUGUI thisLabel;

    private void Start()
    {
        thisLabel.enabled = false;
    }

    private void Update()
    {
        if (SceneGame.RoundCounter == 0)
        {
            thisLabel.enabled = mainLoop.IsItMyTurn();
        }
        else
        {
            Destroy(thisLabel);
            Destroy(this);
        }
    }
}