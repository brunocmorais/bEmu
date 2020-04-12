using System.Collections.Generic;

namespace bEmu.Core.Systems.Gameboy
{
    public class OAM
    {
        public ISystem System { get; }
        public Sprite[] Sprites = new Sprite[40];

        public OAM(ISystem system)
        {
            System = system;
        }

        public void UpdateSprites()
        {
            for (int i = 0; i < Sprites.Length; i++)
                Sprites[i] = GetSprite(i);
        }

        public IEnumerable<Sprite> GetSpritesOnLine(int ly, int spriteSize)
        {
            int counter = 0;

            foreach (var sprite in Sprites)
            {
                if (counter == 10) // limite de 10 sprites por linha
                    yield break;

                if (sprite == null || sprite.X == 0 || sprite.X >= 168 || 
                    sprite.Y == 0 || sprite.Y >= 160 || (sprite.Y <= 8 && spriteSize == 8))
                    continue;
                    
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
                System.MMU[0xFE00 + index + 1] - 8,
                System.MMU[0xFE00 + index + 0] - 16,
                System.MMU[0xFE00 + index + 2],
                System.MMU[0xFE00 + index + 3]);
        }
    }

    public class Sprite
    {
        public Sprite(int x, int y, byte address, byte attr)
        {
            X = x;
            Y = y;
            Address = address;
            Attr = attr;
        }

        public int X { get; }
        public int Y { get; }        
        public byte Address { get; }
        public byte Attr { get; }
        public int LineOffset { get; set; }
        public int Size { get; set; }
        public PaletteType PaletteType => (Attr & 0x10) == 0x10 ? PaletteType.OPB1 : PaletteType.OBP0;
        public bool Priority => (Attr & 0x80) == 0x80;
        public bool XFlip => (Attr & 0x20) == 0x20;
        public bool YFlip => (Attr & 0x40) == 0x40;
    }
}