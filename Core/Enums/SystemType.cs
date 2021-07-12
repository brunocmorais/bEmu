using System.ComponentModel;

namespace bEmu.Core.Enums
{
    public enum SystemType
    {
        [Description("Arcade / Space Invaders")]
        Generic8080 = 1,
        [Description("CHIP-8")]
        Chip8 = 2,
        [Description("GameBoy / GameBoy Color")]
        GameBoy = 3
    }
}