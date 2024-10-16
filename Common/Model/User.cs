namespace Common.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; }
    }
}
