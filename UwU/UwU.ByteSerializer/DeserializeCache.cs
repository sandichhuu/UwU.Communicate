using System.Linq.Expressions;

namespace UwU.ByteSerialization
{
    public static class DeserializeCache
    {
        public delegate void DeserializeDelegate(object instance, ReadOnlySpan<byte> buffer);
        private static readonly Dictionary<Type, DeserializeDelegate> Cache = [];

        public static DeserializeDelegate GetDeserializeDelegate(Type type)
        {
            if (Cache.TryGetValue(type, out var del))
                return del;

            var method = type.GetMethod("Deserialize", [typeof(ReadOnlySpan<byte>)]);
            if (method == null)
                throw new MissingMethodException($"Type {type.Name} has no method Deserialize(ReadOnlySpan<byte>).");

            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var bufferParam = Expression.Parameter(typeof(ReadOnlySpan<byte>), "buffer");

            var castInstance = Expression.Convert(instanceParam, type);
            var call = Expression.Call(castInstance, method, bufferParam);

            var lambda = Expression.Lambda<DeserializeDelegate>(call, instanceParam, bufferParam);
            del = lambda.Compile();

            Cache[type] = del;
            return del;
        }
    }
}
