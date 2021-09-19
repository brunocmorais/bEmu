namespace bEmu.Systems.NES
{
    public interface IMapper
    {
        void LoadProgram(byte[] bytes);
        byte Read(ushort address);
    }
}