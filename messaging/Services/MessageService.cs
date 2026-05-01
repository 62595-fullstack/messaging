using Grpc.Core;

namespace Services.MessageService;

public class MessageService : Message.MessageBase
{
	public override Task<MessageReply> SendMessage(MessageRequest request, ServerCallContext context)
	{
		// var httpContext = context.GetHttpContext();
		// var clientCertificate = httpContext.Connection.ClientCertificate;
		return Task.FromResult(new MessageReply
		{
			SenderId = request.ReceiverId,
			ReceiverId = request.SenderId,
			Content = "Hello " + request.ReceiverId + " from " + request.SenderId
		});
	}
}