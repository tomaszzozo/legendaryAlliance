using UnityEngine;
using UnityEngine.UI;

public class ImageColorManager : MonoBehaviour
{
    public Image redImage;
    public Image blueImage;
    public Image yellowImage;
    public Image violetImage;

    public void EnableAppropriateImage(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                redImage.enabled = true;
                break;
            case 1:
                blueImage.enabled = true;
                break;
            case 2:
                yellowImage.enabled = true;
                break;
            case 3:
                violetImage.enabled = true;
                break;
            default:
                DisableImages();
                break;
        }
    }

    public void DisableImages()
    {
        redImage.enabled = false;
        blueImage.enabled = false;
        yellowImage.enabled = false;
        violetImage.enabled = false;
    }
}