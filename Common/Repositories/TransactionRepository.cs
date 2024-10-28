using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public readonly FinancesDbContext _dbContext;

        public TransactionRepository(FinancesDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Transaction> CreateAsync(Transaction entity)
        {
            await _dbContext.Transactions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            //TODO: double check how this is done
            return entity;
        }

        public Task<ICollection<Transaction>> CreateMultipleAsync(ICollection<Transaction> transactionList)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(Transaction entity)
        {
            _dbContext.Transactions.Remove(entity);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }

        public async Task<Transaction> GetByIdAsync(Guid id)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.User)
                    .ThenInclude(t => t.Household)
                .Include(t => t.Subcategory)
                    .ThenInclude(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            return transaction ?? throw new KeyNotFoundException($"Transaction with ID {id} was not found.");
        }

        public async Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdIdAsync(Guid householdId, string monthYear)
        {
            var monthlyTransactions = await _dbContext.Transactions
                .Include(t => t.User)
                    .ThenInclude(t => t.Household)
                .Include(t => t.Subcategory)
                    .ThenInclude(t => t.Category)
                .Where(t => t.User.HouseholdId == householdId && t.FinancialMonth == monthYear)
                .ToListAsync();

            return monthlyTransactions;
        }

        public async Task<ICollection<Transaction>> GetMonthlyTransactionsByUserIdAsync(Guid userId, string monthYear)
        {
            var monthlyTransactions = await _dbContext.Transactions
                .Include(t => t.User)
                    .ThenInclude(t => t.Household)
                .Include(t => t.Subcategory)
                    .ThenInclude(t => t.Category)
                .Where(t => t.User.Id == userId && t.FinancialMonth == monthYear)
                .ToListAsync();

            return monthlyTransactions;
        }

        public async Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdIdAsync(Guid householdId, int year)
        {
            var yearS = year.ToString();
            var yearlyTransactions = await _dbContext.Transactions
                .Include(t => t.User)
                    .ThenInclude(t => t.Household)
                .Include(t => t.Subcategory)
                    .ThenInclude(t => t.Category)
                .Where(t => t.User.HouseholdId == householdId && t.FinancialMonth.StartsWith(yearS))
                .ToListAsync();

            return yearlyTransactions;
        }

        public async Task<ICollection<Transaction>> GetYearlyTransactionsByUserIdAsync(Guid userId, int year)
        {
            var yearS = year.ToString();
            var yearlyTransactions = await _dbContext.Transactions
                .Include(t => t.User)
                    .ThenInclude(t => t.Household)
                .Include(t => t.Subcategory)
                    .ThenInclude(t => t.Category)
                .Where(t => t.User.Id == userId && t.FinancialMonth.StartsWith(yearS))
                .ToListAsync();

            return yearlyTransactions;
        }

        public async Task<Transaction> UpdateAsync(Transaction entity)
        {
            _dbContext.Transactions.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}
