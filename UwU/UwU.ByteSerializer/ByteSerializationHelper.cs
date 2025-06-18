using System.Text;

namespace UwU.ByteSerialization
{
    public static class ByteSerializationHelper
    {
        public static byte WriteByte(Span<byte> buffer, byte value)
        {
            buffer[0] = value;
            return sizeof(byte);
        }

        public static int WriteInt64(Span<byte> buffer, long value)
        {
            BitConverter.TryWriteBytes(buffer, value);
            return sizeof(long);
        }

        public static int WriteInt32(Span<byte> buffer, int value)
        {
            BitConverter.TryWriteBytes(buffer, value);
            return sizeof(int);
        }

        public static int WriteFloat(Span<byte> buffer, float value)
        {
            BitConverter.TryWriteBytes(buffer, value);
            return sizeof(float);
        }

        public static int WriteDouble(Span<byte> buffer, double value)
        {
            BitConverter.TryWriteBytes(buffer, value);
            return sizeof(double);
        }

        public static int WriteBool(Span<byte> buffer, bool value)
        {
            BitConverter.TryWriteBytes(buffer, value);
            return sizeof(bool);
        }

        public static int WriteString(Span<byte> buffer, string value)
        {
            var offset = 0;
            var encoding = Encoding.UTF8;
            var byteCount = encoding.GetByteCount(value);

            if (buffer.Length < sizeof(int) + byteCount)
                throw new ArgumentException("Buffer too small");

            var stringBytes = encoding.GetBytes(value);

            //offset += WriteInt32(buffer[offset..], byteCount);
            offset += WriteByteArray(buffer[offset..], stringBytes);

            return offset;
        }


        public static int WriteInt64Array(Span<byte> buffer, long[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteInt64(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteInt32Array(Span<byte> buffer, int[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteInt32(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteFloatArray(Span<byte> buffer, float[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteFloat(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteDoubleArray(Span<byte> buffer, double[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteDouble(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteBoolArray(Span<byte> buffer, bool[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteBool(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteStringArray(Span<byte> buffer, string[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteString(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int WriteByteArray(Span<byte> buffer, byte[] values)
        {
            var offset = WriteInt32(buffer, values.Length);
            var length = values.Length;
            for (var i = 0; i < length; i++)
            {
                offset += WriteByte(buffer[offset..], values[i]);
            }
            return offset;
        }

        public static int ReadByte(ReadOnlySpan<byte> buffer, out byte value)
        {
            value = buffer[0];
            return sizeof(byte);
        }

        public static int ReadInt64(ReadOnlySpan<byte> buffer, out long value)
        {
            value = BitConverter.ToInt64(buffer);
            return sizeof(long);
        }

        public static int ReadInt32(ReadOnlySpan<byte> buffer, out int value)
        {
            value = BitConverter.ToInt32(buffer);
            return sizeof(int);
        }

        public static int ReadFloat(ReadOnlySpan<byte> buffer, out float value)
        {
            value = BitConverter.ToSingle(buffer);
            return sizeof(float);
        }

        public static int ReadDouble(ReadOnlySpan<byte> buffer, out double value)
        {
            value = BitConverter.ToDouble(buffer);
            return sizeof(double);
        }

        public static int ReadBool(ReadOnlySpan<byte> buffer, out bool value)
        {
            value = BitConverter.ToBoolean(buffer);
            return sizeof(bool);
        }

        public static int ReadString(ReadOnlySpan<byte> buffer, out string value)
        {
            var offset = 0;
            offset += ReadByteArray(buffer[offset..], out var byteArray);
            value = Encoding.UTF8.GetString(byteArray);
            return offset;
        }

        public static int ReadInt64Array(ReadOnlySpan<byte> buffer, out long[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new long[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadInt64(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadInt32Array(ReadOnlySpan<byte> buffer, out int[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new int[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadInt32(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadFloatArray(ReadOnlySpan<byte> buffer, out float[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new float[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadFloat(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadDoubleArray(ReadOnlySpan<byte> buffer, out double[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new double[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadDouble(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadBoolArray(ReadOnlySpan<byte> buffer, out bool[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new bool[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadBool(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadStringArray(ReadOnlySpan<byte> buffer, out string[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new string[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadString(buffer[offset..], out values[i]);
            }
            return offset;
        }

        public static int ReadByteArray(ReadOnlySpan<byte> buffer, out byte[] values)
        {
            var offset = 0;
            offset += ReadInt32(buffer, out var length);
            values = new byte[length];
            for (var i = 0; i < length; i++)
            {
                offset += ReadByte(buffer[offset..], out values[i]);
            }
            return offset;
        }
    }
}