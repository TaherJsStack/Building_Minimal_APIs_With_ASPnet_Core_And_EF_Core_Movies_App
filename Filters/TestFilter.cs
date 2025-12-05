
using AutoMapper;
using Building_MinimalAPIsMoviesApp.Repositories;

namespace Building_MinimalAPIsMoviesApp.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
            var param2 = context.Arguments.OfType<IGenresRepository>().FirstOrDefault();
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            //this is the code that will execute Before the endpoint
            var results = await next(context);
            //this is the code that will execute After the endpoint

            return results;
        }
    }
}
