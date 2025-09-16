using Microservice.Bancolombia.Api.Entities.Model;
using Microservice.Bancolombia.Entities.Model;
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
            // Configuración para Account
            modelBuilder.Entity<Account>(entity =>
            {
                // Aquí puedes agregar configuraciones específicas para Account
                // Por ejemplo: entity.HasKey(e => e.AccountId);
                // entity.Property(e => e.AccountNumber).IsRequired();
            });

            // Configuración para TransactionHistory
            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.Property(e => e.BankCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.TransactionType)
                    .HasMaxLength(3)
                    .HasColumnType("char(3)")
                    .IsRequired();

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");

                // Configuración de relaciones
                entity.HasOne(d => d.ToAccount)
                    .WithMany()
                    .HasForeignKey(d => d.ToAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}