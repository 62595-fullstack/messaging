using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models.Message;

namespace Services.MessageService;

public class MessageService : Message.MessageBase
{
    public override async Task<MessageReply> SendMessage(MessageRequest request, ServerCallContext context)
    {
        System.Console.WriteLine("test");
        // var httpContext = context.GetHttpContext();
        // var clientCertificate = httpContext.Connection.ClientCertificate;

        DatabaseContext db = new DatabaseContext();

        var resiver = db.User.Where(u => u.Id == request.ReceiverId).FirstOrDefault();
        var sender = db.User.Where(u => u.Id == request.SenderId).FirstOrDefault();

        var message = new Messages()
        {
            content = request.Content,
            userReviverId = resiver,
            userSenderId = sender,

        };

        await db.Message.AddAsync(message);

        return new MessageReply
        {
            SenderId = request.ReceiverId,
            ReceiverId = request.SenderId,
            Content = "Hello " + request.ReceiverId + " from " + request.SenderId
        };
    }


    public override async Task<AllMessage> ReceiveMessage(UserOfMessage request, ServerCallContext context)
    {
        DatabaseContext db = new DatabaseContext();
        AllMessage returnObj = new AllMessage();

        var allTheMessageToAUser = await db.Message.Where(m => m.userReviverId.Id == request.ReceiverId).ToListAsync();

        List<MessageReply> listOfMessages = new List<MessageReply>();

        allTheMessageToAUser.ForEach(x => listOfMessages.Add(new MessageReply
        {
            Content = x.content,
            ReceiverId = x.userReviverId.Id,
            SenderId = x.userSender.Id
        }));

        returnObj.MessageReplys.Add(listOfMessages);
        return returnObj;

    }
}
