using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Filters;
using Building_MinimalAPIsMoviesApp.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace Building_MinimalAPIsMoviesApp.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

            group.MapGet("/{id:int}", GetById).AddEndpointFilter<TestFilter>();

            group.MapPost("/", Create).AddEndpointFilter<GenresValidationFilter>();

            group.MapPut("/{id:int}", Update).AddEndpointFilter<GenresValidationFilter>();

            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository, IMapper mapper)
        {
            var genres = await repository.GetAll();
            //var genreDTO = genres.Select(genre => new GenreDTO { Id = genre.Id, Name = genre.Name }).ToList();
            var genreDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genreDTO);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenresRepository repository, IMapper mapper)
        {
            var genre = await repository.GetById(id);

            if (genre == null)
            {
                return TypedResults.NotFound();
            }

            //var genreDTO = new GenreDTO 
            //{
            //    Id = genre.Id,
            //    Name = genre.Name,
            //};

            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Ok(genreDTO);
        }

        static async Task<Results<Created<GenreDTO>, ValidationProblem>> Create(
            CreateGenreDTO createGenreDTO,
            IGenresRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IValidator<CreateGenreDTO> validator
            )
        {

            var validatorResult = await validator.ValidateAsync(createGenreDTO);

            if (!validatorResult.IsValid)
            {
                return TypedResults.ValidationProblem(validatorResult.ToDictionary());
            }

            //var genre = new Genre
            //{
            //    Name = createGenreDTO.Name,
            //};
            var genre = mapper.Map<Genre>(createGenreDTO);
            var id = await repository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            //var genreDTO = new GenreDTO
            //{
            //    Id = genre.Id,
            //    Name = genre.Name,
            //};
            var genreDTO = mapper.Map<GenreDTO>(genre);
            return TypedResults.Created($"/geners/{id}", genreDTO);
        }

        static async Task<Results<NotFound, NoContent, ValidationProblem>> Update(
            int id,
            CreateGenreDTO createGenreDTO,
            IGenresRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IValidator<CreateGenreDTO> validator
            )
        {
            var validatorResult = await validator.ValidateAsync(createGenreDTO);

            if (!validatorResult.IsValid)
            {
                return TypedResults.ValidationProblem(validatorResult.ToDictionary());
            }

            var exists = await repository.Exists(id);
            if (!exists)
            {
                return TypedResults.NotFound();
            }
            //var genre = new Genre
            //{
            //    Id = id,
            //    Name = createGenreDTO.Name,
            //};
            var genre = mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(
            int id,
            IGenresRepository repository,
            IOutputCacheStore outputCacheStore)
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

    }
}
