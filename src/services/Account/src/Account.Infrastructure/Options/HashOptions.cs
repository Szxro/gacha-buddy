using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Account.Infrastructure.Options;

public class HashOptions : IOptionsModel
{
    public string SectionName => "HashOptions";
    
    public int SaltSize { get; set; }

    public int HashSize { get; set; }

    public int Iterations { get; set; }
}

[Inject(ServiceKind.Options, optionType: typeof(HashOptions))]
public class HashOptionsConfigurator : BaseOptions<HashOptions>
{
    public HashOptionsConfigurator(IConfiguration configuration) : base(configuration)
    {
    }
}

public class HashOptionsValidations : AbstractValidator<HashOptions>
{
    public HashOptionsValidations()
    {
        RuleFor(x => x.Iterations)
            .NotEmpty().WithMessage("The {PropertyName} can't empty")
            .NotNull().WithMessage("The {PropertyName} can't null")
            .GreaterThan(10000).WithMessage("The {PropertyName} must be greater than 10000");

        RuleFor(x => x.SaltSize)
            .NotEmpty().WithMessage("The {PropertyName} can't empty")
            .NotNull().WithMessage("The {PropertyName} can't null")
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0");

        RuleFor(x => x.HashSize)
            .NotEmpty().WithMessage("The {PropertyName} can't empty")
            .NotNull().WithMessage("The {PropertyName} can't null")
            .GreaterThan(0).WithMessage("The {PropertyName} must be greater than 0");
    }
}
