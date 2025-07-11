using Microsoft.AspNetCore.Mvc;
using ProcessorAPI.Interfaces.Services;
using ProcessorAPI.Models;

namespace ProcessorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        ITransactionService transactionService;

        public TransactionController(ITransactionService _transactionService)
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

        [HttpPut("upload")]
        public async Task<IActionResult> PutData([FromBody] List<Transaction> transactions)
        {
            var contentType = Request.ContentType?.ToLower();
            bool result = false;

            if (contentType?.Contains("xml") == true)
            {
                result = await transactionService.HandleXmlUpload(transactions);
            }
            else if (contentType?.Contains("csv") == true)
            {
                result = await transactionService.HandleCsvUpload(transactions);
            }
            else if (contentType?.Contains("json") == true)
            {
                result = await transactionService.HandleJsonUpload(transactions);
            }
            else
            {
                return BadRequest("Unsupported content type. Use application/json, application/xml, or text/csv");
            }

            if (result)
            {
                return Ok("Upload successful");
            }
            else
            {
                return StatusCode(500, "Upload failed");
            }
        }
    }
}