using UnityEngine;

namespace fields
{
    public class UnitsManager : MonoBehaviour
    {
        [SerializeField] private SpriteColorManager tiny;
        [SerializeField] private SpriteColorManager small;
        [SerializeField] private SpriteColorManager medium;
        [SerializeField] private SpriteColorManager large;
        [SerializeField] private SpriteColorManager enormous;

        public void EnableAppropriateSprites(int unitsCount, int playerIndex)
        {
            tiny.DisableSprites();
            small.DisableSprites();
            medium.DisableSprites();
            large.DisableSprites();
            enormous.DisableSprites();
            if (unitsCount > 0) tiny.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 5) small.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 10) medium.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 15) large.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 20) enormous.EnableAppropriateSprite(playerIndex);
        }
    }
}
