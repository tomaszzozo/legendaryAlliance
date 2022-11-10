using UnityEngine;

namespace fields
{
    public class SpriteColorManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer redSprite;
        [SerializeField] private SpriteRenderer blueSprite;
        [SerializeField] private SpriteRenderer yellowSprite;
        [SerializeField] private SpriteRenderer violetSprite;

        public void EnableAppropriateSprite(int currentPlayerIndex)
        {
            switch (currentPlayerIndex)
            {
                case 0:
                    redSprite.enabled = true;
                    break;
                case 1:
                    blueSprite.enabled = true;
                    break;
                case 2:
                    yellowSprite.enabled = true;
                    break;
                case 3:
                    violetSprite.enabled = true;
                    break;
            }
        }

        public void DisableSprites()
        {
            redSprite.enabled = false;
            blueSprite.enabled = false;
            yellowSprite.enabled = false;
            violetSprite.enabled = false;
        }
    }
}
