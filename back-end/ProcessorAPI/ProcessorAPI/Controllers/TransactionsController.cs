using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;
using System.Text;

namespace ProcessorAPI.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        ITransactionService transactionService;

        public TransactionsController(ITransactionService _transactionService)
        {
            transactionService = _transactionService;
        }

        [HttpGet]
        public async Task<IEnumerable<Transaction>> GetList()
        {
            return await transactionService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await transactionService.GetByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPost]
        [Consumes("application/json", "application/xml", "text/xml", "text/csv", "application/csv")]
        public async Task<IActionResult> PostData()
        {
            var contentType = Request.ContentType?.ToLower();

            if (string.IsNullOrWhiteSpace(contentType))
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Status = 400,
                    Title = "Invalid Content Type",
                    Detail = "Content-Type header is missing or invalid",
                    Instance = HttpContext.Request.Path
                });
            }

            try
            {
                string rawData;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    rawData = await reader.ReadToEndAsync();
                }

                var result = await transactionService.ProcessTransactionsAsync(rawData, contentType);

                if (result.Success)
                {
                    var response = new
                    {
                        Message = "Transactions created successfully",
                        Count = result.ProcessedCount,
                    };

                    return Created($"{Request.Path}", response);
                }
                else
                {
                    return StatusCode(500, "Upload failed");
                }
            }
            catch (FormatException ex)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Status = 400,
                    Title = "Invalid Format",
                    Detail = $"The provided data is not in valid {contentType} format: {ex.Message}",
                    Instance = HttpContext.Request.Path
                });
            }
            catch (Exception ex)
            {
                //TODO: Log the exception here
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred while processing the request",
                    Instance = HttpContext.Request.Path
                });
            }
        }
    }
}