namespace bEmu.Systems.Gameboy.GPU
{
    public class BackgroundMap
    {
        private readonly Tile[,] tiles;
        public MMU MMU { get; }
        public int BackgroundMapSelect { get; set; }
        public int WindowMapSelect { get; set; }
        public int TileStartAddress { get; set; }
        public bool Window { get; set; }

        public BackgroundMap(MMU mmu)
        {
            MMU = mmu;

            this.tiles = new Tile[32, 32];
            
            for (int i = 0; i < tiles.GetLength(0); i++)
                for (int j = 0; j < tiles.GetLength(1); j++)
                    tiles[i, j] = new Tile(this, i, j);
        }

        public int GetCoordinateFromPadding(int padding)
        {
            return padding / 8;
        }

        public Tile this[int x, int y] => tiles[x, y];
    }

    public class Tile
    {
        private readonly BackgroundMap backgroundMap;
        private int Map => backgroundMap.Window ? backgroundMap.WindowMapSelect : backgroundMap.BackgroundMapSelect;
        private VRAM VRAM => backgroundMap.MMU.VRAM;
        public int X { get; }
        public int Y { get; }
        public int TileNumber => VRAM[MapAddress];
        public ushort MapAddress => (ushort)(Map + X + (Y * 0x20));
        public ushort TileAddress
        {
            get
            {
                if (backgroundMap.TileStartAddress == 0) // signed
                    return (ushort)(backgroundMap.TileStartAddress + (TileNumber << 4));
                else // unsigned
                    return (ushort)(((TileNumber & 0x80) == 0x80 ? 0x800 : 0x1000) + ((TileNumber & 0x7F) << 4));
            }   
        }
        public byte Attribute => VRAM.Bank1[MapAddress];
        public PaletteType BackgroundPaletteNumber => (PaletteType)((Attribute & 0x7) + 3);
        public int TileVRAMBankNumber => (Attribute & 0x8) == 0x8 ? 1 : 0;
        public bool HorizontalFlip => (Attribute & 0x20) == 0x20;
        public bool VerticalFlip => (Attribute & 0x40) == 0x40;
        public bool BGToOAMPriority => (Attribute & 0x80) == 0x80;

        public Tile(BackgroundMap backgroundMap, int x, int y)
        {
            this.backgroundMap = backgroundMap;
            X = x;
            Y = y;
        }
    }
}