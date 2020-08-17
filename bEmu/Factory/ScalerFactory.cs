using System;
using bEmu.Core;
using bEmu.Scalers;

namespace bEmu.Factory
{
    public static class ScalerFactory
    {
        public static IScaler Get(Scaler scaler, Framebuffer framebuffer)
        {
            switch (scaler)
            {
                case Scaler.None: return new DummyScaler(framebuffer);
                case Scaler.Eagle: return new EagleScaler(framebuffer);
                default: throw new ArgumentException();
            }
        }
    }
}