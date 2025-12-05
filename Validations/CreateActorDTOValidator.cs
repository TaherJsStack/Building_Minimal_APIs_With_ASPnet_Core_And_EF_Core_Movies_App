using FluentValidation;
using Building_MinimalAPIsMoviesApp.DTOs;


namespace Building_MinimalAPIsMoviesApp.Validations
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Custom Not Empty Message Error For {PropertyName}")
                .MaximumLength(150)
                .WithMessage("Custom Maximum Length Message Error For {PropertyName}");


            var minimumDate = new DateTime(1990, 1, 1);

            RuleFor(p => p.DateOfBirth)
                .GreaterThanOrEqualTo(minimumDate)
                .WithMessage("Custom Not Empty Message Error For {PropertyName}");

        }


    }
}
