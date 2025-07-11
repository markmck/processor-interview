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
        public IEnumerable<Transaction> Get()
        {
            return _context.Transactions.ToList();
        }
    }
}