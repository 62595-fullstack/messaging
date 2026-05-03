// using System.Net.WebSockets;
// using System.Text;
using Grpc.Net.Client;

namespace messaging;

public class UnitTest1
{
    [Fact]
    public async Task Test()
    {
        Console.WriteLine("AAA");
        using var channel = GrpcChannel.ForAddress("https://localhost:5500");
        // using var channel = GrpcChannel.ForAddress("https://localhost:5001");
        Console.WriteLine("BBB");
        var client = new Message.MessageClient(channel);
        Console.WriteLine("CCC");
        var reply = await client.SendMessageAsync(new MessageRequest
        {
            Content = "Hellooooo",
            ReceiverId = "1000",
            SenderId = "999",
        }, cancellationToken: TestContext.Current.CancellationToken);
        Console.WriteLine("DDD");
        Console.WriteLine("Greeting: " + reply.Content);
        // Console.WriteLine("Press any key to exit...");
        // Console.ReadKey();
        // Assert.True(false);
    }

    // [Fact]
    // public async Task Test1()
    // {
    //     // Arrange
    //     // Uri uri = new("https://localhost:5500");
    //     // Uri uri = new("http://localhost:5500");
    //     Uri uri = new("ws://localhost:5500");
    //
    //     // Act
    //     using ClientWebSocket ws = new();
    //     await ws.ConnectAsync(uri, default);
    //
    //     var bytes = new byte[1024];
    //     var result = await ws.ReceiveAsync(bytes, default);
    //     string res = Encoding.UTF8.GetString(bytes, 0, result.Count);
    //
    //     await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", default);
    //
    //     // Assert
    //     Assert.True(false);
    // }
}
