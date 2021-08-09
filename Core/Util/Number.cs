namespace bEmu.Core.Util
{
    public class Number<T> where T : struct
    {
        protected T Value;

        protected Number(T value = default)
        {
            Value = value;
        }

        public static implicit operator Number<T>(T value)
        {
            return new Number<T>(value);
        }

        public static implicit operator T(Number<T> value)
        {
            return value.Value;
        }

        public string ToString(string format)
        {
            return uint.Parse(Value.ToString()).ToString(format);
        }
    }
}