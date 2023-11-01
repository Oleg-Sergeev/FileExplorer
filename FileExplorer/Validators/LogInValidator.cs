using FileExplorer.Models;
using FluentValidation;

namespace FileExplorer.Validators;

public class LogInValidator : AbstractValidator<LogIn>
{
    public LogInValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}