using System.Text;

namespace bEmu.Core.Systems.Gameboy
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
            NintendoLogo = GetArray(rom, 0x0104, 0x0133);
            Title = GetString(rom, 0x0134, 0x0143);
            ManufacturerCode = GetString(rom, 0x013F, 0x0142);
            GBCFlag = rom[0x0143];
            NewLicenseeCode = GetString(rom, 0x0144, 0x0145);
            SGBFlag = rom[0x0146];
            CartridgeType = rom[0x0147];
            ROMSize = 32 << rom[0x0148];
            RAMSize = GetRAMSize(rom[0x0149]);
            Japanese = rom[0x014A] == 0;
            OldLicenseeCode = rom[0x014B];
            MaskROMVersionNumber = rom[0x014C];
            HeaderCheckSum = rom[0x014D];
            GlobalChecksum = (ushort) (rom[0x014E] << 8 | rom[0x014F]);
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