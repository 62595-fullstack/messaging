using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Comment;
using System.Security.Claims;

namespace Endpoints;

public record CreateCommentRequestDto(int EventId, string Content, int? ParentCommentId);

public record CommentDto(
	int Id,
	int EventId,
	int? ParentCommentId,
	string AuthorUserId,
	string Content,
	string CreatedDate
);

public static class CommentsEndpoint
{
	public static RouteGroupBuilder MapCommentsEndpoints(this RouteGroupBuilder group)
	{
		group.MapGet("/event/{eventId:int}", async Task<IResult> (int eventId) =>
		{
			try
			{
				DatabaseContext db = new();
				List<Comments> rows = await db.Comment
					.Where(c => c.EventId == eventId)
					.OrderBy(c => c.CreatedDate)
					.ToListAsync();

				List<CommentDto> result = rows.Select(ToDto).ToList();
				return Results.Ok(result);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return Results.Problem(ex.Message);
			}
		})
		.WithName("ListEventComments");

		group.MapPost("/", async Task<IResult> ([FromBody] CreateCommentRequestDto req, ClaimsPrincipal user) =>
		{
			try
			{
				string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();
				if (req.EventId <= 0) return Results.BadRequest("EventId is required.");
				if (string.IsNullOrWhiteSpace(req.Content)) return Results.BadRequest("Content is required.");

				DatabaseContext db = new();

				int? parentId = req.ParentCommentId is > 0 ? req.ParentCommentId : null;
				if (parentId.HasValue)
				{
					Comments? parent = await db.Comment.FirstOrDefaultAsync(c => c.Id == parentId.Value);
					if (parent == null) return Results.NotFound("Parent comment not found.");
					if (parent.EventId != req.EventId) return Results.BadRequest("Parent comment belongs to a different event.");
				}

				Comments comment = new()
				{
					EventId = req.EventId,
					ParentCommentId = parentId,
					AuthorUserId = userId,
					Content = req.Content,
				};

				await db.Comment.AddAsync(comment);
				await db.SaveChangesAsync();

				return Results.Ok(ToDto(comment));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return Results.Problem(ex.Message);
			}
		})
		.WithName("CreateComment");

		return group;
	}

	private static CommentDto ToDto(Comments c) => new(
		c.Id,
		c.EventId,
		c.ParentCommentId,
		c.AuthorUserId,
		c.Content,
		c.CreatedDate.ToString("o")
	);
}
