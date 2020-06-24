namespace bEmu.Core.Systems.Gameboy.GPU
{
    public struct Background
    {
        public PaletteType BackgroundPaletteNumber { get; }
        public int TileVRAMBankNumber { get; }
        public bool HorizontalFlip { get; }
        public bool VerticalFlip { get; }
        public bool BGToOAMPriority { get; }

        public Background(PaletteType backgroundPaletteNumber, int tileVRAMBankNumber, bool horizontalFlip, bool verticalFlip, bool bGToOAMPriority)
        {
            BackgroundPaletteNumber = backgroundPaletteNumber;
            TileVRAMBankNumber = tileVRAMBankNumber;
            HorizontalFlip = horizontalFlip;
            VerticalFlip = verticalFlip;
            BGToOAMPriority = bGToOAMPriority;
        }
    }
}