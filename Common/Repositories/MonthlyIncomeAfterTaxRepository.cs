using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class MonthlyIncomeAfterTaxRepository : IMonthlyIncomeAfterTaxRepository
    {
        public readonly FinancesDbContext _dbContext;

        public MonthlyIncomeAfterTaxRepository(FinancesDbContext context)
        {
            _dbContext = context;
        }
        public async Task<MonthlyIncomeAfterTax> CreateAsync(MonthlyIncomeAfterTax entity)
        {
            _dbContext.Users.Attach(entity.User);

            await _dbContext.MonthlyIncomesAfterTax.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public Task<bool> DeleteAsync(MonthlyIncomeAfterTax entity)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAsync(Guid householdId, string monthYear)
        {
            var monthlyIncomes = await _dbContext.MonthlyIncomesAfterTax
                .Include(i => i.User)
                    .ThenInclude(t => t.Household)
                .Where(i => i.User.HouseholdId == householdId && i.FinancialMonth == monthYear)
                .ToListAsync();
            return monthlyIncomes;
        }

        public async Task<ICollection<MonthlyIncomeAfterTax>> GetYearlyIncomeAfterTaxByHouseholdIdAsync(Guid householdId, int year)
        {
            var yearS = year.ToString();
            var monthlyIncomes = await _dbContext.MonthlyIncomesAfterTax
                .Include(i => i.User)
                    .ThenInclude(t => t.Household)
                .Where(i => i.User.HouseholdId == householdId && i.FinancialMonth.StartsWith(yearS))
                .ToListAsync();
            return monthlyIncomes;
        }

        public Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> GetYearlyIncomeAfterTaxByUserIdAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public async Task<MonthlyIncomeAfterTax> UpdateAsync(MonthlyIncomeAfterTax entity)
        {
            _dbContext.Users.Attach(entity.User);

            _dbContext.MonthlyIncomesAfterTax.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
