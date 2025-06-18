namespace UwU.ByteSerialization.Interfaces
{
    public interface IByteSerializable
    {
        int Serialize(Span<byte> buffer);
        int Deserialize(ReadOnlySpan<byte> data);
    }
}