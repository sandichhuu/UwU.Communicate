namespace UwU.Communicate.Client.MessageListener.Interfaces
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