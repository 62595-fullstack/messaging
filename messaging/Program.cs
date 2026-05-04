using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Services.MessageService;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	Args = args,
	WebRootPath = "../wwwroot"
});

IConfigurationRoot config = new ConfigurationBuilder()
					.AddJsonFile("appsettings.json")
					.AddEnvironmentVariables()
					.AddUserSecrets(Assembly.GetExecutingAssembly())
					.Build();

string programPort = config["programPort"] ?? "";
string host = config["host"] ?? "";

builder.WebHost.UseUrls($"https://{host}:{programPort}");

builder.Services.AddGrpc();
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
					ValidIssuer = $"http://{host}:{programPort}",
					ValidAudience = config["Jwt:Audience"],
					ClockSkew = TimeSpan.Zero,
					ValidIssuers = [
					$"http://{host}:{programPort}"
					],
				};
			});
builder.Services.AddAuthorization();
WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<MessageService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	using (DatabaseContext db = new())
	{
		await db.GetService<IMigrator>().MigrateAsync();
	}
}
else
{
	app.UseHttpsRedirection();
}

app.Run();
