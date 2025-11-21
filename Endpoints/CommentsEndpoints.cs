using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Building_MinimalAPIsMoviesApp.Migrations;
using Building_MinimalAPIsMoviesApp.Repositories;
using Building_MinimalAPIsMoviesApp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Building_MinimalAPIsMoviesApp.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery();
            group.MapPut("/{id:int}", Update).DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(
            int movieId,
            ICommentsRepository repository,
            IMoviesRepository moviesRepository,
            IMapper mapper, 
            int Page = 1, 
            int recordsPerPage = 10)
        {

            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var pagination = new PaginationDTO
            {
                Page = Page,
                RecordesPerPage = recordsPerPage
            };
            var comments = await repository.GetAll(movieId,pagination);
            var commentDTO = mapper.Map<List<CommentDTO>>(comments);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(
            int movieId,
            int id, 
            ICommentsRepository repository,
            IMoviesRepository moviesRepository,
            IMapper mapper)
        {
            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = await repository.GetById(id);
            if (comment is null)
            {
                return TypedResults.NotFound();
            }
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Created<CommentDTO>, NotFound>> Create(
            int movieId,
            CreateCommentDTO createCommentDTO,
            ICommentsRepository repository,
            IMoviesRepository moviesRepository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper
            )
        {

            if (! await moviesRepository.Exists(movieId)) 
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieId;

            var id = await repository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comments/{id}", commentDTO);

        }

        static async Task<Results<NoContent, NotFound>> Update(
            int movieId,
            int id,
            CreateCommentDTO createCommentDTO,
            ICommentsRepository repository,
            IMoviesRepository moviesRepository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper
        )
        {

            if (!await moviesRepository.Exists(movieId))
            {
                return TypedResults.NotFound();
            }
            var movieDB = await repository.GetById(id);
            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Comment>(createCommentDTO);
            movieForUpdate.Id = id;
            movieForUpdate.MovieId = movieId;

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NotFound, NoContent>> Delete(
            int id,
            ICommentsRepository repository,
            IOutputCacheStore outputCacheStore,
            IFileStorage fileStorage)
        {
            var comment = await repository.GetById(id);
            if (comment is null)
            {
                return TypedResults.NotFound();
            }
            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }


    }
}
