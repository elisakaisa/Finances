namespace Common.Model.DatabaseObjects
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //FK
        public Guid HouseholdId { get; set; }

        // Navigation property
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<MonthlyIncomeAfterTax> MonthlyIncomesAfterTax { get; set; }

        public Household Household { get; set; }
    }
}
