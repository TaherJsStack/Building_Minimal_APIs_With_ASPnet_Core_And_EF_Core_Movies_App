using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public interface IGenresRepositories
    {
        Task<int> Create(Genre genre);
        Task<Genre?> GetById(int id);
        Task<List<Genre>> GetAll();
        Task<bool> Exists(int id);
        Task Update(Genre genre);
    }
}
