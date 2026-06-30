using Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
