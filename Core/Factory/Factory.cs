using System;
using bEmu.Core.Util;

namespace bEmu.Core.Factory
{
    public abstract class Factory<T, E, C> : Singleton<T>, IFactory<E, C> 
        where T : class, new()
        where E : Enum
        where C : class
    {
        public abstract C Get(E type, params object[] parameters);
    }
}