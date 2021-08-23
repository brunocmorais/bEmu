using System;
using System.IO;
using System.Linq;
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

            return new CartridgeHeader(bytes);
        }
    }
}