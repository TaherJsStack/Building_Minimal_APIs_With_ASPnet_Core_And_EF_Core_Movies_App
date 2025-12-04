using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<int> genresIds);
        Task Assign(int id, List<ActorMovie> actors);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Movie>> GetAll(PaginationDTO paginationDTO);
        Task<Movie?> GetById(int id);
        Task<List<Movie>> GetByName(string name);
        Task Update(Movie movie);
    }
}