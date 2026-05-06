namespace Models.Comment;

public class Comments
{
    public int Id { get; set; }

    public required int EventId { get; set; }

    public int? ParentCommentId { get; set; }

    public Comments? ParentComment { get; set; }

    public required string AuthorUserId { get; set; }

    public required string Content { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
