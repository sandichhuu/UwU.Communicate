using System.Buffers;
using System.Net.WebSockets;

namespace UwU.Communicate.Server.Extensions
{
    public static class WebSocketExtensions
    {
        public static ValueTask SendAsync(this WebSocket socket, ReadOnlySpan<byte> buffer,
            WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken = default)
        {
            byte[] temp = ArrayPool<byte>.Shared.Rent(buffer.Length);
            buffer.CopyTo(temp);

            var segment = new ArraySegment<byte>(temp, 0, buffer.Length);

            var task = socket.SendAsync(segment, messageType, endOfMessage, cancellationToken);

            return new ValueTask(task.ContinueWith(_ => ArrayPool<byte>.Shared.Return(temp)));
        }
    }
}