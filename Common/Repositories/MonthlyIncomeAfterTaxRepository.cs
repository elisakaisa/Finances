using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;

namespace Common.Repositories
{
    public class MonthlyIncomeAfterTaxRepository : IMonthlyIncomeAfterTaxRepository
    {
        public Task<MonthlyIncomeAfterTax> CreateAsync(MonthlyIncomeAfterTax entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(MonthlyIncomeAfterTax entity)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<MonthlyIncomeAfterTax>> GetYearlyIncomeAfterTaxByHouseholdIdAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetYearlyIncomeAfterTaxByUserIdAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> UpdateAsync(MonthlyIncomeAfterTax entity)
        {
            throw new NotImplementedException();
        }
    }
}
