using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspireApp1.DbApi;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formFileParameters = context.ApiDescription.ParameterDescriptions
            .Where(p => p.Type == typeof(IFormFile) || p.Type == typeof(IFormFileCollection))
            .ToList();

        if (!formFileParameters.Any())
            return;

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = context.ApiDescription.ParameterDescriptions
                            .Where(p => p.Source.Id == "Form")
                            .ToDictionary(
                                p => p.Name,
                                p => p.Type == typeof(IFormFile) || p.Type == typeof(IFormFileCollection)
                                    ? new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.String,
                                        Format = "binary"
                                    }
                                    : context.SchemaGenerator.GenerateSchema(p.Type, context.SchemaRepository)
                            ),
                        Required = context.ApiDescription.ParameterDescriptions
                            .Where(p => p.IsRequired && p.Source.Id == "Form")
                            .Select(p => p.Name)
                            .ToHashSet()
                    }
                }
            }
        };

        // Remove file parameters from query/path as they're now in the request body
        foreach (var parameter in formFileParameters)
        {
            var paramToRemove = operation.Parameters?.FirstOrDefault(p => p.Name == parameter.Name);
            if (paramToRemove != null && operation.Parameters != null)
            {
                operation.Parameters.Remove(paramToRemove);
            }
        }
    }
}
