using bEmu.Core.Util;

namespace bEmu.Core.CPU
{
    public interface IOpcode
    {
        int CyclesTaken { get; set; }
    }
}