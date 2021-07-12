namespace bEmu.Core.System
{
    public interface IState : ISaveable
    {
        ISystem System { get; }
        ushort PC { get; set; }
        ushort SP { get; set; }
        int Cycles { get; set; }
        bool Halted { get; set; }
        ulong Instructions { get; set; }
        void Reset();
    }
}