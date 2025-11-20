using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Repositories;
using Building_MinimalAPIsMoviesApp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Building_MinimalAPIsMoviesApp.Endpoints
{
    public static class MoviesEndpoints
    {

        private readonly static string _container = "movies";

        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetMovies).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapGet("/GetByName/{name}", GetByName);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetMovies(IMoviesRepository repository, IMapper mapper, int Page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO
            {
                Page = Page,
                RecordesPerPage = recordsPerPage
            };
            var movies = await repository.GetAll(pagination);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository repository, IMapper mapper)
        {
            var movie = await repository.GetById(id);
            if (movie == null)
            {
                return TypedResults.NotFound();
            }
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<List<MovieDTO>>, NotFound>> GetByName(string name, IMoviesRepository repository, IMapper mapper)
        {
            var movies = await repository.GetByName(name);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Created<MovieDTO>> Create(
            [FromForm] CreateMovieDTO createMovieDTO,
            IMoviesRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IFileStorage fileStorage
            )
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Store(_container, createMovieDTO.Poster);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"/movies/{id}", movieDTO);

        }

        static async Task<Results<NoContent, NotFound>> Update(
            int id,
            [FromForm] CreateMovieDTO createMovieDTO,
            IMoviesRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IFileStorage fileStorage
            )
        {

            var movieDB = await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movieDB.Poster;

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, _container, createMovieDTO.Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NotFound, NoContent>> Delete(
            int id,
            IMoviesRepository repository,
            IOutputCacheStore outputCacheStore,
            IFileStorage fileStorage)
        {
            var movie = await repository.GetById(id);
            if (movie is null)
            {
                return TypedResults.NotFound();
            }
            await repository.Delete(id);
            await fileStorage.Delete(movie.Poster, _container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }




    }
}
