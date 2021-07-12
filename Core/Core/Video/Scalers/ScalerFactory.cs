using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Video.Scalers
{
    public static class ScalerFactory
    {
        public static IScaler Get(ScalerType scaler, int pixelSize)
        {
            switch (scaler)
            {
                case ScalerType.None: return new EmptyScaler();
                case ScalerType.Eagle: return new EagleScaler();
                case ScalerType.EPX: return new EPXScaler();
                case ScalerType.NearestNeighbor: return new NearestNeighborScaler(pixelSize);
                case ScalerType.Scale2x: return new Scale2xScaler();
                case ScalerType.Scale3x: return new Scale3xScaler();
                case ScalerType.Bilinear: return new BilinearScaler(pixelSize);
                case ScalerType.SuperEagle: return new SuperEagleScaler();
                case ScalerType._2xSaI: return new _2xSaIScaler();
                case ScalerType.Super2xSaI: return new Super2xSaIScaler();
                default: throw new ArgumentException();
            }
        }
    }
}