using UwU.Communicate.Client;
using UwU.Communicate.Client.MessageListener.Interfaces;
using UwU.Communicate.Message;

namespace UwU.Communicate.Example
{
    public class ConnectListener : IMessageListener<OnConnected>
    {
        public void OnMessage(Instance webSocker, OnConnected data)
        {
            Console.WriteLine("Đã kết nối");
            Console.WriteLine($"ConnectionId: {data.connectionId}");
            Console.WriteLine($"Content: {data.message}");
        }
    }
}