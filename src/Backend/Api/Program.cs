using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();
builder.AddExceptionHandler();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
