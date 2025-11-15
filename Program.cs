using Building_Minimal_APIs_With_ASPnet_Core_And_EF_Core_Movies_App.Entities;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Services Zone - START

//string ConfigName = builder.Configuration.GetValue<string>("ConfigName");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration
        .WithOrigins(builder.Configuration["AllowedOrigins"]!)
        .AllowAnyMethod()
        .AllowAnyHeader();
    });

    options.AddPolicy("free", configurations =>
    {
        configurations.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

});


// Services Zone - END

var app = builder.Build();

// Middleware Zone - START

app.UseCors();

app.MapGet("/", () => "ConfigName");

app.MapGet("/genres", [EnableCors(policyName:"free")] () =>
{
    var GenresList = new List<Genre>
    {
        new Genre
        {
            Id = 1,
            Name = "Drama"
        },
        new Genre
        {
            Id = 2,
            Name = "Action"
        },
        new Genre
        {
            Id = 3,
            Name = "Comedy"
        }
    };

    return GenresList;
});



// Middleware Zone - END

app.Run();
