using System.Net.WebSockets;
using UwU.Communicate.Server.Connection;

namespace UwU.Communicate.Server.MessageListener.Interfaces
{
    public interface IMessageListener
    {
        void OnMessage(Instance webSocker, object message);
    }

    public interface IMessageListener<T>
    {
        void OnMessage(Instance webSocker, T message);
    }
}
