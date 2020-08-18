namespace bEmu.Systems.Gameboy
{
    public class Joypad
    {
        private byte activeColumn;
        public byte Column1 { get; set; }
        public byte Column2 { get; set; }

        public Joypad()
        {
            Column1 = 0xF;
            Column2 = 0xF;
        }

        public void SetJoypadColumn(byte column)
        {
            activeColumn = (byte) (column & 0x30);
        }

        public byte GetJoypadInfo()
        {
            if (activeColumn == 0x10)
                return Column1;
            if (activeColumn == 0x20)
                return Column2;
            
            return 0;
        }
    }
}