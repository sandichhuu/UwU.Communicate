using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;

namespace UwU.Communicate.Message
{
    public sealed class MessageBase
    {
        public static void Initialize()
        {
            TypeRegistry.TypeIdDictionary.Register<OnConnected>();
            TypeRegistry.TypeIdDictionary.Register<OnChatMessage>();
        }
    }

    public class MessageBase<T> : IHaveIndex
    {
        public int GetIndex() => TypeRegistry.TypeIdDictionary.GetId(typeof(T));
    }
}