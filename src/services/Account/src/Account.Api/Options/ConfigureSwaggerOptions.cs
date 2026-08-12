using Account.Api.Filters;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Account.Api.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }
    
    public void Configure(SwaggerGenOptions options)
    {
        // It's going to configure the swagger doc base on the version discover in the project 
        foreach (ApiVersionDescription description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName, // v1,v2,....
                new OpenApiInfo()
                {
                    Title = "Account API",
                    Description = "Handles user account operations such as user registration and account-related processes.",
                    Version = description.GroupName,
                });
            
            // Adding the filter to the swagger doc
            options.SchemaFilter<SwaggerExcludeFilter>();
        }
    }
}