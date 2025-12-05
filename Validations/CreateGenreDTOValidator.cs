using Building_MinimalAPIsMoviesApp.DTOs;
using Building_MinimalAPIsMoviesApp.Repositories;
using FluentValidation;

namespace Building_MinimalAPIsMoviesApp.Validations
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenresRepository genresRepository, IHttpContextAccessor httpContextAccessor)
        {
            var routeValueId = httpContextAccessor?.HttpContext?.Request.RouteValues["id"];
            var id = 0;

            if (routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id);
            }

            RuleFor(p => p.Name)
                .NotEmpty()
                    .WithMessage(ValidationUtilities.NotEmptyMessage)
                .MaximumLength(150)
                    .WithMessage(ValidationUtilities.MaximumLengthMessage)
                .Must(ValidationUtilities.FirstLitterIsUppercase)
                    .WithMessage(ValidationUtilities.FirstLitter)
                .MustAsync(async (name, _) =>
                {
                    var exists = await genresRepository.Exists(id, name);
                    return !exists;
                })
                  .WithMessage(g => ValidationUtilities.ExistsMessage(g.Name));
        }


    }
}
