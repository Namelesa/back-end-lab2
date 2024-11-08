using FluentValidation;

namespace RESTAPI.Validators;

public class StringParamValidator: AbstractValidator<string>
{
    public StringParamValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Param is required.")
            .MinimumLength(3).WithMessage("Param must be at least 3 characters long.")
            .MaximumLength(10).WithMessage("The parameter must contain up to 10 characters.");
    }
}