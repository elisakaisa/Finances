using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class MonthlyIncomeAfterTaxRepository(FinancesDbContext context) : IMonthlyIncomeAfterTaxRepository
    {
        public async Task<MonthlyIncomeAfterTax> CreateAsync(MonthlyIncomeAfterTax entity)
        {
            context.Users.Attach(entity.User);

            await context.MonthlyIncomesAfterTax.AddAsync(entity);
            await context.SaveChangesAsync();
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
            var monthlyIncomes = await context.MonthlyIncomesAfterTax
                .Include(i => i.User)
                    .ThenInclude(t => t.Household)
                .Where(i => i.User.HouseholdId == householdId && i.FinancialMonth == monthYear)
                .ToListAsync();
            return monthlyIncomes;
        }

        public async Task<ICollection<MonthlyIncomeAfterTax>> GetYearlyIncomeAfterTaxByHouseholdIdAsync(Guid householdId, int year)
        {
            var yearS = year.ToString();
            var monthlyIncomes = await context.MonthlyIncomesAfterTax
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
            context.Users.Attach(entity.User);

            context.MonthlyIncomesAfterTax.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
