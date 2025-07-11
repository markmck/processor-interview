using Microsoft.EntityFrameworkCore;
using ProcessorAPI.Models;

namespace ProcessorAPI.Data
{
    public class ProcessorDbContext : DbContext
    {
        public ProcessorDbContext(DbContextOptions<ProcessorDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }
    }
}