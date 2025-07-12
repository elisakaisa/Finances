namespace Common.Model.DatabaseObjects
{
    public class MonthlyIncomeAfterTax
    {
        public required Guid Id { get; set; }
        public required decimal IncomeAfterTax { get; set; }
        public required string FinancialMonth { get; set; }

        // FK
        public required Guid UserId { get; set; }

        // NavigationProperty
        public User User { get; set; } = default!;
    }
}
