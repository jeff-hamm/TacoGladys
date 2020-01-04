using TacoLib.Interop;

namespace TacoLib
{
    public class TacoValue
    {
        public TacoDefinition Definition { get; set; }

        private int _intValue;
        public int IntValue
        {
            get => _intValue;
            set
            {
                if (value != _intValue)
                    _value = null;
                _intValue = value;
            }
        }

        private object _value;
        public object Value() => _value ??= Definition.ValueFromInt(IntValue);
    }
}