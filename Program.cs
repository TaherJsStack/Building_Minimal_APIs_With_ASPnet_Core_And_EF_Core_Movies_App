using Building_MinimalAPIsMoviesApp;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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

var genresEndpoints = app.MapGroup("/genres");

genresEndpoints.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

genresEndpoints.MapGet("/{id:int}", GetById);

genresEndpoints.MapPost("/", Create);

genresEndpoints.MapPut("/{id:int}", Update);

genresEndpoints.MapDelete("/{id:int}", Delete);


// Middleware Zone - END

app.Run();

static async Task<Ok<List<Genre>>> GetGenres(IGenresRepositories repository)
{
    var genres = await repository.GetAll(); ;
    return TypedResults.Ok(genres); 
}

static async Task<Results<Ok<Genre>, NotFound>> GetById(int id, IGenresRepositories repository)
{
    var genre = await repository.GetById(id);

    if (genre == null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(genre);
}

static async Task<Created<Genre>> Create(Genre genre, IGenresRepositories repository, IOutputCacheStore outputCacheStore)
{
    var id = await repository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.Created($"/geners/{id}", genre);
}

static async Task<Results<NotFound, NoContent>> Update(int id, Genre genre, IGenresRepositories repository, IOutputCacheStore outputCacheStore)
{
    var exists = await repository.Exists(id);
    if (!exists)
    {
        return TypedResults.NotFound();
    }
    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}

static async Task<Results<NotFound, NoContent>> Delete(int id, IGenresRepositories repository, IOutputCacheStore outputCacheStore)
{
    var exists = await repository.Exists(id);
    if (!exists)
    {
        return TypedResults.NotFound();
    }
    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}