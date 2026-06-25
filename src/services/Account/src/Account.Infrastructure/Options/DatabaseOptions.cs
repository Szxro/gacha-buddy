using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Account.Infrastructure.Options;

public sealed class DatabaseOptions : IOptionsModel
{
    public string SectionName => "DatabaseOptions";
    
    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeout { get; set; }
}

[Inject(ServiceKind.Options, optionType: typeof(DatabaseOptions))]
public class DatabaseOptionsConfigurator : BaseOptions<DatabaseOptions>
{
    public DatabaseOptionsConfigurator(IConfiguration configuration) : base(configuration) { }

    public override void Configure(DatabaseOptions options)
    {
        base.Configure(options);

        string? connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        options.ConnectionString = connectionString ?? string.Empty;
    }
}

public class DatabaseOptionsValidator : AbstractValidator<DatabaseOptions>
{
    public DatabaseOptionsValidator()
    {
        RuleFor(options => options.ConnectionString)
            .NotNull().WithMessage("{PropertyName} can't be null")
            .NotEmpty().WithMessage("{PropertyName} can't be empty");
        
        RuleFor(options => options.CommandTimeout)
            .NotNull().WithMessage("{PropertyName} can't be null")
            .NotEmpty().WithMessage("{PropertyName} can't be empty")
            .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0 to not create infinite wait time");;
    }
}