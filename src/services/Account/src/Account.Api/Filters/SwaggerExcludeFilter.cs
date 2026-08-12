using System.Reflection;
using Account.SharedKernel.Common.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Account.Api.Filters;

// Custom filter to exclude properties base on reflection 
public class SwaggerExcludeFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema?.Properties is null) return;

        IEnumerable<PropertyInfo> ignoreDataMemberProperties = context.Type
            .GetProperties()
            .Where(property => property.GetCustomAttribute<SwaggerExcludeAttribute>() is not null);

        foreach (PropertyInfo ignoreDataMemberProperty in ignoreDataMemberProperties)
        {
            string? propertyToHide = schema.Properties.Keys
                .FirstOrDefault(x => string.Equals(x, ignoreDataMemberProperty.Name, StringComparison.CurrentCultureIgnoreCase));

            if (propertyToHide is not null)
            {
                schema.Properties.Remove(propertyToHide);
            }
        }
    }
}
