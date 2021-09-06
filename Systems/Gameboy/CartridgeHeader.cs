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

        public CartridgeHeader(byte[] nintendoLogo, string title, string manufacturerCode, 
            byte gBCFlag, string newLicenseeCode, byte sGBFlag, byte cartridgeType, 
            int rOMSize, int rAMSize, bool japanese, byte oldLicenseeCode, byte maskROMVersionNumber, 
            byte headerCheckSum, ushort globalChecksum)
        {
            NintendoLogo = nintendoLogo;
            Title = title;
            ManufacturerCode = manufacturerCode;
            GBCFlag = gBCFlag;
            NewLicenseeCode = newLicenseeCode;
            SGBFlag = sGBFlag;
            CartridgeType = cartridgeType;
            ROMSize = rOMSize;
            RAMSize = rAMSize;
            Japanese = japanese;
            OldLicenseeCode = oldLicenseeCode;
            MaskROMVersionNumber = maskROMVersionNumber;
            HeaderCheckSum = headerCheckSum;
            GlobalChecksum = globalChecksum;
        }
    }
}