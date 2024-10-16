using Common.Model.Enums;

namespace Common.Model
{
    public class FinancialMonth
    {
        public string Id { get; set; } //should be of format YYYY-MM
        public FinancialMonthName FinancialMonthName { get; set; }
        public int FinancialYear { get; set; }

        // Navigation Property
        public ICollection<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
