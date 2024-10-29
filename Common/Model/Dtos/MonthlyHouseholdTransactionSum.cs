using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Model.Dtos
{
    public class MonthlyHouseholdTransactionSum
    {
        public required string FinancialMonth { get; set; }
        public required int Year { get; set; }
        public required TransactionType TransactionType { get; set; }
        public required int CategoryId { get; set; }
        public required int SubcategoryId { get; set; }
        public required decimal Total { get; set; }
        public required decimal CommonTotal { get; set; }
        public required List<MonthlyUserTransactionSum> monthlyUserTransactionSums { get; set; }
        public required Household Household { get; set; }
    }
}
