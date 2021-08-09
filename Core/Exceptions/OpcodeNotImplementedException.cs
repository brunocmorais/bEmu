using System;
using bEmu.Core.CPU;

namespace bEmu.Core.Exceptions
{
    public class OpcodeNotImplementedException : Exception
    {
        const string message = "Opcode {0:x} não implementado!";
        public OpcodeNotImplementedException(IOpcode opcode) : base(string.Format(message, opcode)) { }
    }
}