using Common.Model.Enums;

namespace Common.Model.DatabaseObjects
{
    public class FinancialMonth
    {
        public Guid Id { get; set; }
        public string Name { get; set; } //should be of format YYYY-MM
        public FinancialMonthName FinancialMonthName { get; set; }
        public int FinancialYear { get; set; }

        //FK
        public Guid HouseholdId { get; set; }

        // Navigation Property
        public ICollection<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

        public Household Household { get; set; }
    }
}
