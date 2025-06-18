using UwU.ByteSerialization.Interfaces;

namespace UwU.ByteSerialization
{
    public sealed class Payload<T> where T : IByteSerializable, new()
    {
        public int compression; // 0: No compression, 1: Snappier compression, 2: LZMA compression (next version planned)
        public int type;
        public T value;

        //public int Serialize(Span<byte> buffer)
        //{
        //    var offset = 0;
        //    offset += ByteSerializationHelper.WriteInt32(buffer[offset..], this.compression);
        //    offset += ByteSerializationHelper.WriteInt32(buffer[offset..], this.type);
        //    offset += this.value.Serialize(buffer[offset..]);
        //    return offset;
        //}

        //public int Deserialize(ReadOnlySpan<byte> data)
        //{
        //    var offset = 0;
        //    offset += ByteSerializationHelper.ReadInt32(data[offset..], out this.compression);
        //    offset += ByteSerializationHelper.ReadInt32(data[offset..], out this.type);
        //    offset += ByteConvert.Deserialize(data[offset..], out this.value);
        //    return offset;
        //}
    }
}