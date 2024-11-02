using Common.Model.Enums;

namespace Common.Model.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public required string Description { get; set; }
        public DateOnly Date { get; set; }
        public string? FromOrTo { get; set; }
        public string? Location { get; set; }
        public bool ExcludeFromSummary { get; set; }
        public TransactionType TransactionType { get; set; }
        public SplitType SplitType { get; set; }
        public decimal? UserShare { get; set; }
        public decimal Amount { get; set; }
        public bool ToVerify { get; set; }
        public ModeOfPayment ModeOfPayment { get; set; }
        public required string FinancialMonth { get; set; }
        public required string SubcategoryName { get; set; }
        public required string CategoryName { get; set; }
        public Guid UserId { get; set; }
    }
}
