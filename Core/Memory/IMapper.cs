namespace bEmu.Core.Memory
{
    public interface IMapper
    {
        void LoadProgram(byte[] bytes);
        
        byte ReadROM(int addr);
    }
}