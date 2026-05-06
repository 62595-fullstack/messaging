using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Models.Comment;

namespace Services.CommentService;

public class CommentService : Comment.CommentBase
{
    public override async Task<CommentReply> CreateComment(CreateCommentRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Content is required."));
        }
        if (string.IsNullOrWhiteSpace(request.AuthorUserId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "AuthorUserId is required."));
        }
        if (request.EventId <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "EventId is required."));
        }

        DatabaseContext db = new();

        int? parentId = request.ParentCommentId > 0 ? request.ParentCommentId : null;
        if (parentId.HasValue)
        {
            Comments? parent = await db.Comment.FirstOrDefaultAsync(c => c.Id == parentId.Value);
            if (parent == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Parent comment not found."));
            }
            if (parent.EventId != request.EventId)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Parent comment belongs to a different event."));
            }
        }

        Comments comment = new()
        {
            EventId = request.EventId,
            ParentCommentId = parentId,
            AuthorUserId = request.AuthorUserId,
            Content = request.Content,
        };

        await db.Comment.AddAsync(comment);
        await db.SaveChangesAsync();

        return ToReply(comment);
    }

    public override async Task<CommentList> ListCommentsByEvent(ListCommentsRequest request, ServerCallContext context)
    {
        DatabaseContext db = new();
        List<Comments> rows = await db.Comment
            .Where(c => c.EventId == request.EventId)
            .OrderBy(c => c.CreatedDate)
            .ToListAsync();

        CommentList result = new();
        foreach (Comments c in rows)
        {
            result.Comments.Add(ToReply(c));
        }
        return result;
    }

    private static CommentReply ToReply(Comments c) => new()
    {
        Id = c.Id,
        EventId = c.EventId,
        ParentCommentId = c.ParentCommentId ?? 0,
        AuthorUserId = c.AuthorUserId,
        Content = c.Content,
        CreatedDate = c.CreatedDate.ToString("o"),
    };
}
