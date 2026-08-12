using Account.Api.Options;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Account.Api.Extensions;

public static class SwaggerGenExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IHostEnvironment env)
    {
        services.AddSwaggerGen(options =>
        {
            // Pick up docs from every layer that emits them
            foreach (string xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "Account.*.xml"))
            {
                options.IncludeXmlComments(xmlFile);
            }
            
            options.EnableAnnotations();
            
            options.DescribeAllParametersInCamelCase();
            
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Standard Authorization header using the Bearer Scheme (\"Bearer {token}\")",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            });
            
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    []
                }
            });
        });
        
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        
        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Reverse the list of API versions so the newest version is rendered first
            foreach (ApiVersionDescription description in app.DescribeApiVersions().Reverse())
            {
                options.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",  // (first) swagger/v2/swagger.json -> (second) swagger/v1/swagger.json 
                    description.GroupName.ToUpperInvariant());
            }
            
            options.DefaultModelRendering(ModelRendering.Model);
            
            if (app.Environment.IsDevelopment())
            {
                // Persist the token even if the page is reloaded or the project itself
                options.EnablePersistAuthorization();
            }       
        });
        
        return app;
    }
}