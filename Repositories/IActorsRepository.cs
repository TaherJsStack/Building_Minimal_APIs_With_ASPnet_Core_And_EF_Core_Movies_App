using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public interface IActorsRepository
    {
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
    }
}