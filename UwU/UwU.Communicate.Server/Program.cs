using System.Text;
using UwU.Communicate.Server;
using UwU.Communicate.Server.MessageListener;
using UwU.Communicate.Server.Middleware;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var serviceListener = new ServiceListener();
var chatMessageListener = new ChatMessageListener();

serviceListener.RegisterListener(chatMessageListener);

app.UseWebSockets();
app.UseMiddleware<WebSocketMiddleware>();

app.Run();