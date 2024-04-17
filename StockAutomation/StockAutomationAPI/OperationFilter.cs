namespace WebAPI;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


public class OperationFilter : IOperationFilter
{
    private readonly string _name;
    private readonly string _description;
    private readonly string _defaultValue;
    private readonly IList<string> _permittedValues;
    private readonly bool _required;

    public OperationFilter(string name, string description, string defaultValue, IList<string> permittedValues, bool required)
    {
        _name = name;
        _description = description;
        _defaultValue = defaultValue;
        _permittedValues = permittedValues;
        _required = required;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = _name,
            In = ParameterLocation.Query,
            Description = _description,
            Required = _required,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(_defaultValue),
                Enum = _permittedValues.Select(v => new OpenApiString(v)).ToList<IOpenApiAny>()
            }
        });
    }
}
