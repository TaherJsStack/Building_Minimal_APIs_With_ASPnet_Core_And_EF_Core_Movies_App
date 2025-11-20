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
    public static class ActorsEndpoints
    {

        private readonly static string _container = "actors";

        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetActors).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actors-get"));

            group.MapGet("/{id:int}", GetById);

            group.MapGet("/GetByName/{name}", GetByName);

            group.MapPost("/", Create).DisableAntiforgery();
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> GetActors(IActorsRepository repository, IMapper mapper)
        {
            var actors = await repository.GetAll();
            var actorDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorsRepository repository, IMapper mapper)
        {
            var actor = await repository.GetById(id);
            if (actor == null)
            {
                return TypedResults.NotFound();
            }
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Results<Ok<List<ActorDTO>>, NotFound>> GetByName(string name, IActorsRepository repository, IMapper mapper)
        {
            var actors = await repository.GetByName(name);
            var actorDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorDTO);
        }


        static async Task<Created<ActorDTO>> Create(
            [FromForm] CreateActorDTO createActorDTO,
            IActorsRepository repository,
            IOutputCacheStore outputCacheStore,
            IMapper mapper,
            IFileStorage fileStorage
            ) 
        {
            var actor = mapper.Map<Actor>(createActorDTO);

            if (createActorDTO.Picture is not null) 
            { 
                var url =  await fileStorage.Store(_container, createActorDTO.Picture);
                actor.Picture = url;
            }

            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDTO);

        }



    }
}
