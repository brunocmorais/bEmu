using System;
using System.IO;
using bEmu.Core.IO;

namespace bEmu.Systems.GBS
{
    public class ROM : Core.Memory.ROM
    {
        public GBSHeader GBSHeader { get; }

        public ROM(FileStream fileStream, GBSHeader gbsHeader) : base(fileStream)
        {
            GBSHeader = gbsHeader;
        }
    }
}