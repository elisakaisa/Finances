using Common.Model.DatabaseObjects;

namespace Common.Model.Dtos
{
    public class Repartition
    {
        public Household Household { get; set; }
        public Dictionary<Guid, User> Users { get; set; }
        public Dictionary<Guid, decimal> IncomeAfterTax {  get; set; }
        public Dictionary<Guid, decimal> UserSharesOfHouseholdIncome {  get; set; }
        public decimal TotalCommonExpenses { get; set; }
        public Dictionary<Guid, decimal> TotalCommonExpensesPaidByUser { get; set; }
        public Dictionary<Guid, decimal> UserShouldPay { get; set; }
        public Dictionary<Guid, decimal> TargetUserShare { get; set; }
        public Dictionary<Guid, decimal> ActualUserShare { get; set; }

    }
}
