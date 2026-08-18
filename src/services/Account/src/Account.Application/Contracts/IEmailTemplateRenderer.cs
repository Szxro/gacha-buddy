namespace Account.Application.Contracts;

public interface IEmailTemplateRenderer
{
    Task<string> RenderAsync(
        string templateName,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken);
}