using Building_MinimalAPIsMoviesApp.DTOs;
using FluentValidation;

namespace Building_MinimalAPIsMoviesApp.Validations
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
        }
    }
}
