using Building_MinimalAPIsMoviesApp;
using Building_MinimalAPIsMoviesApp.Endpoints;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Building_MinimalAPIsMoviesApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

builder.Services.AddScoped<IGenresRepository, GenresRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorsRepository, ErrorsRepository>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

// Services Zone - END

var app = builder.Build();

// Middleware Zone - START

//if (builder.Environment.IsDevelopment()) {
app.UseSwagger();
app.UseSwaggerUI(c => { });
//}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{

    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorsRepository>();
    await repository.Create(error);

    await Results.BadRequest(new
    {
        type = "Error",
        Message = "an unexpected exception has occurrd.",
        status = 500
    }).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapGet("/", () => "ConfigName");
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("Example Error Message");
});

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movie/{movieId:int}/comments").MapComments();

// Middleware Zone - END

app.Run();

