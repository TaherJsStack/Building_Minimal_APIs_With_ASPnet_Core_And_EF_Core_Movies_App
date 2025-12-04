using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActorsRepository(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Actor>> GetAll(PaginationDTO paginationDTO)
        {

            var queryable = _context.Actors.AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertpaginationparametersInResponseHeader(queryable);
            return await queryable.OrderBy(actor => actor.Name).Pagination(paginationDTO).ToListAsync();
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

        public async Task<List<int>> Exist(List<int> ids)
        {
            return await _context.Actors
                                 .Where(actor => ids.Contains(actor.Id))
                                 .Select(actor => actor.Id)
                                 .ToListAsync();
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
