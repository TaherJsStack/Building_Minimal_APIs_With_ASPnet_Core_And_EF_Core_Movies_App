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
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById).WithName("CommentsById");
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>();
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>();
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
            var comments = await repository.GetAll(movieId, pagination);
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

        //static async Task<Results<Created<CommentDTO>, NotFound>> Create(
        static async Task<Results<CreatedAtRoute<CommentDTO>, NotFound>> Create(
            int movieId,
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

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieId;

            var id = await repository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comment);
            //return TypedResults.Created($"/comments/{id}", commentDTO);
            return TypedResults.CreatedAtRoute(commentDTO, "CommentsById", new { id, movieId });

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
            var commentDB = await repository.GetById(id);
            if (commentDB is null)
            {
                return TypedResults.NotFound();
            }

            var commentForUpdate = mapper.Map<Comment>(createCommentDTO);
            commentForUpdate.Id = id;
            commentForUpdate.MovieId = movieId;

            await repository.Update(commentForUpdate);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NotFound, NoContent>> Delete(
            int movieId,
            int id,
            ICommentsRepository repository,
            IMoviesRepository moviesRepository,
            IOutputCacheStore outputCacheStore,
            IFileStorage fileStorage)
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
            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }


    }
}
