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
        public decimal TotalCommonPaidByUser1 { get; set; }
        public decimal TotalCommonPaidByUser2 { get; set; }
        public decimal ShareUser1 { get; set; }
        public decimal ShareUser2 { get; set; }
    }
}
