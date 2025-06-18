using UwU.ByteSerialization.Interfaces;

namespace UwU.Communicate.Message.TypeRegistry
{
    public static class TypeIdDictionary
    {
        private static readonly Dictionary<int, Type> idToType = [];
        private static readonly Dictionary<Type, int> typeToId = [];
        private static int currentId = 1;

        public static void Register<T>() where T : IByteSerializable
        {
            var type = typeof(T);
            if (!typeToId.ContainsKey(type))
            {
                typeToId[type] = currentId;
                idToType[currentId] = type;
                currentId++;
            }
        }

        public static int GetId(Type type) => typeToId[type];

        public static Type GetTypeByIndex(int id) => idToType.TryGetValue(id, out var type) ? type : null;
    }
}
