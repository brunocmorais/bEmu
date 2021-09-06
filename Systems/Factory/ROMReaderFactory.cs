using System;
using bEmu.Core.Enums;
using bEmu.Core.Factory;
using bEmu.Core.IO;
using bEmu.Core.Memory;
using bEmu.Systems.Generic8080;

namespace bEmu.Systems.Factory
{
    public class ROMReaderFactory : Factory<ROMReaderFactory, SystemType, IReader<IROM>>
    {
        public override IReader<IROM> Get(SystemType type, params object[] parameters)
        {
            switch (type)
            {
                case SystemType.Generic8080: 
                    return Generic8080.ROMReader.Instance;
                case SystemType.Chip8:
                    return Core.Memory.ROMReader.Instance;
                case SystemType.GameBoy:
                    return Gameboy.ROMReader.Instance;
                case SystemType.GameBoySoundSystem:
                    return GBS.ROMReader.Instance;
                default:
                    throw new Exception("Sistema não suportado!");
            }
        }
    }
}