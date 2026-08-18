using Account.Application.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options;
using Account.SharedKernel.Common.Messaging;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Abstractions;
using MimeKit;

namespace Account.Infrastructure.Messaging.Email;

[Inject(serviceKind:ServiceKind.Service, ServiceLifetime.Scoped)]
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpOptions _smtpOptions;
    
    private static readonly string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
    private static readonly Random rng = new Random(); // static instance to avoid re-seeding
    
    public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> logger)
    {
        _logger = logger;
        _smtpOptions = options.Value;
    }

    public string GenerateCode(int length = 10)
    {
        char[] buffer = new char[length];

        for (int i = 0; i < length; i++)
        {
            buffer[i] = Chars[rng.Next(Chars.Length)];
        }

        return new string(buffer);
    }

    public async Task SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        using SmtpClient client = new SmtpClient();

        MimeMessage message = CreateMimeMessage(emailMessage);

        // client events
        client.Authenticated += OnAuthenticated!;

        client.Connected += OnConnected!;

        client.Disconnected += OnDisconnected!;

        try
        {
            await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, cancellationToken: cancellationToken);

            if (!string.IsNullOrEmpty(_smtpOptions.Username) && !string.IsNullOrEmpty(_smtpOptions.Password))
            {
                await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password, cancellationToken);
            }
            
            await client.SendAsync(message, cancellationToken);

            _logger.LogInformation(
                "Email sent to '{toAddress}' with subject '{subject}'",
                emailMessage.ToAddress,
                emailMessage.Subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An unexpected error happen while trying to send an email to {toAddress} with the error message: {message}",
                emailMessage.ToAddress,
                ex.Message);

            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
    
    
    private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
    {
        MimeMessage mimeMessage = new MimeMessage();

        mimeMessage.Subject = emailMessage.Subject;

        mimeMessage.From.Add(MailboxAddress.Parse(_smtpOptions.FromAddress));

        mimeMessage.To.Add(MailboxAddress.Parse(emailMessage.ToAddress));

        BodyBuilder builder = new BodyBuilder { HtmlBody = emailMessage.Body };
        
        // Add the logo always if the template required it
        string assemblyPath = Path.GetDirectoryName(
            typeof(EmailTemplateRenderer).Assembly.Location)!;
        
        string logoPath = Path.Combine(
            assemblyPath,
            "Messaging",
            "Email",
            "Templates",
            "logo.png");
        
        // TODO: Add the logo to he template if its required
        if (File.Exists(logoPath) && emailMessage.Body.Contains("cid:logoCid"))
        {
            MimeEntity image = builder.LinkedResources.Add(logoPath);
            image.ContentId = "logoCid";
            image.ContentDisposition = new ContentDisposition(ContentDisposition.Inline);
        }
        
        // Logic to add the attachments to the email
        if (emailMessage.Attachments is not null &&  emailMessage.Attachments.Any())
        {
            foreach (EmailAttachment attachment in emailMessage.Attachments)
            {
                builder.Attachments.Add(
                    attachment.FileName, 
                    attachment.Content,
                    ContentType.Parse(attachment.ContentType));
            }
        }
        
        mimeMessage.Body = builder.ToMessageBody();

        return mimeMessage;
    }
    
    private void OnAuthenticated(object sender, AuthenticatedEventArgs args)
    {
        _logger.LogInformation(
            "SMTP Authentication: '{Message}'",
            args.Message);
    }

    private void OnConnected(object sender, ConnectedEventArgs args)
    {
        _logger.LogInformation(
            "SMTP Connected: Host '{Host}', Port '{Port}'",
            args.Host,
            args.Port);
    }

    private void OnDisconnected(object sender, DisconnectedEventArgs args)
    {
        _logger.LogInformation(
            "SMTP Disconnected: Host '{Host}', Port '{Port}', Requested '{Requested}'",
            args.Host,
            args.Port,
            args.IsRequested ? "true" : "false");
    }
}