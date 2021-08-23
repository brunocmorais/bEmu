using System.Collections.Generic;
using System.Linq;

namespace bEmu.Systems.Generic8080
{
    public class GameInfo
    {
        public GameInfoItem[] Items { get; }

        public GameInfo(IEnumerable<GameInfoItem> items)
        {
            Items = items.ToArray();
        }   
    }
}