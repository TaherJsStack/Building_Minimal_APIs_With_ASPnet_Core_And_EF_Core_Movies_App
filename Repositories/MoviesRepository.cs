using AutoMapper;
using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public MoviesRepository(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<Movie>> GetAll(PaginationDTO paginationDTO)
        {
            var queryable = _context.Movies.AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertpaginationparametersInResponseHeader(queryable);
            return await queryable.OrderBy(movie => movie.Title).Pagination(paginationDTO).ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context
                            .Movies
                            .Include(movie => movie.Comments)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(movie => movie.Id == id);
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

        public async Task Assign(int id, List<int> genresIds) 
        { 
            var movie = await _context.Movies.Include(m => m.GenresMovies).FirstOrDefaultAsync(movie => movie.Id == id);
            if (movie == null) 
            {
                throw new ArgumentNullException($"there's no movie with id: { id }");
            }
            var genresMovies = genresIds.Select(genreId => new GenreMovie { GenreId = genreId });
            movie.GenresMovies = _mapper.Map(genresMovies, movie.GenresMovies);
            await _context.SaveChangesAsync();
        }

        public async Task Assign(int id, List<ActorMovie> actors)
        {
            for (int i = 1; i < actors.Count; i++)
            {
                actors[i - 1].Order = i;
            }
            var movie = await _context.Movies.Include(movie => movie.ActorsMovies)
                                             .FirstOrDefaultAsync(movie => movie.Id == id);

            if (movie is null) 
            {
                throw new ArgumentNullException($"there's no movie with id: { id }");
            }
        
            movie.ActorsMovies = _mapper.Map(actors, movie.ActorsMovies);

            await _context.SaveChangesAsync();
        }

    }
}
