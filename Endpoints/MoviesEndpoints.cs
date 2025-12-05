using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Filters;
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
            group.MapPost("/", Create).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>();
            group.MapPut("/{id:int}", Update).DisableAntiforgery().AddEndpointFilter<ValidationFilter<CreateMovieDTO>>();
            group.MapDelete("/{id:int}", Delete);
            group.MapPost("/{id:int}/assignGenres", AssignGernres);
            group.MapPost("/{id:int}/assignActors", AssignActors);
            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetMovies(
            IMoviesRepository repository,
            IMapper mapper,
            int Page = 1,
            int recordsPerPage = 10)
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

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(
            int id,
            IMoviesRepository repository,
            IMapper mapper)
        {
            var movie = await repository.GetById(id);
            if (movie == null)
            {
                return TypedResults.NotFound();
            }
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<List<MovieDTO>>, NotFound>> GetByName(
            string name,
            IMoviesRepository repository,
            IMapper mapper)
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


        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignGernres(
            int id,
            List<int> genresIds,
            IMoviesRepository repository,
            IGenresRepository genresRepository,
            IOutputCacheStore outputCacheStore
        )
        {
            if (!await repository.Exists(id))
            {
                return TypedResults.NoContent();
            }

            var existingGenres = new List<int>();
            if (genresIds.Count != 0)
            {
                existingGenres = await genresRepository.Exists(genresIds);
            }

            if (genresIds.Count != existingGenres.Count)
            {
                var nonExistingGenres = genresIds.Except(existingGenres);
                var nonExistingGenresCSV = string.Join(",", nonExistingGenres);
                return TypedResults.BadRequest($"the genres of id {nonExistingGenresCSV} does not exist. ");
            }

            await repository.Assign(id, existingGenres);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignActors(
            int id,
            List<AssignActorMovieDTO> actorsDTO,
            IMoviesRepository repository,
            IActorsRepository actorRepository,
            IMapper mapper
        )
        {
            if (!await repository.Exists(id))
            {
                return TypedResults.NoContent();
            }

            var existingActors = new List<int>();
            var actorsIds = actorsDTO.Select(actor => actor.ActorId).ToList();

            if (actorsDTO.Count != 0)
            {
                existingActors = await actorRepository.Exist(actorsIds);
            }

            if (existingActors.Count != actorsDTO.Count)
            {
                var nonExistingActors = actorsIds.Except(existingActors);
                var nonExistingActorsCVS = string.Join(",", nonExistingActors);
                return TypedResults.BadRequest($"the actors of id {nonExistingActorsCVS} does not exist. ");
            }

            var actors = mapper.Map<List<ActorMovie>>(actorsDTO);
            await repository.Assign(id, actors);
            return TypedResults.NoContent();
        }


    }
}
