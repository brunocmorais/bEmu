using System;

namespace bEmu.Core.Util
{
    public class RangeAttribute : Attribute
    {
        public int Minimum;
        public int Maximum;

        public RangeAttribute(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }
}