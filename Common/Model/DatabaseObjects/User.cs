namespace Common.Model.DatabaseObjects
{
    public class User
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        //FK
        public Guid HouseholdId { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; } = [];
        public ICollection<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; } = [];

        public required Household Household { get; set; }
    }
}
