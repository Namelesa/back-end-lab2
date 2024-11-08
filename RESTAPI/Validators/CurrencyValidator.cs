using FluentValidation;
using RESTAPI.Models;

namespace RESTAPI.Validators;

public class CurrencyValidator: AbstractValidator<Currency>
{
    public CurrencyValidator()
    {
        RuleFor(currency => currency.Name).NotEmpty().MaximumLength(30);
    }
}