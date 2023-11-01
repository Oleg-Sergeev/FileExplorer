using System.Net;
using FileExplorer.Models;
using FluentValidation;

namespace FileExplorer.Filters;

public class LogInIsValidFilter : IEndpointFilter
{
    private readonly IValidator<LogIn> _validator;

    public LogInIsValidFilter(IValidator<LogIn> validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var signup = context.GetArgument<LogIn>(1);

        var res = _validator.Validate(signup);

        if (!res.IsValid)
        {
            return Results.ValidationProblem(res.ToDictionary(), statusCode: (int)HttpStatusCode.BadRequest);
        }

        return await next(context);
    }
}