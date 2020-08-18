using System;
using bEmu.Core;
using bEmu.Scalers;

namespace bEmu.Factory
{
    public static class ScalerFactory
    {
        public static IScaler Get(Scaler scaler, int pixelSize)
        {
            switch (scaler)
            {
                case Scaler.None: return new DummyScaler();
                case Scaler.Eagle: return new EagleScaler();
                case Scaler.EPX: return new EPXScaler();
                case Scaler.NearestNeighbor: return new NearestNeighborScaler(pixelSize);
                case Scaler.Scale2x: return new Scale2xScaler();
                case Scaler.Scale3x: return new Scale3xScaler();
                case Scaler.Bilinear: return new BilinearScaler(pixelSize);
                default: throw new ArgumentException();
            }
        }
    }
}