using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProcessorAPI.SwaggerFilters;

public class MultipleContentTypesOperationFilter : IOperationFilter
{
    private const string JSON_EXAMPLE = @"[
          {
            ""cardNumber"": ""4111111111111111"",
            ""amount"": 100.50,
            ""timestamp"": ""2024-01-15T10:30:00""
          },
          {
            ""cardNumber"": ""5500000000000004"",
            ""amount"": 250.75,
            ""timestamp"": ""2024-01-15T14:45:00""
          }
        ]";

    private const string XML_EXAMPLE = "<?xml version=\"1.0\" ?>\n" +
        "<transactions>\n" +
        "  <transaction>\n" +
        "    <cardNumber>4111111111111111</cardNumber>\n" +
        "    <amount>100.50</amount>\n" +
        "    <timestamp>2024-01-15T10:30:00</timestamp>\n" +
        "  </transaction>\n" +
        "  <transaction>\n" +
        "    <cardNumber>5500000000000004</cardNumber>\n" +
        "    <amount>250.75</amount>\n" +
        "    <timestamp>2024-01-15T14:45:00</timestamp>\n" +
        "  </transaction>\n" +
        "</transactions>";

    private const string CSV_EXAMPLE = "cardNumber,amount,timestamp\n" +
        "4111111111111111,100.50,2024-01-15T10:30:00\n" +
        "5500000000000004,250.75,2024-01-15T14:45:00";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var consumesAttribute = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<ConsumesAttribute>()
            .FirstOrDefault();

        if (consumesAttribute?.ContentTypes?.Count > 0)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>()
            };

            foreach (var contentType in consumesAttribute.ContentTypes)
            {
                var mediaType = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                };

                // Add examples for each content type
                switch (contentType.ToLower())
                {
                    case "application/json":
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(JSON_EXAMPLE);
                        break;

                    case "application/xml":
                    case "text/xml":
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(XML_EXAMPLE);
                        break;

                    case "text/csv":
                    case "application/csv":
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(CSV_EXAMPLE);
                        break;
                }

                operation.RequestBody.Content[contentType] = mediaType;
            }
        }
    }
}