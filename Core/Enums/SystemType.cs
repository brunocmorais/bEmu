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
        [Description("CHIP-8 / Super-Chip 8")]
        Chip8 = 2,
        [Description("GameBoy (GB) / GameBoy Color (GBC)")]
        GameBoy = 3,
        [Description("Nintendo Entertainment System (NES)")]
        NES = 4,
        [Description("GameBoy Sound System")]
        GameBoySoundSystem = 5
    }
}