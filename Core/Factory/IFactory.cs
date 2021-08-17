using System;

namespace bEmu.Core.Factory
{
    public interface IFactory<E, C> where E : Enum where C : class
    {
        C Get(E type, params object[] parameters);
    }
}