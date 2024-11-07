namespace Common.Model.Dtos
{
    public class Repartition
    {
        public required Guid HouseholdId { get; set; }
        public required string MonthYear { get; set; }
        public required Dictionary<Guid, string> UserName { get; set; }
        public required Dictionary<Guid, decimal> IncomeAfterTax {  get; set; }
        public required Dictionary<Guid, decimal> UserSharesOfHouseholdIncome {  get; set; }
        public decimal TotalCommonExpenses { get; set; }
        public required Dictionary<Guid, decimal> TotalCommonExpensesPaidByUser { get; set; }
        public required Dictionary<Guid, decimal> UserShouldPay { get; set; }
        public required Dictionary<Guid, decimal> TargetUserShare { get; set; }
        public required Dictionary<Guid, decimal> ActualUserShare { get; set; }

    }
}
