using Common.Model.DatabaseObjects;

namespace Common.Model.Dtos
{
    public class Repartition
    {
        public Household Household { get; set; }
        public User User1 { get; set; }
        public User User2 { get; set; }
        public decimal IncomeAfterTaxUser1 { get; set; }
        public decimal IncomeAfterTaxUser2 { get; set; }
        public decimal TotalCommonExpenses { get; set; }
        public decimal TotalCommonExpensesPaidByUser1 { get; set; }
        public decimal TotalCommonExpensesPaidByUser2 { get; set; }
        public decimal User1ShouldPay { get; set; }
        public decimal User2ShouldPay { get; set; }
        public decimal TargetShareUser1 { get; set; }
        public decimal TargetShareUser2 { get; set; }
        public decimal ActualShareUser1 { get; set; }
        public decimal ActualShareUser2 { get; set; }
    }
}
