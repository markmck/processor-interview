using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProcessorAPI.SwaggerFilters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "integer";
            schema.Format = "int32";
            
            var enumValues = new List<string>();
            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                var enumName = Enum.GetName(context.Type, enumValue);
                var enumIntValue = (int)enumValue;
                enumValues.Add($"{enumName} = {enumIntValue}");
            }
            
            schema.Description = $"Possible values: {string.Join(", ", enumValues)}";
            
            // Add the actual enum values
            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                schema.Enum.Add(new Microsoft.OpenApi.Any.OpenApiInteger((int)enumValue));
            }
        }
    }
}