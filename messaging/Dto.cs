namespace Dto;

public record CreateCommentRequestDto(int EventId, string Content, int? ParentCommentId);
public record CommentDto(int Id, int EventId, int? ParentCommentId, string AuthorUserId, string Content, string CreatedDate);
public record SendMessageRequestDto(string ReceiverUserId, string Content);
public record MessageDto(int Id, string SenderUserId, string ReceiverUserId, string Content, string CreatedDate);
