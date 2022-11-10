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

        public void EnableAppropriateSprites(int currentPlayerIndex, int unitsCount)
        {
            tiny.DisableSprites();
            small.DisableSprites();
            medium.DisableSprites();
            large.DisableSprites();
            enormous.DisableSprites();
            if (unitsCount > 0) tiny.EnableAppropriateSprite(currentPlayerIndex);
            if (unitsCount > 5) small.EnableAppropriateSprite(currentPlayerIndex);
            if (unitsCount > 10) medium.EnableAppropriateSprite(currentPlayerIndex);
            if (unitsCount > 15) large.EnableAppropriateSprite(currentPlayerIndex);
            if (unitsCount > 20) enormous.EnableAppropriateSprite(currentPlayerIndex);
        }
    }
}
