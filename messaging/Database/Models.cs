
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Message;
using Models.User;

using System.Reflection;

public class DatabaseContext : DbContext
{
	public DbSet<Users> User { get; set; }
	public DbSet<Messages> Message { get; set; }
	

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		try
		{
			IConfigurationRoot config = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.AddUserSecrets(Assembly.GetExecutingAssembly())
				.Build();

			var connString = $@"Host={config["host"]};
						 Port={config["port"]};
                         Username={config["username"]};
                         Password={config["password"]};
                         Database={config["database"]}";
			optionsBuilder.UseNpgsql(connString);
		}
		catch
		{
			System.Console.WriteLine("No connections");
		}
	}
}

// dotnet ef migrations add [nameofmigrations]
// dotnet ef database update