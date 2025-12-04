using Building_MinimalAPIsMoviesApp.DTOs;
using FluentValidation;

namespace Building_MinimalAPIsMoviesApp.Validations
{
    public class CreateGenreDTOValidator: AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Custom Not Empty Message Error For {PropertyName}")
                .MaximumLength(150)
                .WithMessage("Custom Maximum Length Message Error For {PropertyName}")
                .Must(FirstLitterIsUppercase)
                .WithMessage("First Litter Is Uppercase Error Message")
                ;
        }

        private bool FirstLitterIsUppercase(string value) 
        { 
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            };

            var firestLitter = value[0].ToString();
            return firestLitter == firestLitter.ToUpper();
        }
    }
}
