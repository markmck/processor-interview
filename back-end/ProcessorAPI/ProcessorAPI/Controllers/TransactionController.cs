using Microsoft.AspNetCore.Mvc;

namespace ProcessorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Transaction> Get()
        {
            // Example static data; replace with your data source
            return new List<Transaction>
            {
                new Transaction { Id = 1, Date = DateTime.Now.AddDays(-2), Amount = 100.50m, Description = "Payment" },
                new Transaction { Id = 2, Date = DateTime.Now.AddDays(-1), Amount = 250.00m, Description = "Refund" }
            };
        }
    }
}