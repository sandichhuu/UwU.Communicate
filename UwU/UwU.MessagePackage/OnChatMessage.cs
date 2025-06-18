using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;

namespace UwU.Communicate.Message
{
    public class OnChatMessage : MessageBase<OnChatMessage>, IByteSerializable
    {
        public string message = string.Empty;

        public int Serialize(Span<byte> buffer)
        {
            var offset = 0;
            offset += ByteSerializationHelper.WriteString(buffer[offset..], this.message);
            return offset;
        }

        public int Deserialize(ReadOnlySpan<byte> data)
        {
            var offset = 0;
            offset += ByteSerializationHelper.ReadString(data[offset..], out this.message);
            return offset;
        }

        public override string ToString()
        {
            return $"{{ message: {this.message} }}";
        }
    }
}