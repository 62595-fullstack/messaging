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
            userReviverId = request.ReceiverId,
            userSenderId = request.SenderId,
            userReviver = resiver,
            userSender = sender,
        };

        await db.Message.AddAsync(message);
        await db.SaveChangesAsync();

        return new MessageReply
        {
            Id = message.Id,
            SenderId = message.userSenderId,
            ReceiverId = message.userReviverId,
            Content = message.content,
            CreatedDate = message.CreatedDate.ToString("o")
        };
    }


    public override async Task<AllMessage> ReceiveMessage(UserOfMessage request, ServerCallContext context)
    {
        DatabaseContext db = new();
        AllMessage returnObj = new();

        List<Messages> allTheMessageToAUser = string.IsNullOrEmpty(request.SenderId)
            ? await db.Message.Where(m => m.userReviverId == request.ReceiverId)
                .OrderBy(m => m.CreatedDate).ToListAsync()
            : await db.Message.Where(m =>
                (m.userReviverId == request.ReceiverId && m.userSenderId == request.SenderId) ||
                (m.userSenderId == request.ReceiverId && m.userReviverId == request.SenderId))
                .OrderBy(m => m.CreatedDate).ToListAsync();

        List<MessageReply> listOfMessages = new();

        allTheMessageToAUser.ForEach(x => listOfMessages.Add(new MessageReply
        {
            Id = x.Id,
            Content = x.content,
            ReceiverId = x.userReviverId,
            SenderId = x.userSenderId,
            CreatedDate = x.CreatedDate.ToString("o")
        }));

        returnObj.MessageReplies.Add(listOfMessages);
        return returnObj;

    }
}
