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
    
    public string Port { get; set; } = string.Empty;
    
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
        options.Port = port ?? string.Empty;
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

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("SMTP username is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("SMTP password is required.");

        RuleFor(x => x.FromAddress)
            .NotEmpty()
            .WithMessage("SMTP from address is required.")
            .EmailAddress()
            .WithMessage("SMTP from address must be a valid email address.");
    }

    private static bool BeValidPort(string port)
    {
        return int.TryParse(port, out int portNumber) && portNumber is >= 1 and <= 65535;
    }
}