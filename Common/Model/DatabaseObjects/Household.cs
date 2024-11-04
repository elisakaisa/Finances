namespace Common.Model.DatabaseObjects
{
    public class Household
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        // Navigation properties
        public ICollection<User> Users { get; set; }
    }
}
