using System;
using bEmu.Systems;

namespace bEmu.Systems.Exceptions
{
    public class SystemNotSupportedException : Exception
    {
        public SystemNotSupportedException() : base("Sistemas suportados: \n\n" + GetSupportedSystems()) { }

        private static string GetSupportedSystems()
        {
            return string.Join("\n", Enum.GetNames(typeof(SupportedSystems)));
        }
    }
}