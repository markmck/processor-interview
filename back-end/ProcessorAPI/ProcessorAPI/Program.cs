using Microsoft.EntityFrameworkCore;
using ProcessorAPI.Data;
using ProcessorAPI.Data.Repositories;
using ProcessorAPI.Interfaces.Repositories;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Services;

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
