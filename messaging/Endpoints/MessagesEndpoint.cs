using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Models.Message;
using Services.MessageService;
using System.Security.Claims;

namespace Endpoints;

public record SendMessageRequestDto(string ReceiverUserId, string Content);

public record MessageDto(
	int Id,
	string SenderUserId,
	string ReceiverUserId,
	string Content,
	string CreatedDate
);

public static class MessagesEndpoint
{
	public static RouteGroupBuilder MapMessagesEndpoints(this RouteGroupBuilder group)
	{
		group.MapGet("/with/{otherUserId}", async Task<IResult> (string otherUserId, ClaimsPrincipal user) =>
		{
			try
			{
				string? meId = user.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(meId)) return Results.Unauthorized();
				if (string.IsNullOrWhiteSpace(otherUserId)) return Results.BadRequest("otherUserId is required.");

				MessageService service = new();
				AllMessage reply = await service.ReceiveMessage(
					new UserOfMessage { SenderId = otherUserId, ReceiverId = meId },
					null!
				);

				List<MessageDto> result = reply.MessageReplies.Select(ToDto).ToList();
				return Results.Ok(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return Results.Problem(ex.Message);
			}
		})
		.WithName("ListMessagesWithUser");

		group.MapPost("/", async Task<IResult> ([FromBody] SendMessageRequestDto req, ClaimsPrincipal user) =>
		{
			try
			{
				string? meId = user.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(meId)) return Results.Unauthorized();
				if (string.IsNullOrWhiteSpace(req.ReceiverUserId)) return Results.BadRequest("ReceiverUserId is required.");
				if (string.IsNullOrWhiteSpace(req.Content)) return Results.BadRequest("Content is required.");
				if (req.ReceiverUserId == meId) return Results.BadRequest("Cannot send a message to yourself.");

				MessageService service = new();
				MessageReply reply = await service.SendMessage(
					new MessageRequest
					{
						SenderId = meId,
						ReceiverId = req.ReceiverUserId,
						Content = req.Content,
					},
					null!
				);

				return Results.Ok(ToDto(reply));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return Results.Problem(ex.Message);
			}
		})
		.WithName("SendMessage");

		return group;
	}

	private static MessageDto ToDto(MessageReply m) => new(
		m.Id,
		m.SenderId,
		m.ReceiverId,
		m.Content,
		m.CreatedDate
	);
}
