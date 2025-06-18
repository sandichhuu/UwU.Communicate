using UwU.Communicate.Client;
using UwU.Communicate.Client.MessageListener.Interfaces;
using UwU.Communicate.Message;

namespace UwU.Communicate.Example
{
    public class ChatMessageListener : IMessageListener<OnChatMessage>
    {
        public void OnMessage(Instance webSocker, OnChatMessage data)
        {
            Console.WriteLine("Tin nhắn tới");
            Console.WriteLine($"Content: {data.message}");
        }
    }
}