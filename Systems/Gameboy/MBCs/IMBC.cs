using bEmu.Core.Memory;

namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IMBC : IMapper
    {
        void SetMode(int addr, byte value);
    }
}