using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Client.MessageListener.Interfaces;

namespace UwU.Communicate.Client.MessageListener.Adapter
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