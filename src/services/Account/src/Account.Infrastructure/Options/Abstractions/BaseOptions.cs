using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Account.Infrastructure.Options.Abstractions;

public abstract class BaseOptions<TOptions> : IConfigureOptions<TOptions>
    where TOptions : class, IOptionsModel
{
    protected readonly IConfiguration Configuration;

    public BaseOptions(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public virtual void Configure(TOptions options)
    {
        Configuration
            .GetSection(options.SectionName)
            .Bind(options);
    }
}