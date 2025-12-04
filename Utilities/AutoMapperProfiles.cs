using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Utilities
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(p => p.Picture, options => options.Ignore());

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, entity =>
                    entity.MapFrom(p =>
                        p.GenresMovies.Select(gm => 
                            new GenreDTO { Id = gm.GenreId, Name = gm.Genre.Name })))
                .ForMember(m => m.Actors, entity =>
                    entity.MapFrom(a => 
                        a.ActorsMovies.Select( am => 
                            new ActorMovieDTO { 
                                Id = am.ActorId, 
                                Name = am.Actor.Name, 
                                Character = am.Character
                            } 
                         )));

            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(p => p.Poster, options => options.Ignore());

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();

            CreateMap<AssignActorMovieDTO, ActorMovie>();


        }
    }
}
