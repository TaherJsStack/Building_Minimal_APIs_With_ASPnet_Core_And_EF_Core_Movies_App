using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MoviesRepository(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = _context.Movies.AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertpaginationparametersInResponseHeader(queryable);
            return await queryable.OrderBy(movie => movie.Title).Pagination(paginationDTO).ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context.Movies.AsNoTracking().FirstOrDefaultAsync(movie => movie.Id == id);
        }

        public async Task<List<Movie>> GetByName(string name)
        {
            return await _context.Movies
                .Where(movie => movie.Title.Contains(name))
                .OrderBy(movie => movie.Title)
                .ToListAsync();
        }

        public async Task<int> Create(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie.Id;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Movies.AnyAsync(movie => movie.Id == id);
        }

        public async Task Update(Movie movie)
        {
            _context.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Movies.Where(movie => movie.Id == id).ExecuteDeleteAsync();
        }

    }
}
