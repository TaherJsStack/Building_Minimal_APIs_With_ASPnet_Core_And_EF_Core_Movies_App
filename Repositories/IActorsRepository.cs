using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task<List<Actor>> GetByName(string name);
        Task Update(Actor actor);
    }
}