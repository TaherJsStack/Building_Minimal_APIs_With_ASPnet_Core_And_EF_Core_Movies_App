using Building_MinimalAPIsMoviesApp;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Microsoft.AspNetCore.Cors;
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

app.MapGet("/genres", [EnableCors(policyName:"free")] async (IGenresRepositories repository) =>
{
    return await repository.GetAll();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

app.MapGet("/geners/{id:int}", [EnableCors(policyName: "free")] async (int id, IGenresRepositories repository) =>
{
    var genre = await repository.GetById(id); 
    
    if (genre == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genre); 
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(10)));


app.MapPost("/genres", async (Genre genre, IGenresRepositories repository, IOutputCacheStore outputCacheStore) =>
{
    var id = await repository.Create(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.Created($"/geners/{id}", genre);
});

app.MapPut("/genres/{id}", async (int id, Genre genre, IGenresRepositories repository, IOutputCacheStore outputCacheStore) =>
{
    var exists = await repository.Exists(id);
    if (!exists)
    {
        return Results.NotFound();
    }
    await repository.Update(genre);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    //return Results.Ok(genre);
    return Results.NoContent();
});

app.MapDelete("/genres/{id:int}", async (int id, IGenresRepositories repository, IOutputCacheStore outputCacheStore) =>
{
    var exists = await repository.Exists(id);
    if (!exists)
    {
        return Results.NotFound();
    }
    await repository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});


// Middleware Zone - END

app.Run();
