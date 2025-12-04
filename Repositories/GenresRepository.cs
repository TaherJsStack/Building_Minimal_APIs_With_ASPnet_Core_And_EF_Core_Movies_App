using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class GenresRepository : IGenresRepository
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

        public async Task<List<Genre>> GetAll()
        {
            return await _context.Genres.OrderBy(genre => genre.Name).ToListAsync();
            //return await _context.Genres.OrderByDescending(genre => genre.Name).ToListAsync();
        }


        public async Task<Genre?> GetById(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(genre => genre.Id == id);
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Genres.AnyAsync(genre => genre.Id == id);
        }

        public async Task<List<int>> Exists(List<int> ids) 
        {
            return await _context
                .Genres
                .Where(genre => ids.Contains(genre.Id))
                .Select(gerne => gerne.Id)
                .ToListAsync();
        }

        public async Task Update(Genre genre)
        {
            _context.Update(genre);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Genres.Where<Genre>(genre => genre.Id == id).ExecuteDeleteAsync();
        }
    }
}
