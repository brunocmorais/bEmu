using System.ComponentModel;

namespace bEmu.Core.Enums
{
    public enum ScalerType
    {
        [Description("Nenhum")]
        None = 0,
        [Description("Eagle")]
        Eagle = 1,
        [Description("EPX")]
        EPX = 2,
        [Description("Nearest")]
        NearestNeighbor = 3,
        [Description("Scale2x")]
        Scale2x = 4,
        [Description("Scale3x")]
        Scale3x = 5,
        [Description("Bilinear")]
        Bilinear = 6,
        [Description("Super Eagle")]
        SuperEagle = 7,
        [Description("2xSaI")]
        _2xSaI = 8,
        [Description("Super 2xSaI")]
        Super2xSaI = 9
    }
}