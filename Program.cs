using Building_MinimalAPIsMoviesApp;
using Building_MinimalAPIsMoviesApp.Endpoints;
using Building_MinimalAPIsMoviesApp.Repositories;
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

app.MapGroup("/genres").MapGenres();

// Middleware Zone - END

app.Run();

