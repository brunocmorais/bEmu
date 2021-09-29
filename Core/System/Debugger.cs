using System;
using System.Linq;
using System.Reflection;
using bEmu.Core.CPU;
using bEmu.Core.Util;

namespace bEmu.Core.System
{

    public class Debugger : IDebugger
    {
        public IRunnableSystem System { get; }
        public int BreakpointAddress { get; set; }
        public int AccessBreakpointAddress { get; set; }
        public bool IsStopped { get; set; }

        public Debugger(IRunnableSystem system)
        {
            System = system;
        }

        public void StepInstruction()
        {
            System.Runner.StepCycle();
        }

        public string PrintRegisters()
        {
            return System.State.ToString();
        }

        public byte GetByteFromMemoryAddress(int address)
        {
            return System.MMU[address];
        }

        public ushort GetWordFromMemoryAddress(int address)
        {
            byte a = System.MMU[address];
            byte b = System.MMU[address + 1];
            return BitFacade.GetWordFrom2Bytes(a, b);
        }

        public void SetByteToMemoryAddress(int address, byte value)
        {
            System.MMU[address] = value;
        }

        public void SetWordToMemoryAddress(int address, ushort word)
        {
            BitFacade.Get2BytesFromWord(word, out byte a, out byte b);
            System.MMU[address] = b;
            System.MMU[address + 1] = a;
        }

        public string GetRegisterValue(string registerName)
        {
            PropertyInfo[] properties = GetRegisters();

            var property = properties.FirstOrDefault(x => x.Name.Equals(registerName, StringComparison.InvariantCultureIgnoreCase));

            if (property == null)
                return string.Empty;

            Type[] numericTypes = {
                typeof(int), typeof(uint), typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            };

            if (numericTypes.Contains(property.PropertyType))
                return Convert.ToInt32(property.GetValue(System.State)).ToString("x");
            else
                return property.GetValue(System.State).ToString();
        }

        public PropertyInfo[] GetRegisters()
        {
            var type = System.State.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType.IsValueType).ToArray();
            return properties;
        }
    }
}