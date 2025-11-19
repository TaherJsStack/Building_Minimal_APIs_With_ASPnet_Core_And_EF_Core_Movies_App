using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;

namespace Building_MinimalAPIsMoviesApp.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

            group.MapGet("/{id:int}", GetById);

            group.MapPost("/", Create);

            group.MapPut("/{id:int}", Update);

            group.MapDelete("/{id:int}", Delete);

            return group;
        }

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

    }
}
