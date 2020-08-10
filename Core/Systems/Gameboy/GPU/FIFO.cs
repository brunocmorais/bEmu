using System;
using System.Collections.Generic;

namespace bEmu.Core.Systems.Gameboy.GPU
{
    public class FIFO
    {
        public Queue<Pixel> Pixels { get; }
        private FIFOStep step;
        private readonly LCD lcd;

        public FIFO(LCD lcd)
        {
            Pixels = new Queue<Pixel>(16);
            step = FIFOStep.GetTile;
            this.lcd = lcd;
        }

        public void Fetch()
        {
            switch (step)
            {
                case FIFOStep.GetTile: GetTile(); break;
                case FIFOStep.GetTileDataLow: GetTileDataLow(); break;
                case FIFOStep.GetTileDataHigh: GetTileDataHigh(); break;
                case FIFOStep.Sleep: Sleep(); break;
                case FIFOStep.Push: Push(); break;
            }
        }

        private void Push()
        {
            step = FIFOStep.GetTile;
        }

        private void Sleep()
        {
            step = FIFOStep.Push;
        }

        private void GetTileDataHigh()
        {
            step = FIFOStep.Sleep;
        }

        private void GetTileDataLow()
        {
            step = FIFOStep.GetTileDataHigh;
        }

        private void GetTile()
        {
            int tileMap;
            
            if (lcd.GetLCDCFlag(LCDC.BGTileMapDisplaySelect) && lcd.LY < lcd.WY)
                tileMap = 0x1C00;
            else if (lcd.GetLCDCFlag(LCDC.WindowTileMapDisplaySelect) && lcd.LY >= lcd.WY)
                tileMap = 0x1C00;
            else
                tileMap = 0x1800;

            step = FIFOStep.GetTileDataLow;
        }
    }

    public enum FIFOStep
    {
        GetTile,
        GetTileDataLow,
        GetTileDataHigh,
        Sleep,
        Push
    }
}