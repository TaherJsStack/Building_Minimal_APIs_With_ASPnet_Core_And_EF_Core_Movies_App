using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp.Repositories
{
    public static class HttpContextExtensions
    {
        public async static Task InsertpaginationparametersInResponseHeader<T>
            ( this HttpContext httpContext, IQueryable<T> queryable ) 
        { 
            if ( httpContext is null )
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double count = await queryable.CountAsync();
            httpContext.Response.Headers.Append("TotalAmountOfRecordes", count.ToString() );
        }

    }
}
