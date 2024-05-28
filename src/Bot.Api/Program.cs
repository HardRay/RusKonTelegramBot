using Bot.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddLogger();

builder.Services.AddAppServices(builder.Configuration);

var app = builder.Build();

app.UseBot();

app.UseHttpsRedirection();

app.Run();