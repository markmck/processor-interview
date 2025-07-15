using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;
using ProcessorAPI.Models.DTOs;
using ProcessorAPI.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
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
        ILogger<TransactionsController> logger;

        public TransactionsController(ITransactionService _transactionService, ILogger<TransactionsController> _logger)
        {
            transactionService = _transactionService;
            this.logger = _logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Get filtered transactions",
            Description = "Retrieves a paginated list of transactions with optional filtering by card type, status, and date range"
        )]
        [ProducesResponseType(typeof(IEnumerable<Transaction>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IEnumerable<Transaction>> GetList(
            [FromQuery] CardType? cardType = null,
            [FromQuery] TransactionStatus? status = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null
        )
        {
            return await transactionService.GetAllAsync(cardType, status, fromDate, toDate);
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Upload transaction data",
            Description = "Processes and stores transaction data from JSON, XML, or CSV format"
        )]
        [Consumes("application/json", "application/xml", "text/xml", "text/csv", "application/csv")]
        [ProducesResponseType(typeof(UploadResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UploadResponseDTO>> PostData()
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
                    var response = new UploadResponseDTO
                    {
                        Message = "Transactions processed successfully",
                        Count = result.ProcessedCount,
                        ProcessedAt = DateTime.UtcNow
                    };

                    return CreatedAtAction(nameof(GetList), response);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "Processing Failed",
                        Detail = result.ErrorMessage ?? "Transaction processing failed",
                        Status = StatusCodes.Status500InternalServerError,
                        Instance = HttpContext.Request.Path
                    });
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
                logger.LogError(ex, "Error processing transactions");
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