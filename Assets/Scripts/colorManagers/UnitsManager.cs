using UnityEngine;
using UnityEngine.Serialization;

namespace fields
{
    public class UnitsManager : MonoBehaviour
    {
        [FormerlySerializedAs("tiny")] [SerializeField]
        private SpriteColorManager few;

        [FormerlySerializedAs("small")] [SerializeField]
        private SpriteColorManager several;

        [FormerlySerializedAs("medium")] [SerializeField]
        private SpriteColorManager pack;

        [FormerlySerializedAs("large")] [SerializeField]
        private SpriteColorManager lots;

        [FormerlySerializedAs("enormous")] [SerializeField]
        private SpriteColorManager horde;

        [SerializeField] private SpriteColorManager throng;
        [SerializeField] private SpriteColorManager swarm;
        [SerializeField] private SpriteColorManager zounds;
        [SerializeField] private SpriteColorManager legion;

        public void EnableAppropriateSprites(int unitsCount, int playerIndex)
        {
            few.DisableSprites();
            several.DisableSprites();
            pack.DisableSprites();
            lots.DisableSprites();
            horde.DisableSprites();
            throng.DisableSprites();
            swarm.DisableSprites();
            zounds.DisableSprites();
            legion.DisableSprites();

            if (unitsCount > 0) few.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 4) several.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 9) pack.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 14) lots.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 19) horde.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 29) throng.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 39) swarm.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 49) zounds.EnableAppropriateSprite(playerIndex);
            if (unitsCount > 59) legion.EnableAppropriateSprite(playerIndex);
        }
    }
}