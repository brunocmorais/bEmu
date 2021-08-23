using System.Text;

namespace bEmu.Systems.Gameboy
{
    public class CartridgeHeader
    {
        public byte[] NintendoLogo { get; }
        public string Title { get; }
        public string ManufacturerCode { get; }
        public byte GBCFlag { get; }
        public string NewLicenseeCode { get; }
        public byte SGBFlag { get; }
        public byte CartridgeType { get; }
        public int ROMSize { get; }
        public int RAMSize { get; }
        public bool Japanese { get; }
        public byte OldLicenseeCode { get; }
        public byte MaskROMVersionNumber { get; }
        public byte HeaderCheckSum { get; }
        public ushort GlobalChecksum { get; }

        public CartridgeHeader(byte[] rom)
        {
            NintendoLogo = GetArray(rom, 0x004, 0x033);
            Title = GetString(rom, 0x034, 0x043);
            ManufacturerCode = GetString(rom, 0x03F, 0x042);
            GBCFlag = rom[0x043];
            NewLicenseeCode = GetString(rom, 0x044, 0x045);
            SGBFlag = rom[0x046];
            CartridgeType = rom[0x047];
            ROMSize = 32 << rom[0x048];
            RAMSize = GetRAMSize(rom[0x049]);
            Japanese = rom[0x04A] == 0;
            OldLicenseeCode = rom[0x04B];
            MaskROMVersionNumber = rom[0x04C];
            HeaderCheckSum = rom[0x04D];
            GlobalChecksum = (ushort) (rom[0x04E] << 8 | rom[0x04F]);
        }

        private byte[] GetArray(byte[] rom, ushort start, ushort end)
        {
            byte[] array = new byte[end - start + 1];

            for (int i = 0; i < array.Length; i++)
                array[i] = rom[i + start];

            return array;
        }

        private string GetString(byte[] header, ushort start, ushort end)
        {
            var sb = new StringBuilder();
            
            for (int i = start; i < end; i++)
            {
                if (header[i] == 0)
                    break;

                sb.Append((char) header[i]);
            }

            return sb.ToString();
        }

        private int GetRAMSize(byte value)
        {
            switch (value)
            {
                case 0: return 0;
                case 1: return 2;
                case 2: return 8;
                case 3: return 32;
                case 4: return 128;
                case 5: return 64;
                default: return 0;
            }
        }
    }
}