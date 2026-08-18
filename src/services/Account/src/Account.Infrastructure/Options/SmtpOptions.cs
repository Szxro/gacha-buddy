using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Account.Infrastructure.Options;

public class SmtpOptions : IOptionsModel
{
    public string SectionName => "SmtpServerOptions";

    public string Host { get; set; } = string.Empty;
    
    public int Port { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;

    public bool UseSsl { get; set; }

    public string FromAddress { get; set; } = string.Empty;
}

[Inject(ServiceKind.Options, optionType: typeof(SmtpOptions))]
public class SmtpOptionsConfigurator : BaseOptions<SmtpOptions>
{
    public SmtpOptionsConfigurator(IConfiguration configuration) : base(configuration) { }

    public override void Configure(SmtpOptions options)
    {
        base.Configure(options);
        
        string? host = Environment.GetEnvironmentVariable("SMTP_HOST");
        
        string? port = Environment.GetEnvironmentVariable("SMTP_PORT");
        
        string? username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
        
        string? password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
        
        string? fromAddress = Environment.GetEnvironmentVariable("SMTP_FROM_ADDRESS");
        
        options.Host = host ?? string.Empty;
        options.Port = int.TryParse(port, out int portNumber)  ? portNumber : 0;
        options.Username = username ?? string.Empty;
        options.Password = password ?? string.Empty;
        options.FromAddress = fromAddress ?? string.Empty;
    }
}

public class SmtpOptionsValidator : AbstractValidator<SmtpOptions>
{
    public SmtpOptionsValidator()
    {
        RuleFor(x => x.Host)
            .NotEmpty()
            .WithMessage("SMTP host is required.");

        RuleFor(x => x.Port)
            .NotEmpty()
            .WithMessage("SMTP port is required.")
            .Must(BeValidPort)
            .WithMessage("SMTP port must be a valid port number.");

        RuleFor(x => x.FromAddress)
            .NotEmpty()
            .WithMessage("SMTP from address is required.")
            .EmailAddress()
            .WithMessage("SMTP from address must be a valid email address.");
    }

    private static bool BeValidPort(int portNumber) => portNumber is >= 1 and <= 65535;
}