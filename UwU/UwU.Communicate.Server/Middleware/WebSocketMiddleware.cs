using System.Net.WebSockets;
using UwU.ByteSerialization;
using UwU.Communicate.Config;
using UwU.Communicate.Message;
using UwU.Communicate.Message.TypeRegistry;
using UwU.Communicate.Server.Connection;
using UwU.Communicate.Server.Services;

namespace UwU.Communicate.Server.Middleware;

public class WebSocketMiddleware(RequestDelegate next)
{
    private readonly WebSocketHandler handler = new();

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/game" && context.WebSockets.IsWebSocketRequest)
        {
            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                var instance = InstanceManager.Create(socket);
                await SendConnectionSuccess(instance);
                await this.handler.HandleAsync(instance);
                InstanceManager.Remove(instance.uuid);
            }
        }
        else
        {
            await next(context);
        }
    }

    private static async Task SendConnectionSuccess(Instance instance)
    {
        var connectedMessage = new OnConnected
        {
            connectionId = instance.uuid.ToString(),
            message = "Đã kết nối thành công"
        };
        
        await instance.Send(connectedMessage);
    }
}
