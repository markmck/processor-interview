using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProcessorAPI.Data;
using ProcessorAPI.Data.Repositories;
using ProcessorAPI.Helpers;
using ProcessorAPI.Interfaces.Helpers;
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

// Add CORS only for development/debugging
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? 
            throw new InvalidOperationException("JWT SecretKey not configured"))),
        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
    };
});

builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IJSONParser, JSONParser>();
builder.Services.AddScoped<IXMLParser, XMLParser>();
builder.Services.AddScoped<ICSVParser, CSVParser>();
builder.Services.AddScoped<ITransactionValidator, TransactionValidator>();

builder.Services.AddDbContext<ProcessorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProcessorDb")));

// Add OpenAPI/Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Processor API",
        Version = "v1",
        Description = "API for processing credit card transactions"
    });

    options.EnableAnnotations();

    options.OperationFilter<MultipleContentTypesOperationFilter>();

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //Configure Swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Card Processor API v1");
        options.RoutePrefix = "swagger";
    });

    //Allow all connections for development env
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();

app.UseAuthentication();
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