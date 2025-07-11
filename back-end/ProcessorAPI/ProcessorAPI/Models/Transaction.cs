
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessorAPI.Models
{
    public class Transaction
    {
        // In the real work, this should probably be a GUID or similar for security reasons. But for simplicity, I'm using an integer ID.
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(16)]
        public string CardNumber { get; set; } = string.Empty;
    }
}