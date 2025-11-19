using Building_MinimalAPIsMoviesApp;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Services Zone - START

//string ConfigName = builder.Configuration.GetValue<string>("ConfigName");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer("name=DBConnection");
});

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

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenresRepositories, GenresRepository>();

// Services Zone - END

var app = builder.Build();

// Middleware Zone - START

//if (builder.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(c => {});
//}

app.UseCors();

app.UseOutputCache();

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
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)));

app.MapPost("/Genres", async (Genre genre, IGenresRepositories repository) =>
{
    var id = await repository.Create(genre);
    return Results.Created($"/geners/{id}", genre);
});

// Middleware Zone - END

app.Run();
