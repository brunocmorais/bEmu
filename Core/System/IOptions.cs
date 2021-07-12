using bEmu.Core.Enums;
using bEmu.Core.GUI;

namespace bEmu.Core.System
{
    public interface IOptions
    {
        int Frameskip { get; }
        bool ShowFPS { get; }
        ScalerType Scaler { get; }
        int Size { get; }
        bool EnableSound { get; }

        void OptionChangedEvent(object sender, OnOptionChangedEventArgs e);
        void SetOption(string optionName, bool increment);
    }
}