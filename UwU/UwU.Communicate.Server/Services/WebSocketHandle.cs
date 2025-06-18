using System.Net.WebSockets;
using UwU.Communicate.Config;
using UwU.Communicate.Server.Connection;

namespace UwU.Communicate.Server.Services;

public class WebSocketHandler
{
    public static Action<Instance, byte[]> OnReceived { get; set; }

    public async Task HandleAsync(Instance instance)
    {
        var buffer = new byte[GlobalConfig.BUFFER_SIZE];

        while (instance.socket.State == WebSocketState.Open)
        {
            try
            {
                var result = await instance.socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await instance.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
                    break;
                }

                OnReceived?.Invoke(instance, buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                break;
            }
        }
    }
}