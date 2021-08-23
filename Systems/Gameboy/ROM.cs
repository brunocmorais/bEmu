using System;
using System.IO;
using bEmu.Core.IO;

namespace bEmu.Systems.Gameboy
{
    public class ROM : Core.Memory.ROM
    {
        public CartridgeHeader CartridgeHeader { get; }

        public ROM(FileStream fileStream, CartridgeHeader cartridgeHeader) : base(fileStream)
        {
            CartridgeHeader = cartridgeHeader;
        }
    }
}