using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Server.Connection;
using UwU.Communicate.Server.MessageListener.Interfaces;

namespace UwU.Communicate.Server.MessageListener.Adapter
{
    public class MessageListenerAdapter<T>(IMessageListener<T> inner) : IMessageListener where T : IByteSerializable
    {
        public void OnMessage(Instance instance, object message)
        {
            if (message is T typedMessage)
            {
                inner.OnMessage(instance, typedMessage);
            }
            else
            {
                throw new InvalidCastException($"Invalid message type: expected {typeof(T)}, got {message.GetType()}");
            }
        }
    }
}