using System.ComponentModel;
using bEmu.Core.Attributes;
using bEmu.Core.Util;

namespace bEmu.Core.Enums
{
    public enum SystemType
    {
        [Ignore]
        None = 0,
        [Description("Arcade / Space Invaders")]
        Generic8080 = 1,
        [Description("CHIP-8")]
        Chip8 = 2,
        [Description("GameBoy / GameBoy Color")]
        GameBoy = 3
    }
}