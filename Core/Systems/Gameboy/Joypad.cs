namespace bEmu.Core.Systems.Gameboy
{
    public class Joypad
    {
        public byte Column1 { get; set; } = 0xF;
        public byte Column2 { get; set; } = 0xF;
        byte activeColumn;

        public void SetJoypadColumn(byte column)
        {
            activeColumn = (byte) (column & 0x30);
        }

        public byte GetJoypadInfo()
        {
            if (activeColumn == 0x10)
                return (Column1);
            else if (activeColumn == 0x20)
                return (Column2);
            else
                return 0;
        }
    }
}