namespace bEmu.Core.Systems.Gameboy.GPU
{
    public enum LCDC
    {
        BGDisplayEnable = 0x1,
        SpriteDisplayEnable = 0x2,
        SpriteSize = 0x4,
        BGTileMapDisplaySelect = 0x8,
        BGWindowTileDataSelect = 0x10,
        WindowDisplayEnable = 0x20,
        WindowTileMapDisplaySelect = 0x40,
        LCDDisplayEnable = 0x80
    }
}