# UwU.Communicate

**A lightweight, high-performance WebSocket library for .NET — supports typed messaging, Snappier compression, and zero-allocation design using Span\<T\>.**

---

## ✨ Overview

`UwU.Communicate` is a fast and efficient WebSocket library built for modern .NET applications. It enables:

- Strongly-typed message sending and receiving.
- Automatic compression and decompression using [Snappier](https://github.com/google/snappy).
- Low memory allocations thanks to `Span<T>` and memory-efficient data handling.

---

## 🚀 Key Features

- **Typed Message Support**  
  Send and receive strongly-typed objects with no need to manually handle raw byte arrays or JSON parsing.

- **High-Speed Compression with Snappier**  
  Efficient packet compression and decompression reduce bandwidth usage without compromising speed.

- **Allocation-Free Design**  
  Leverages `Span<T>` to minimize heap allocations and reduce GC pressure.

- **Flexible Integration**  
  Easily integrates with Windows Form, ASP.NET Core, console applications, or any .NET-based server or client.
  Currently not support Unity 3D because this library is based on .NET 9. (Unity3D still using old .NET 4.6 and performance not good)
---

## 🔧 Installation

The package is not published to NuGet yet. You can clone the repository and reference it locally:

```bash
git clone https://github.com/sandichhuu/UwU.Communicate.git
```

## 🔧 How to use

- ** Step1: Create a package data**
```csharp
using UwU.ByteSerialization;
using UwU.ByteSerialization.Interfaces;
using UwU.Communicate.Message;

public class OnChatMessage : MessageBase<OnChatMessage>, IByteSerializable
{
    public string displayName = string.Empty;
    public string message = string.Empty;

    // Convert data on this class to byteArray.
    public int Serialize(Span<byte> buffer)
    {
        var offset = 0;
        offset += ByteSerializationHelper.WriteString(buffer[offset..], this.message);
        offset += ByteSerializationHelper.WriteString(buffer[offset..], this.displayName);
        return offset;
    }

    // Convert from byteArray into values.
    public int Deserialize(ReadOnlySpan<byte> data)
    {
        var offset = 0;
        offset += ByteSerializationHelper.ReadString(data[offset..], out this.message);
        offset += ByteSerializationHelper.ReadString(data[offset..], out this.displayName);
        return offset;
    }

    public override string ToString()
    {
        return $"{{ {this.displayName}: {this.message} }}";
    }
}
```

- ** Step2: Create a data listener**
```csharp
using UwU.Communicate.Message;
using UwU.Communicate.Server.Connection;
using UwU.Communicate.Server.MessageListener.Interfaces;
using UwU.Communicate.Server.MessageListener;

public class ChatMessageListener : IMessageListener<OnChatMessage>
{
    public void OnMessage(Instance instance, OnChatMessage message)
    {
        Console.WriteLine($"ReceivedMessage from {instance.uuid}");
        Console.WriteLine($"MessageContent: {message.message}");
    }
}
```

- ** Step3: Sign the listener on main**
```csharp
using System.Text;
using UwU.Communicate.Server;
using UwU.Communicate.Server.MessageListener;
using UwU.Communicate.Server.Middleware;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var serviceListener = new ServiceListener();

// Sign your listener to start
var chatMessageListener = new ChatMessageListener();
serviceListener.RegisterListener(chatMessageListener);

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.Run();
```

- ** Example SendData**
```csharp
using UwU.Communicate.Message;
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

    // Function to send data package
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
```
