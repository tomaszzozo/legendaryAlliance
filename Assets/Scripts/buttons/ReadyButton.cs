using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite readyHoverSprite;
    [SerializeField] private Sprite readyPressedSprite;
    [SerializeField] private Sprite notReadySprite;
    [SerializeField] private Sprite notReadyHoverSprite;
    [SerializeField] private Sprite notReadyPressedSprite;
    
    private ButtonWrapper _buttonWrapper;

    public void ChangeSprites(bool isReady)
    {
        _buttonWrapper.Image.sprite = !isReady ? readySprite : notReadySprite;
        _buttonWrapper.Button.spriteState = new SpriteState
        {
            pressedSprite = !isReady ? readyPressedSprite : notReadyPressedSprite,
            highlightedSprite = !isReady ? readyHoverSprite : notReadyHoverSprite
        };
    }

    private void Start()
    {
        _buttonWrapper = new ButtonWrapper(gameObject);
    }
}
