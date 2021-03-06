namespace bEmu.Core.System
{
    public interface IState : ISaveable
    {
        IRunnableSystem System { get; }
        int Cycles { get; set; }
        bool Halted { get; set; }
        ulong Instructions { get; set; }
        void Reset();
    }
}