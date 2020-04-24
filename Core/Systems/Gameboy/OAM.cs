using System.Collections.Generic;
using System.Linq;

namespace bEmu.Core.Systems.Gameboy
{
    public class OAM
    {
        public MMU MMU { get; }
        public Sprite[] Sprites { get; }

        public OAM(MMU mmu)
        {
            MMU = mmu;
            Sprites = new Sprite[40];
        }

        public void UpdateSprites()
        {
            for (int i = 0; i < Sprites.Length; i++)
                Sprites[i] = GetSprite(i);
        }

        public IEnumerable<Sprite> GetSpritesForScanline(int ly, int spriteSize)
        {
            int counter = 0;

            foreach (var sprite in Sprites.Where(x => x != null).OrderByDescending(sprite => sprite.X))
            {
                if (counter == 10) // limite de 10 sprites por linha
                    yield break;

                // if (sprite.X <= -8 || sprite.X >= 168 || sprite.Y <= -8 || sprite.Y >= 144)
                //     continue;
                    
                int lineOffset = ly - sprite.Y;

                if (sprite.YFlip)
                    lineOffset = spriteSize - lineOffset - 1;

                if (lineOffset >= 0 && lineOffset < spriteSize)
                {
                    sprite.LineOffset = lineOffset;
                    sprite.Size = spriteSize;
                    counter++;
                    yield return sprite;
                }
            }
        }

        private Sprite GetSprite(int index)
        {
            index *= 4;

            return new Sprite(
                MMU.OAM[index + 1] - 8,
                MMU.OAM[index + 0] - 16,
                MMU.OAM[index + 2],
                MMU.OAM[index + 3]);
        }
    }
}