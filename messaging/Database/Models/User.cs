using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;

namespace Models.User;

public class Users : IdentityUser
{
	[MaxLength(128)]
	public required string FirstName { get; set; }

	[MaxLength(128)]
	public required string LastName { get; set; }

	[Key]
	public override string? Email { get; set; }



}