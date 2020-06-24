using System.Collections.Generic;
using System.Linq;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class OAM
    {
        public MMU MMU { get; }
        private readonly byte[] oam;

        public byte this[int index]
        {
            get { return oam[index]; }
            set { oam[index] = value; }
        }

        public OAM(MMU mmu)
        {
            MMU = mmu;
            oam = new byte[160];
        }

        public IEnumerable<Sprite> GetSpritesForScanline(int ly, int spriteSize)
        {
            int counter = 0;
            var sprites = Enumerable.Range(0, 40).Select(x => GetSprite(x)).OrderByDescending(sprite => sprite.X);

            foreach (var sprite in sprites.Where(x => x != null).OrderByDescending(sprite => sprite.X))
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
                this[index + 1] - 8,
                this[index + 0] - 16,
                this[index + 2],
                this[index + 3]);
        }

        public void StartDMATransfer(byte value)
        {
            var oamStartAddress = (value << 8);

            for (int i = 0; i < 0x9F; i++)
                this[i] = MMU[oamStartAddress + i];
        }
    }
}