using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyCapturer : MonoBehaviour
{
    public Button buttonOnEnter;
    public TMP_InputField input1;
    public TMP_InputField input2;
    public TMP_InputField input3;
    private int _inputsCount;

    private void Start()
    {
        _inputsCount = input3 == null ? 2 : 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            buttonOnEnter.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (input1.isFocused)
            {
                input2.Select();
                input2.ActivateInputField();
            }
            else if (input2.isFocused)
            {
                if (_inputsCount == 2)
                {
                    input1.Select();
                    input1.ActivateInputField();
                }
                else
                {
                    input3.Select();
                    input3.ActivateInputField();
                }
            }
            else if (_inputsCount == 3 && input3.isFocused)
            {
                input1.Select();
                input1.ActivateInputField();
            }
        }
    }
}