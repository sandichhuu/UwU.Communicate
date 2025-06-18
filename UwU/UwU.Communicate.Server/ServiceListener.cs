using Snappier;
using System.Buffers;
using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Config;
using UwU.Communicate.Message;
using UwU.Communicate.Message.TypeRegistry;
using UwU.Communicate.Server.Connection;
using UwU.Communicate.Server.MessageListener.Adapter;
using UwU.Communicate.Server.MessageListener.Interfaces;
using UwU.Communicate.Server.Services;

namespace UwU.Communicate.Server
{
    public class ServiceListener
    {
        private readonly Dictionary<Type, IMessageListener> listeners = [];

        public ServiceListener()
        {
            MessageBase.Initialize();
            WebSocketHandler.OnReceived += OnReceived;
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
                if (GlobalConfig.TRACE_COMMUNICATE_DEBUG)
                {
                    Console.WriteLine($"Dispatch: {message}");
                }
                listener.OnMessage(instance, message);
            }
            else
            {
                throw new KeyNotFoundException($"No listener registered for message type {messageType}");
            }
        }

        public void OnReceived(Instance instance, byte[] buffer)
        {
            ReadOnlySpan<byte> dataBuffer;
            ReadOnlySpan<byte> payloadBuffer = buffer;

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
                    DispatchMessage(instance, serializable);
                }
                else
                {
                    throw new InvalidCastException($"Instance of type {objectType.Name} does not implement IByteSerializable.");
                }
            }
        }
    }
}