using System;
using bEmu.Core;
using bEmu.Core.Enums;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Video.Scalers
{
    public static class ScalerFactory
    {
        public static IScaler Get(ScalerType scaler, int pixelSize, IFrameBuffer frameBuffer)
        {
            switch (scaler)
            {
                case ScalerType.None: return new EmptyScaler(frameBuffer);
                case ScalerType.Eagle: return new EagleScaler(frameBuffer);
                case ScalerType.EPX: return new EPXScaler(frameBuffer);
                case ScalerType.NearestNeighbor: return new NearestNeighborScaler(frameBuffer, pixelSize);
                case ScalerType.Scale2x: return new Scale2xScaler(frameBuffer);
                case ScalerType.Scale3x: return new Scale3xScaler(frameBuffer);
                case ScalerType.Bilinear: return new BilinearScaler(frameBuffer, pixelSize);
                case ScalerType.SuperEagle: return new SuperEagleScaler(frameBuffer);
                case ScalerType._2xSaI: return new _2xSaIScaler(frameBuffer);
                case ScalerType.Super2xSaI: return new Super2xSaIScaler(frameBuffer);
                default: throw new ArgumentException();
            }
        }
    }
}