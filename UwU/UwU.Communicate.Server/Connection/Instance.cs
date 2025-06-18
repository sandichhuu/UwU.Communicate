using Snappier;
using System.Buffers;
using System.Net.WebSockets;
using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Config;
using UwU.Communicate.Message.TypeRegistry;

namespace UwU.Communicate.Server.Connection
{
    public class Instance
    {
        public readonly WebSocket socket;

        public Guid uuid { get; private set; }

        public Instance(WebSocket socket)
        {
            this.socket = socket;
            this.uuid = Guid.NewGuid();
        }

        public async Task Send<T>(T obj) where T : IByteSerializable, new()
        {
            var useCompression = GlobalConfig.USE_COMPRESSSION;

            Span<byte> buffer = stackalloc byte[GlobalConfig.BUFFER_SIZE];
            var offset = 0;
            offset += ByteSerializationHelper.WriteInt32(buffer[offset..], GlobalConfig.USE_COMPRESSSION);
            offset += ByteSerializationHelper.WriteInt32(buffer[offset..], TypeIdDictionary.GetId(obj.GetType()));

            if (useCompression == 0)
            {
                offset += obj.Serialize(buffer[offset..]);
            }
            else if (useCompression == 1)
            {
                Span<byte> compressBuffer = stackalloc byte[GlobalConfig.BUFFER_SIZE];
                var length = obj.Serialize(compressBuffer);
                using IMemoryOwner<byte> compressed = Snappy.CompressToMemory(compressBuffer[..length]);
                var compressedBytes = compressed.Memory.ToArray();
                offset += ByteSerializationHelper.WriteByteArray(buffer[offset..], compressedBytes);
            }
            else
            {
                throw new Exception("Unsupported compression method [2]LZMA !");
            }

            byte[] temp = ArrayPool<byte>.Shared.Rent(offset);
            buffer[..offset].CopyTo(temp);

            try
            {
                await this.socket.SendAsync(
                    new ArraySegment<byte>(temp),
                    WebSocketMessageType.Binary,
                    true,
                    CancellationToken.None
                );
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(temp);
            }

            if (GlobalConfig.TRACE_COMMUNICATE_DEBUG)
            {
                Console.WriteLine($"Send: {temp.Length} bytes");
            }
        }
    }
}