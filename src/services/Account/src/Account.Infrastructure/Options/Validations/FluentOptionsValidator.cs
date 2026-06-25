using Account.Infrastructure.Options.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Account.Infrastructure.Options.Validations;

public class FluentOptionsValidator<TOption> : IValidateOptions<TOption>
    where TOption : class, IOptionsModel
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public FluentOptionsValidator(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    
    public ValidateOptionsResult Validate(string? name, TOption options)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        IValidator<TOption> validator = scope.ServiceProvider.GetRequiredService<IValidator<TOption>>();

        ValidationResult result = validator.Validate(options);
        
        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        List<string> errors = result.Errors
            .Select(x => $"Validation failed to {x.PropertyName} with the Error Message {x.ErrorMessage}")
            .ToList();

        return ValidateOptionsResult.Fail(errors);
    }
}