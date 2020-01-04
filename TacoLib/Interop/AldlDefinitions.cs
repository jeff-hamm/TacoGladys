using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TacoLib.Data;

namespace TacoLib.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AldlDefinitionMessage
    {
        public const string MAGIC_PHRASE_VAL = "STREAMDEFINITIONS:";
        public long Length;
        public TacoDefinition[] Defs;

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct AldlDataMessage
    {
        public long Length;
        public long Time;
        public AldlData[] Data;

    }
    public struct AldlData
    {
        private double? _floatVal;
        public double FloatVal => _floatVal ?? (_floatVal = BitConverter.Int32BitsToSingle(IntVal)).Value;
        public int IntVal;
    }

    public enum AldlDataType
    {
        ALDL_INT = 0,
        ALDL_FLOAT = 1,
        ALDL_BOOL = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TacoDefinition
    {
        public int Index { get; set; }
        private TacoValueId? _valueId;
        public TacoValueId? ValueId => _valueId ??=
            Enum.TryParse<TacoValueId>(Id, out var val) ? val : (TacoValueId?) null;
        // char*
        public string Id; /* unique name and identifier */

        //char *
        public string Description; /* description of each definition */

        /* ----- stuff for modules ----------------------------*/
        public bool log; /* log data from this definition */
        public bool display; /* display data from this definition */
        public bool alarm_low_enable;
        public bool alarm_high_enable; /* enable high/low alarms */

        public AldlData alarm_low, alarm_high; /* value for alarms */

        /* ----- output definition -------------------------- */
        public AldlDataType type; /* the OUTPUT type */
        public string Unit; /* unit of measure string */

        public byte precision; /* floating point display precision.  not used in
                           conversion; only for display plugins. */

        public AldlData min, max; /* the low and high range of OUTPUT value */

        /* ----- conversion ----------------------------------*/
        public AldlData adder; /* forms a linear equation, such as */

        public AldlData multiplier; /* MULTIPLIER(n)+ADDER */

        /* ----- input definition --------------------------- */
        public byte packet; /* selects which packet unique id the data comes from */
        public byte offset; /* offset within packet in bytes */

        public byte size; /* size in bits.  8,16,32 only... */

        /* binary stuff only */
        public byte binary; /* offset in bits.  only works for 1 bit fields */
        public bool invert; /* invert (0 means no) */
        public byte err; /* is an error code */

        public object ValueFromInt(int intVal)
        {
            switch (type)
            {
                case AldlDataType.ALDL_BOOL:
                    return intVal != 0;
                case AldlDataType.ALDL_FLOAT:
                    return (double) BitConverter.Int32BitsToSingle(intVal);
                case AldlDataType.ALDL_INT:
                default:
                    return intVal;
            }
        }

        public object MaxValue() =>
            ValueFromInt(max.IntVal);
        public object MinValue() =>
            ValueFromInt(min.IntVal);
    }


}