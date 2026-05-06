using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Models.Message;
using Services.MessageService;
using Services.MessageStream;
using System.Security.Claims;
using System.Text.Json;

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
	private static readonly JsonSerializerOptions _streamJsonOptions = new(JsonSerializerDefaults.Web);

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

				MessageDto dto = ToDto(reply);
				MessageStream.Publish(reply.ReceiverId, dto);
				return Results.Ok(dto);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return Results.Problem(ex.Message);
			}
		})
		.WithName("SendMessage");

		group.MapGet("/stream", async (HttpContext http, ClaimsPrincipal user, CancellationToken ct) =>
		{
			string? meId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (string.IsNullOrEmpty(meId))
			{
				http.Response.StatusCode = StatusCodes.Status401Unauthorized;
				return;
			}

			http.Response.Headers.Append("Content-Type", "text/event-stream");
			http.Response.Headers.Append("Cache-Control", "no-cache");
			http.Response.Headers.Append("X-Accel-Buffering", "no");

			(Guid connectionId, var reader) = MessageStream.Subscribe(meId);

			try
			{
				await http.Response.WriteAsync(": connected\n\n", ct);
				await http.Response.Body.FlushAsync(ct);

				await foreach (object payload in reader.ReadAllAsync(ct))
				{
					string json = JsonSerializer.Serialize(payload, _streamJsonOptions);
					await http.Response.WriteAsync($"data: {json}\n\n", ct);
					await http.Response.Body.FlushAsync(ct);
				}
			}
			catch (OperationCanceledException) { }
			finally
			{
				MessageStream.Unsubscribe(meId, connectionId);
			}
		})
		.WithName("StreamMessages");

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
