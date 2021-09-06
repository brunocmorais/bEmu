using System.IO;
using bEmu.Core.Extensions;
using bEmu.Core.IO;
using bEmu.Core.Util;

namespace bEmu.Systems.GBS
{
    public class GBSHeaderReader : Singleton<GBSHeaderReader>, IReader<GBSHeader>
    {
        public GBSHeader Read(FileStream stream)
        {
            var bytes = new byte[0x70];

            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)stream.ReadByte();

            var identifier = bytes.GetString(0x0, 0x2);
            var version = bytes[0x3];
            var numberOfSongs = bytes[0x4];
            var firstSong = bytes[0x5];
            var loadAddress = (ushort)((bytes[0x7] << 8) | bytes[0x6]);
            var initAddress = (ushort)((bytes[0x9] << 8) | bytes[0x8]);
            var playAddress = (ushort)((bytes[0xB] << 8) | bytes[0xA]);
            var sp = (ushort)((bytes[0xD] << 8) | bytes[0xC]);
            var timerModulo = bytes[0xE];
            var timerControl = bytes[0xF];
            var title = bytes.GetString(0x10, 0x2F);
            var author = bytes.GetString(0x30, 0x4F);
            var copyright = bytes.GetString(0x50, 0x6F);

            return new GBSHeader(identifier, version, numberOfSongs, firstSong, 
                loadAddress, initAddress, playAddress, sp, timerModulo, 
                timerControl, title, author, copyright);
        }
    }
}