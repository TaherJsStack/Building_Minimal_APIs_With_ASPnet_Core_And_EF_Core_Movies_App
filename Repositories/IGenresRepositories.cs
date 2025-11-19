using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public interface IGenresRepositories
    {
        Task<int> Create(Genre genre);

    }
}
