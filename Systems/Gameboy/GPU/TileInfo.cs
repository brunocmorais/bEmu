namespace bEmu.Systems.Gameboy.GPU
{
    public struct TileInfo
    {
        public PaletteType BackgroundPaletteNumber { get; }
        public int TileVRAMBankNumber { get; }
        public bool HorizontalFlip { get; }
        public bool VerticalFlip { get; }
        public bool BGToOAMPriority { get; }

        public TileInfo(PaletteType backgroundPaletteNumber, int tileVRAMBankNumber, bool horizontalFlip, bool verticalFlip, bool bGToOAMPriority)
        {
            BackgroundPaletteNumber = backgroundPaletteNumber;
            TileVRAMBankNumber = tileVRAMBankNumber;
            HorizontalFlip = horizontalFlip;
            VerticalFlip = verticalFlip;
            BGToOAMPriority = bGToOAMPriority;
        }
    }
}