using Grpc.Net.Client;
using static Message;

namespace messaging;

public class GrpcTest
{
    [Fact]
    public async Task Grpc_Message_Successfully()
    {
        // Arrange
        using GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5500");
        MessageClient client = new MessageClient(channel);
        // Act
        MessageReply reply = await client.SendMessageAsync(new MessageRequest
        {
            Content = "Hello from client!",
            ReceiverId = "1000",
            SenderId = "999",
        }, cancellationToken: TestContext.Current.CancellationToken);
        // Assert
        Assert.NotNull(reply);
    }
}
