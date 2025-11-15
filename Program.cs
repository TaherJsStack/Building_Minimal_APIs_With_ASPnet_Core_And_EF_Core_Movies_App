using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);


// Services Zone - START

string ConfigName = builder.Configuration.GetValue<string>("ConfigName");

// Services Zone - END

var app = builder.Build();

// Middleware Zone - START

app.MapGet("/", () => ConfigName);

// Middleware Zone - END

app.Run();
