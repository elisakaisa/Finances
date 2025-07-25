﻿using Common.Model.Enums;

namespace Common.Model.DatabaseObjects
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public State State { get; set; }
        public TransactionType TransactionType { get; set; }

        // Navigation property: category has many categories
        public ICollection<Subcategory> Subcategories { get; set; } = [];
    }
}
