using System;
using System.IO;
using bEmu.Core.CPU;
using bEmu.Core.Enums;
using bEmu.Core.Memory;
using bEmu.Core.System;
using bEmu.Core.Util;

namespace bEmu.Core.Factory
{

    public class DisassemblerFactory : Factory<DisassemblerFactory, SystemType, IDisassembler>
    {
        public override IDisassembler Get(SystemType type, params object[] parameters)
        {
            var mmu = parameters[0] as MMU;

            switch (type)
            {
                case SystemType.Generic8080: 
                    return new Core.CPU.Intel8080.Disassembler(mmu);
                case SystemType.GameBoy: 
                    return new Core.CPU.LR35902.Disassembler(mmu);
                default:
                    throw new Exception("Disassembler não disponível para a CPU");
            }
        }
    }
}