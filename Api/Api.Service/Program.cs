using Api.Service;
using Api.Service.Auth;

var builder = WebApplication.CreateBuilder(args);
builder.AddLogging();
builder.RegisterServices();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<TokenAuthMiddleware>();

app.MapControllers();

app.Run();
