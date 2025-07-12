using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProcessorAPI.Data;
using ProcessorAPI.Data.Repositories;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.InputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerInputFormatter(options));
    options.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter());
});

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

builder.Services.AddDbContext<ProcessorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProcessorDb")));

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Processor API",
        Version = "v1",
        Description = "API for processing credit card transactions"
    });

    options.EnableAnnotations();

    options.OperationFilter<MultipleContentTypesOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Card Processor API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



public class MultipleContentTypesOperationFilter : IOperationFilter
{
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
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(
                            @"[
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
                            ]");
                        break;

                    case "application/xml":
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(
                            @"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <ArrayOfTransaction>
                              <Transaction>
                                <cardNumber>4111111111111111</cardNumber>
                                <amount>100.50</amount>
                                <timestamp>2024-01-15T10:30:00</timestamp>
                              </Transaction>
                              <Transaction>
                                <cardNumber>5500000000000004</cardNumber>
                                <amount>250.75</amount>
                                <timestamp>2024-01-15T14:45:00</timestamp>
                              </Transaction>
                            </ArrayOfTransaction>");
                        break;

                    case "text/csv":
                        mediaType.Example = new Microsoft.OpenApi.Any.OpenApiString(
                            @"cardNumber,amount,timestamp
                            4111111111111111,100.50,2024-01-15T10:30:00
                            5500000000000004,250.75,2024-01-15T14:45:00");
                        break;
                }

                operation.RequestBody.Content[contentType] = mediaType;
            }
        }
    }
}