namespace Account.SharedKernel.Common.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SwaggerExcludeAttribute : Attribute { }
// Attribute used to exclude properties from the swagger documentation