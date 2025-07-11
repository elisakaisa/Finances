﻿using Common.Model.DatabaseObjects;

namespace Common.Repositories.Interfaces
{
    public interface IMonthlyIncomeAfterTaxRepository : IRepository<MonthlyIncomeAfterTax>
    {
        Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAsync(Guid userId, string monthYear);
        Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAsync(Guid HouseholdId, string monthYear);
        Task<MonthlyIncomeAfterTax> GetYearlyIncomeAfterTaxByUserIdAsync(Guid userId, int year);
        Task<ICollection<MonthlyIncomeAfterTax>> GetYearlyIncomeAfterTaxByHouseholdIdAsync(Guid householdId, int year);
    }
}
