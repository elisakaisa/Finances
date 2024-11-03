namespace Common.Model.Dtos
{
    public class MonthlyIncomeAfterTaxDto
    {
        public Guid Id { get; set; }
        public required decimal IncomeAfterTax { get; set; }
        public required string FinancialMonth { get; set; }
        public required Guid UserId { get; set; }
    }
}
