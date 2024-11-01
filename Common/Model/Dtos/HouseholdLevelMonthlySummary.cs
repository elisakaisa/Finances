﻿using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Model.Dtos
{
    public class HouseholdLevelMonthlySummary
    {
        public required string FinancialMonth { get; set; }
        public required int Year { get; set; }
        public required TransactionType TransactionType { get; set; }
        public required Subcategory Subcategory { get; set; }
        public required decimal Total { get; set; }
        public required decimal CommonTotal { get; set; }
        public required List<UserLevelMonthlySummary> UserLevelMonthlySummary { get; set; }
        public required Household Household { get; set; }
    }
}