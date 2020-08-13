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
}