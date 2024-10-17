using Common.Model;
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

        public Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAndByMonthAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAndByYearAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAndByMonthAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAndByYearAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> UpdateAsync(MonthlyIncomeAfterTax entity)
        {
            throw new NotImplementedException();
        }
    }
}
