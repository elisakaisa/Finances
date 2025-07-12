using Common.Model.Enums;

namespace Common.Model.DatabaseObjects
{
    public class Subcategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public State State { get; set; }
        public TransactionType TransactionType { get; set; }
        public int DisplayOrder {  get; set; }

        // FK
        public int CategoryId { get; set; }

        // Navigation property: each subcategory belongs to 1 category
        public Category Category { get; set; } = default!;
    }
}
