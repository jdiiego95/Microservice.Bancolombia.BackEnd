using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservice.Bancolombia.Api.Entities.Model
{
    public partial class TransactionHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TransactionId { get; set; }

        [StringLength(50)]
        public string BankCode { get; set; } = null!;

        public int FromAccountId { get; set; }

        public int ToAccountId { get; set; }

        [StringLength(3)]
        [Column(TypeName = "char(3)")]
        public string TransactionType { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [ForeignKey(nameof(FromAccountId))]
        public virtual Account FromAccount { get; set; } = null!;

        [ForeignKey(nameof(ToAccountId))]
        public virtual Account ToAccount { get; set; } = null!;
    }
}