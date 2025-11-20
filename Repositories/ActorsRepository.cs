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
            return await _context.Actors.AsNoTracking().FirstOrDefaultAsync(actor => actor.Id == id);
        }

        public async Task<List<Actor>> GetByName(string name)
        {
            return await _context.Actors
                .Where(actor => actor.Name.Contains(name))
                .OrderBy(actor => actor.Name)
                .ToListAsync();
        }

        public async Task<int> Create(Actor actor) 
        { 
            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Exist(int id)
        { 
            return await _context.Actors.AnyAsync(actor => actor.Id == id);
        }

        public async Task Update(Actor actor) 
        {
            _context.Update(actor);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id) 
        {
            await _context.Actors.Where(actor => actor.Id == id).ExecuteDeleteAsync();
        }


    }
}
