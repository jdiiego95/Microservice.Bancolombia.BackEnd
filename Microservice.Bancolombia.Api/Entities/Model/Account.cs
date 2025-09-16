using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservice.Bancolombia.Api.Entities.Model
{
    public partial class Account
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBalance { get; set; }

        public virtual ICollection<TransactionHistory> IncomingTransactions { get; set; } = new List<TransactionHistory>();
    }
}