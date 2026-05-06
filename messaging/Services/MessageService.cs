using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models.Message;
using Models.User;

namespace Services.MessageService;

public class MessageService : Message.MessageBase
{
    public override async Task<MessageReply> SendMessage(MessageRequest request, ServerCallContext context)
    {
        Console.WriteLine("test");
        // var httpContext = context.GetHttpContext();
        // var clientCertificate = httpContext.Connection.ClientCertificate;

        DatabaseContext db = new();

        Users? resiver = db.User.Where(u => u.Id == request.ReceiverId).FirstOrDefault();
        Users? sender = db.User.Where(u => u.Id == request.SenderId).FirstOrDefault();

        Messages message = new()
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
        DatabaseContext db = new();
        AllMessage returnObj = new();

        List<Messages> allTheMessageToAUser = await db.Message.Where(m => m.userReviverId.Id == request.ReceiverId).ToListAsync();

        List<MessageReply> listOfMessages = new();

        allTheMessageToAUser.ForEach(x => listOfMessages.Add(new MessageReply
        {
            Content = x.content,
            ReceiverId = x.userReviverId.Id,
            SenderId = x.userSender.Id
        }));

        returnObj.MessageReplies.Add(listOfMessages);
        return returnObj;

    }
}
