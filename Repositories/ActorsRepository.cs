using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly ApplicationDBContext _context;

        public ActorsRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<Actor>> GetAll()
        {
            return await _context.Actors.OrderBy(actor => actor.Name).ToListAsync();
        }

        public async Task<Actor?> GetById(int id)
        {
            return await _context.Actors.FirstOrDefaultAsync(actor => actor.Id == id);
        }

    }
}
