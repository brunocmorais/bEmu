using System;
using bEmu.Core;
using bEmu.Core.Scalers;

namespace bEmu.Core.Factory
{
    public static class ScalerFactory
    {
        public static IScaler Get(Scaler scaler, int pixelSize)
        {
            switch (scaler)
            {
                case Scaler.None: return new EmptyScaler();
                case Scaler.Eagle: return new EagleScaler();
                case Scaler.EPX: return new EPXScaler();
                case Scaler.NearestNeighbor: return new NearestNeighborScaler(pixelSize);
                case Scaler.Scale2x: return new Scale2xScaler();
                case Scaler.Scale3x: return new Scale3xScaler();
                case Scaler.Bilinear: return new BilinearScaler(pixelSize);
                case Scaler.SuperEagle: return new SuperEagleScaler();
                case Scaler._2xSaI: return new _2xSaIScaler();
                case Scaler.Super2xSaI: return new Super2xSaIScaler();
                default: throw new ArgumentException();
            }
        }
    }
}