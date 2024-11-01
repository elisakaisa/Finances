using Common.Model.DatabaseObjects;
using Microsoft.EntityFrameworkCore;

namespace Common.Database
{
    public class FinancesDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subcategory> Subcategories { get; set; }

        public FinancesDbContext(DbContextOptions<FinancesDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonthlyIncomeAfterTax>()
                .Property(m => m.IncomeAfterTax)
                .HasPrecision(12, 3);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(12, 3);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.UserShare)
                .HasPrecision(4, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
