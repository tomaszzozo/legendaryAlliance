using System;
using ScenesMainLoops;
using UnityEngine;
using UnityEngine.UI;

public class GameKeyListener : MonoBehaviour
{
    [SerializeField] private Canvas fieldManagerCanvas;
    [SerializeField] private Canvas attackModeCanvas;
    [SerializeField] private Button fieldManagerBackButton;
    [SerializeField] private Button nextTurnButton;
    [SerializeField] private Button attackModeCancelButton;
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (attackModeCanvas.enabled)
            {
                attackModeCancelButton.onClick.Invoke();
            }
            else if (fieldManagerCanvas.enabled)
            {
                fieldManagerBackButton.onClick.Invoke();
                SharedVariables.IsOverUi = false;
            }
            else if (EscMenuManager.Instance.IsVisible())
            {
                EscMenuManager.Instance.Hide();
            }
            else
            {
                EscMenuManager.Instance.Show();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (attackModeCanvas.enabled)
            {
                
            }
            else if (fieldManagerCanvas.enabled)
            {
                
            }
            else if (nextTurnButton.interactable)
            {
                nextTurnButton.onClick.Invoke();
            }
        }
        
    }
}
