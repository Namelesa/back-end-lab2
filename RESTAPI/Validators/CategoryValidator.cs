using FluentValidation;
using RESTAPI.Models;

namespace RESTAPI.Validators;

public class CategoryValidator: AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(category => category.CategoryName).NotEmpty().MaximumLength(30);
    }
}