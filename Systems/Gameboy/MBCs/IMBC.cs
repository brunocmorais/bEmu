using bEmu.Core.Mappers;

namespace bEmu.Systems.Gameboy.MBCs
{
    public interface IMBC : IMapper
    {
        void SetMode(int addr, byte value);
    }
}