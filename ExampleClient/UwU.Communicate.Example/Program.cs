using System.Text;
using UwU.Communicate.Client;
using UwU.Communicate.Example;
using UwU.Communicate.Message;

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var connectListener = new ConnectListener();
        var chatMessageListener = new ChatMessageListener();

        var instance = new Instance();
        instance.RegisterListener(connectListener);
        instance.RegisterListener(chatMessageListener);

        await instance.Connect("ws://localhost:5000/game");
        StartListen(instance);

        while (instance.IsConnectionAlive())
        {
            Console.Write("> ");
            string input = GetInput();
            await instance.Send(new OnChatMessage
            {
                message = input
            });
            Console.WriteLine($"Đã gửi tới server: {input}");
        }

        instance.Dispose();
    }

    private static string GetInput()
    {
        return Console.ReadLine();
    }

    private static async void StartListen(Instance instance)
    {
        await instance.StartListen();
    }
}
