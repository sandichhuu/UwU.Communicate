using UwU.Communicate.Message;
using UwU.Communicate.Server.Connection;
using UwU.Communicate.Server.MessageListener.Interfaces;

namespace UwU.Communicate.Server.MessageListener
{
    public class ChatMessageListener : IMessageListener<OnChatMessage>
    {
        public ChatMessageListener()
        {
        }

        public void OnMessage(Instance instance, OnChatMessage message)
        {
            Console.WriteLine($"ReceivedMessage from {instance.uuid}");
            Console.WriteLine($"MessageContent: {message.message}");
        }
    }
}