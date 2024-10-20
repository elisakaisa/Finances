namespace Common.Model.DatabaseObjects
{
    public class MonthlyIncomeAfterTax
    {
        public Guid Id { get; set; }
        public decimal IncomeAfterTax { get; set; }
        public string FinancialMonth { get; set; }

        // FK
        public Guid UserId { get; set; }

        // NavigationProperty
        public User User { get; set; }
    }
}
