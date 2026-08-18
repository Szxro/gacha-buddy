using Account.SharedKernel.Common.Messaging;

namespace Account.Application.Contracts;

public interface IEmailService
{
    string GenerateCode(int length = 10);
    
    Task SendEmailAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default);
}