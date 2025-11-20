using Building_MinimalAPIsMoviesApp.DTOs;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public static class IQueryableExtensions
    {

        public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, PaginationDTO paginationDTO)
        {
            return queryable
                .Skip((paginationDTO.Page - 1) * paginationDTO.RecordesPerPage)
                .Take(paginationDTO.RecordesPerPage);
        }

    }
}
