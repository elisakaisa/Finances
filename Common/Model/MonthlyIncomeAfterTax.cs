using Common.Model.Enums;

namespace Common.Model
{
    public class MonthlyIncomeAfterTax
    {
        public Guid Id { get; set; }
        public decimal IncomeAfterTax { get; set; }

        // FK
        public int UserId { get; set; }
        public string FinancialMonthId { get; set; }

        // NavigationProperty
        public User User { get; set; }
        public FinancialMonth FinancialMonth { get; set; }
    }
}
