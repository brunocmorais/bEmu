namespace bEmu.Systems.Gameboy.GPU
{
    public class ColorPaletteData : IPaletteData
    {
        public byte[] BackgroundPalettes { get; }
        public byte[] SpritePalettes { get; }
        private byte backgroundPaletteIndex;
        private byte spritePaletteIndex;

        public ColorPaletteData()
        {
            BackgroundPalettes = new byte[64];
            for (int i = 0; i < BackgroundPalettes.Length; i++)
                BackgroundPalettes[i] = 0xFF;
            SpritePalettes = new byte[64];
            backgroundPaletteIndex = 0;
            spritePaletteIndex = 0;
        }

        private byte BCPS
        {
            get { return backgroundPaletteIndex; }
            set { backgroundPaletteIndex = value; }
        }

        private byte BCPD
        {
            get { return BackgroundPalettes[BCPS & 0x3F]; }
            set
            {
                BackgroundPalettes[BCPS & 0x3F] = value;

                if ((BCPS & 0x80) == 0x80)
                    BCPS = (byte) (0x80 | ((BCPS & 0x3F) + 1));
            }
        }

        private byte OCPS
        {
            get { return spritePaletteIndex; }
            set { spritePaletteIndex = value; }
        }

        private byte OCPD
        {
            get { return SpritePalettes[OCPS & 0x3F]; }
            set
            {
                SpritePalettes[OCPS & 0x3F] = value;

                if ((OCPS & 0x80) == 0x80)
                    OCPS = (byte) (0x80 | ((OCPS + 1) & 0x3F));
            }
        }

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0xFF68: return BCPS;
                    case 0xFF69: return BCPD;
                    case 0xFF6A: return OCPS;
                    case 0xFF6B: return OCPD;
                    default: return 0;
                }
            }
            set
            {
                switch (index)
                {
                    case 0xFF68: BCPS = value; break;
                    case 0xFF69: BCPD = value; break;
                    case 0xFF6A: OCPS = value; break;
                    case 0xFF6B: OCPD = value; break;
                }
            }
        }
    }
}