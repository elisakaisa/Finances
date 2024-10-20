using Common.Model.Enums;

namespace Common.Model.DatabaseObjects
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
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
        public string FinancialMonth { get; set; }




        // FKs
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public Guid UserId { get; set; }

        // Navigation property: Each transaction has one category
        public Category Category { get; set; }

        // Navigation property: Each transaction has one subcategory
        public Subcategory Subcategory { get; set; }
        // Navigation property: Each transaction has one user
        public User User { get; set; }
    }
}
