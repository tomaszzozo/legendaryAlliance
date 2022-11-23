using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MessageBoxFactory : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject _canvasObject;
    private string _content = "";
    private int _width, _height;

    public static void ShowAlertDialog(string content, GameObject gameObject)
    {
        gameObject.AddComponent<MessageBoxFactory>().AlertDialog(content);
    }

    private void AlertDialog(string content)
    {
        _width = (int)(Screen.width / 1.3);
        _height = (int)(Screen.height / 1.3);
        _content = content;
        SharedVariables.IsOverUi = true;

        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject es = new("EventSystem", typeof(EventSystem));
            es.AddComponent<StandaloneInputModule>();
        }

        CreateCanvas();
        CreateBackground();
        CreateTitle();
        CreateButton();
    }

    private void CreateTitle()
    {
        GameObject titleObject = new("MessageBoxTitle");
        titleObject.transform.SetParent(_canvas.transform);
        var text = titleObject.AddComponent<TextMeshProUGUI>();
        text.text = _content;
        text.fontSize = 40;
        text.alignment = TextAlignmentOptions.Midline;
        text.color = Color.white;
        text.rectTransform.anchoredPosition = Vector3.zero;
        text.rectTransform.sizeDelta = new Vector2(_width * 0.8f, _height);
        text.rectTransform.anchoredPosition = new Vector2(0, Screen.height / 4);
    }

    private void CreateButton()
    {
        float width = Screen.width / 5;
        float height = Screen.height / 8;

        GameObject buttonObject = new("MessageBoxButton");
        var buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.transform.SetParent(_canvas.transform, false);
        buttonImage.rectTransform.sizeDelta = new Vector2(width, height);
        buttonImage.rectTransform.anchoredPosition = new Vector2(0, height * -2);
        buttonImage.color = new Color(.2f, .2f, .2f, .99f);

        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(() =>
        {
            SharedVariables.IsOverUi = false;
            Destroy(_canvasObject);
        });

        GameObject textObject = new("MessageBoxButtonText");
        textObject.transform.SetParent(buttonObject.transform, false);
        var text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "OK";
        text.rectTransform.sizeDelta = Vector2.zero;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.anchoredPosition = new Vector2(.5f, .5f);
        text.fontSize = 35;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Midline;
    }

    private void CreateBackground()
    {
        GameObject backgroundObject = new("MessageBoxBackground");
        var background = backgroundObject.AddComponent<Image>();
        background.transform.SetParent(_canvas.transform, false);
        background.rectTransform.sizeDelta = new Vector2(_width, _height);
        background.rectTransform.anchoredPosition = Vector3.zero;
        background.color = new Color(0.5f, 0.5f, 0.5f, 0.99f);
    }

    private void CreateCanvas()
    {
        GameObject canvasObject = new("MessageBoxCanvas");
        _canvasObject = canvasObject;
        var canvas = canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<GraphicRaycaster>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObject.transform.position =
            new Vector3(canvasObject.transform.position.x, canvasObject.transform.position.y, 10);
        _canvas = canvas;
    }
}