using Microservice.Bancolombia.Api.Entities.Model;
using Microsoft.EntityFrameworkCore;

namespace Microservice.Bancolombia.Api.Entities
{
    public partial class MainContext : DbContext
    {
        public MainContext()
        {
        }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionHistory>()
                .ToTable("TransactionsHistory");

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasIndex(e => e.TransactionDate)
                    .HasDatabaseName("IX_TransactionHistory_TransactionDate");
                entity.HasIndex(e => e.BankCode)
                    .HasDatabaseName("IX_TransactionHistory_BankCode");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}