using Bot.Api.Extensions;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddBot(builder.Configuration);

builder.AddLogger();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseBot();

app.UseHttpsRedirection();

app.Run();