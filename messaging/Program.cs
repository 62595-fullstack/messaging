using Endpoints;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
using Services.CommentService;
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
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                };
            });
builder.Services.AddAuthorization();
WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<CommentService>();
app.MapGroup("/Comments")
    .RequireAuthorization()
    .MapCommentsEndpoints();
app.MapGroup("/Messages")
    .RequireAuthorization()
    .MapMessagesEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await using DatabaseContext db = new();
    await db.GetService<IMigrator>().MigrateAsync();
}
else
{
    app.UseHttpsRedirection();
}

app.Run();
