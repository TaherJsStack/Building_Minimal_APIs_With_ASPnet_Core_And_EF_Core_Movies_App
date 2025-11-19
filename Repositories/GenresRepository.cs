using Building_MinimalAPIsMoviesApp.Entities;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class GenresRepository : IGenresRepositories
    {
        private readonly ApplicationDBContext _context;

        public GenresRepository(ApplicationDBContext context) 
        {
            _context = context;
        }

        public async Task<int> Create(Genre genre)
        {
            _context.Add(genre);
            await _context.SaveChangesAsync();
            return genre.Id;
        }
    }
}
