using Building_MinimalAPIsMoviesApp.DTOs;
using FluentValidation;

namespace Building_MinimalAPIsMoviesApp.Validations
{
    public class CreateCommentDTOValidator: AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator() 
        {
            RuleFor(p => p.Body)
                .NotEmpty()
                .WithMessage(ValidationUtilities.NotEmptyMessage)
                .MaximumLength(350)
                .WithMessage(ValidationUtilities.MaximumLengthMessage);

        }
    }
}
