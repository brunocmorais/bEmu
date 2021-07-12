using bEmu.Core.Enums;
using bEmu.Core.Video.Scalers;

namespace bEmu.Core.Components
{
    public interface IOptions
    {
        int Frameskip { get; }
        bool ShowFPS { get; }
        ScalerType Scaler { get; }
        int Size { get; }
        bool EnableSound { get; }

        void SetOption(string optionName, bool increment);
    }
}