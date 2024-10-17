namespace Common.Model.DatabaseObjects
{
    public class Household
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public ICollection<User> Users { get; set; }
        public ICollection<FinancialMonth> FinancialMonths { get; set; }
    }
}
