using Microsoft.AspNetCore.Mvc;
using ProcessorAPI.Data;
using ProcessorAPI.Models;

namespace ProcessorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ProcessorDbContext _context;

        public TransactionController(ProcessorDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Transaction> GetList()
        {
            return _context.Transactions.ToList();
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var transaction = _context.Transactions.Find(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpPut]
        public IActionResult PutList([FromBody] List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0 || !ModelState.IsValid)
            {
                return BadRequest("Invalid transaction data.");
            }

            _context.Transactions.AddRange(transactions);
            _context.SaveChanges();
            return Ok();
        }
    }
}