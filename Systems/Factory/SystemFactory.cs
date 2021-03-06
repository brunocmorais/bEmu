using System;
using bEmu.Core.Enums;
using bEmu.Core.Factory;
using bEmu.Core.IO;
using bEmu.Core.System;

namespace bEmu.Systems.Factory
{
    public class SystemFactory : Factory<SystemFactory, SystemType, IRunnableSystem>
    {
        public override IRunnableSystem Get(SystemType type, params object[] parameters)
        {
            var rom = ROMReaderFactory.Instance.Get(type).Read(FileManager.Read(parameters[0] as string));

            switch (type)
            {
                case SystemType.Chip8:
                    return new Chip8.System(rom);
                case SystemType.GameBoy:
                    return new Gameboy.System(rom);
                case SystemType.Generic8080:
                    return new Generic8080.System(rom);
                case SystemType.GameBoySoundSystem:
                    return new GBS.System(rom);
                default:
                    throw new Exception("Sistema não suportado.");
            }
        }

        public ISystem GetEmptySystem() => new EmptySystem();
    }
}