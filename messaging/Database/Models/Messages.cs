
using Models.User;

namespace Models.Message;

public class Messages
{
	public int Id { get; set; }

	public required string content { get; set; }

    public required  Users userSenderId { get; set; }
    
    public required Users userReviverId { get; set; }

	public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public  Users userSender { get; set; }

	public  Users userReviver { get; set; }
}