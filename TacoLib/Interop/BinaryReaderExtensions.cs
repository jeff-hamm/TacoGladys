using System;
using System.IO;

namespace TacoLib.Interop
{
    public static class BinaryReaderExtensions
    {
        public static byte ReadByte(this BinaryReader r, ref int bytes)
        {
            bytes += 1;
            return r.ReadByte();
        }
        public static char ReadChar(this BinaryReader r, ref int bytes)
        {
            bytes += 1;
            return r.ReadChar();
        }
        public static int ReadInt32(this BinaryReader r, ref int bytes)
        {
            bytes += 4;
            return r.ReadInt32();
        }
        public static bool ReadInt32AsBool(this BinaryReader r, ref int bytes)
        {
            bytes += 4;
            return r.ReadInt32() != 0;
        }
        public static bool ReadByteAsBool(this BinaryReader r, ref int bytes)
        {
            bytes += 1;
            return r.ReadByte() != 0;
        }

        public static long ReadPointer(this BinaryReader r, ref int bytes)
        {
            if (Environment.Is64BitOperatingSystem)
                return r.ReadInt64(ref bytes);
            else
                return r.ReadInt32(ref bytes);

        }

        public static long ReadInt64(this BinaryReader r, ref int bytes)
        {
            bytes += 8;
            return r.ReadInt64();
        }

        public static string ReadChoppedAsciiString(this BinaryReader r, int truncate, ref int bytes)
        {
            var i = r.ReadInt32(ref bytes);
            if (i == 0) 
                return String.Empty;
            if( i < 0) 
                throw new InvalidOperationException();
            bytes += i;
            return new string(r.ReadChars(i-truncate));

        }
        public static string ReadAsciiString(this BinaryReader r, ref int bytes)
        {
            var i = r.ReadInt32(ref bytes);
            if (i == 0) 
                return String.Empty;
            if( i < 0) 
                throw new InvalidOperationException();
            bytes += i;
            return new string(r.ReadChars(i));
        }
    }
}