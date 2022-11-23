using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWrapper
{
    public ButtonWrapper(GameObject buttonGameObject)
    {
        Button = buttonGameObject.GetComponent<Button>();
        Label = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public Button Button { get; }
    public TextMeshProUGUI Label { get; }
}