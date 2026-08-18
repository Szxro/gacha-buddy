using Account.Application.Contracts;
using Account.Domain.Events;
using Account.SharedKernel.Common.Messaging;

namespace Account.Application.EventHandlers;

public class EmailConfirmationEventHandler : IEventHandler<EmailConfirmationEvent>
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;

    public EmailConfirmationEventHandler(
        IEmailService emailService,
        IEmailTemplateRenderer emailTemplateRenderer)
    {
        _emailService = emailService;
        _emailTemplateRenderer = emailTemplateRenderer;
    }

    public async Task Handle(EmailConfirmationEvent @event, CancellationToken cancellationToken = default)
    {
        string body = await _emailTemplateRenderer.RenderAsync(
            "EmailVerification",
            new Dictionary<string, string>
            {
                ["Username"] = @event.Username,
                ["ConfirmationCode"] = @event.Code,
            },
            cancellationToken);

        EmailMessage message = new EmailMessage
        {
            Subject = "Email Verification",
            ToAddress = @event.Email,
            Body = body,
        };

        await _emailService.SendEmailAsync(
            message,
            cancellationToken);
    }
}