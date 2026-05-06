using Models.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Message;

public class Messages
{
    public int Id { get; set; }

    public required string content { get; set; }

    public required string userSenderId { get; set; }

    public required string userReviverId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public Users? userSender { get; set; }

    [NotMapped]
    public Users? userReviver { get; set; }
}
