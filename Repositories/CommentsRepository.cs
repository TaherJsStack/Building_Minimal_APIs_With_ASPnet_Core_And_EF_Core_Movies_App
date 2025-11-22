using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentsRepository(ApplicationDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Comment>> GetAll(int moviId, PaginationDTO paginationDTO)
        {
            var queryable = _context.Comments.AsQueryable();
            await _httpContextAccessor.HttpContext!.InsertpaginationparametersInResponseHeader(queryable);
            return await queryable
                            .Where(comment => comment.MovieId == moviId)
                            .OrderBy(comment => comment.Id)
                            .Pagination(paginationDTO)
                            .ToListAsync();
        }

        public async Task<Comment?> GetById(int id)
        {
            return await _context.Comments.AsNoTracking().FirstOrDefaultAsync(comment => comment.Id == id);
        }

        public async Task<int> Create(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment.Id;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Comments.AnyAsync(comment => comment.Id == id);
        }

        public async Task Update(Comment comment)
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await _context.Comments.Where(comment => comment.Id == id).ExecuteDeleteAsync();
        }
    }
}
