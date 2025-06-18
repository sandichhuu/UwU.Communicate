using System.Net.WebSockets;

namespace UwU.Communicate.Server.Connection
{
    public static class InstanceManager
    {
        private static readonly Dictionary<Guid, Instance> Container = [];

        public static Instance Create(WebSocket socket)
        {
            var instance = new Instance(socket);
            Container[instance.uuid] = instance;
            return instance;
        }

        public static Instance GetInstance(Guid uuid) => Container[uuid];

        public static void Remove(Guid uuid)
        {
            Container.Remove(uuid);
        }
    }
}