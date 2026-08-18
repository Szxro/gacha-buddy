using System.Reflection;
using Account.Application.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Account.Infrastructure.Messaging.Email;

[Inject(serviceKind:ServiceKind.Service, ServiceLifetime.Scoped)]
public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly string _templatePath;
    
    public EmailTemplateRenderer()
    {
        // in this case the HTML files are going to be copied to the output (TODO: SEARCH A BETTER WAY)
        string assemblyPath = Path.GetDirectoryName(
            typeof(EmailTemplateRenderer).Assembly.Location)!;

        // Getting the template path
        _templatePath = Path.Combine(
            assemblyPath,
            "Messaging",
            "Email",
            "Templates");
    }
    
    public async Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> parameters, CancellationToken cancellationToken)
    {
        // templatePath/EmailVerification.html
        string filePath = Path.Combine(
            _templatePath,
            $"{templateName}.html");
    
        // checking if the file exists
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Email template '{templateName}' was not found.",
                filePath);
        }
        
        // Reading the hold HTML
        string template = await File.ReadAllTextAsync(filePath, cancellationToken);
        
        // for each parameter replacing by key the key value
        foreach (KeyValuePair<string, string> parameter in parameters)
        {
            template = template.Replace(
                $"{{{parameter.Key}}}", // {CodeName} -> replace -> 1234
                parameter.Value,
                StringComparison.InvariantCultureIgnoreCase);
        }
        
        // returning the template (in string form)
        return template;
    }
}