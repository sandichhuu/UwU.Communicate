using Snappier;
using System.Buffers;
using System.Net.WebSockets;
using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Client.MessageListener.Adapter;
using UwU.Communicate.Client.MessageListener.Interfaces;
using UwU.Communicate.Config;
using UwU.Communicate.Message;
using UwU.Communicate.Message.TypeRegistry;

namespace UwU.Communicate.Client
{
    public class Instance : IDisposable
    {
        private readonly Dictionary<Type, IMessageListener> listeners = [];
        private readonly ClientWebSocket socket;

        public Instance()
        {
            MessageBase.Initialize();
            this.socket = new ClientWebSocket();
        }

        public void RegisterListener<T>(IMessageListener<T> listener) where T : IByteSerializable
        {
            this.listeners[typeof(T)] = new MessageListenerAdapter<T>(listener);
        }

        private void DispatchMessage(Instance instance, IByteSerializable message)
        {
            var messageType = message.GetType();

            if (this.listeners.TryGetValue(messageType, out var listener))
            {
                listener.OnMessage(instance, message);
            }
            else
            {
                throw new KeyNotFoundException($"No listener registered for message type {messageType}");
            }
        }

        /// <summary>
        /// wss:// nếu dùng trên server có tên miền (ws:// dùng cho địa chỉ IP:port)
        /// Ví dụ:
        /// client.Connect("ws://localhost:5245/game")
        /// </summary>
        /// <param name="wsAddress"></param>
        public async Task Connect(string wsAddress)
        {
            var uri = new Uri(wsAddress);
            Console.WriteLine("🔌 Đang kết nối tới server WebSocket...");
            await this.socket.ConnectAsync(uri, CancellationToken.None);
            Console.WriteLine("✅ Đã kết nối!");
        }

        public async Task StartListen()
        {
            try
            {
                while (this.socket.State == WebSocketState.Open)
                {
                    var receiveBuffer = new byte[GlobalConfig.BUFFER_SIZE];
                    var result = await this.socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("⚠️ Server request close connection.");
                        await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }
                    else if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        OnReceiveMessage(receiveBuffer[..result.Count]);
                        break;
                    }
                    else
                    {
                        throw new Exception($"Unsupport message type: {result.MessageType}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}\n{ex.StackTrace}");
                await this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                Console.WriteLine("🔒 Connection closed.");
            }
        }

        public async Task Send<T>(T obj) where T : IByteSerializable
        {
            var useCompression = GlobalConfig.USE_COMPRESSSION;

            Span<byte> sendBuffer = stackalloc byte[GlobalConfig.BUFFER_SIZE];
            var offset = 0;
            offset += ByteSerializationHelper.WriteInt32(sendBuffer[offset..], GlobalConfig.USE_COMPRESSSION);
            offset += ByteSerializationHelper.WriteInt32(sendBuffer[offset..], TypeIdDictionary.GetId(obj.GetType()));

            if (useCompression == 0)
            {
                offset += obj.Serialize(sendBuffer[offset..]);
            }
            else if (useCompression == 1)
            {
                Span<byte> compressBuffer = stackalloc byte[GlobalConfig.BUFFER_SIZE];
                var length = obj.Serialize(compressBuffer);
                using IMemoryOwner<byte> compressed = Snappy.CompressToMemory(compressBuffer[..length]);
                offset += ByteSerializationHelper.WriteByteArray(sendBuffer[offset..], compressed.Memory.ToArray());
            }
            else
            {
                throw new Exception("Unsupported compression method [2]LZMA !");
            }

            byte[] temp = ArrayPool<byte>.Shared.Rent(offset);
            sendBuffer[..offset].CopyTo(temp);

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
        }

        public bool IsConnectionAlive()
        {
            return this.socket.State == WebSocketState.Open;
        }

        private void OnReceiveMessage(ReadOnlySpan<byte> payloadBuffer)
        {
            ReadOnlySpan<byte> dataBuffer;

            var offset = 0;
            offset += ByteSerializationHelper.ReadInt32(payloadBuffer[offset..], out var isCompressed);
            offset += ByteSerializationHelper.ReadInt32(payloadBuffer[offset..], out var objectTypeId);

            if (GlobalConfig.TRACE_COMMUNICATE_DEBUG)
            {
                Console.WriteLine($"Received: {payloadBuffer[offset..].Length} bytes");
            }

            if (isCompressed == 0)
            {
                dataBuffer = payloadBuffer[offset..];
            }
            else if (isCompressed == 1)
            {
                offset += ByteSerializationHelper.ReadByteArray(payloadBuffer[offset..], out var compressedDataBuffer);
                using IMemoryOwner<byte> decompressed = Snappy.DecompressToMemory(compressedDataBuffer);
                dataBuffer = decompressed.Memory.ToArray();
            }
            else
            {
                throw new Exception("Not support compression method [2]");
            }

            var objectType = TypeIdDictionary.GetTypeByIndex(objectTypeId);
            if (objectType != null)
            {
                var newObject = Activator.CreateInstance(objectType);
                var deserializer = DeserializeCache.GetDeserializeDelegate(objectType);
                deserializer(newObject, dataBuffer);

                if (newObject is IByteSerializable serializable)
                {
                    DispatchMessage(this, serializable);
                }
                else
                {
                    throw new InvalidCastException($"Instance of type {objectType.Name} does not implement IByteSerializable.");
                }
            }
        }

        public void Dispose()
        {
            this.socket.Dispose();
        }
    }
}