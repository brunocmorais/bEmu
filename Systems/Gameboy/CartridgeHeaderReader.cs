using System;
using System.IO;
using System.Linq;
using System.Text;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.Memory;
using bEmu.Core.Util;

namespace bEmu.Systems.Gameboy
{
    public class CartridgeHeaderReader : Singleton<CartridgeHeaderReader>, IReader<CartridgeHeader>
    {
        public CartridgeHeader Read(FileStream stream)
        {
            var bytes = new byte[0x50];

            stream.Position = 0x100;

            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)stream.ReadByte();

            stream.Position = 0;

            var nintendoLogo = bytes.GetArray(0x004, 0x033);
            var title = bytes.GetString(0x034, 0x043);
            var manufacturerCode = bytes.GetString(0x03F, 0x042);
            var gbcFlag = bytes[0x043];
            var newLicenseeCode = bytes.GetString(0x044, 0x045);
            var sgbFlag = bytes[0x046];
            var cartridgeType = bytes[0x047];
            var romSize = 32 << bytes[0x048];
            var ramSize = GetRAMSize(bytes[0x049]);
            var japanese = bytes[0x04A] == 0;
            var oldLicenseeCode = bytes[0x04B];
            var maskROMVersionNumber = bytes[0x04C];
            var headerCheckSum = bytes[0x04D];
            var globalChecksum = (ushort) ((bytes[0x04E] << 8) | bytes[0x04F]);

            return new CartridgeHeader(nintendoLogo, title, manufacturerCode, 
                gbcFlag, newLicenseeCode, sgbFlag, cartridgeType, romSize, 
                ramSize, japanese, oldLicenseeCode, maskROMVersionNumber, 
                headerCheckSum, globalChecksum);
        }

        private static int GetRAMSize(byte value)
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