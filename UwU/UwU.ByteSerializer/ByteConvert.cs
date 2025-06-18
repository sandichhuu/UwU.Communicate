using UwU.ByteSerialization.Interfaces;

namespace UwU.ByteSerialization
{
    public static class ByteConvert
    {
        public static int Serialize<T>(Span<byte> buffer, T obj) where T : IByteSerializable
        {
            return obj.Serialize(buffer);
        }

        public static int Deserialize<T>(ReadOnlySpan<byte> data, out T obj) where T : IByteSerializable, new()
        {
            obj = new T();
            return obj.Deserialize(data);
        }
    }
}
