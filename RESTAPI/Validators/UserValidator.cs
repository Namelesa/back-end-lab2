using FluentValidation;
using RESTAPI.Models;

namespace RESTAPI.Validators;

public class UserValidator: AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Name).NotEmpty().MaximumLength(10).Matches(@"^(?=(.*[a-zA-Z]){2})(?=.*\d).*$");
    }
}