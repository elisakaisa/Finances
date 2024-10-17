using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;

namespace Common.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public Task<Transaction> CreateAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> CreateMultipleAsync(ICollection<Transaction> transactionList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdIdAsync(Guid householdId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetMonthlyTransactionsByUserIdAsync(Guid userId, string monthYear)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdIdAsync(Guid householdId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetYearlyTransactionsByUserIdAsync(Guid userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> UpdateAsync(Transaction entity)
        {
            throw new NotImplementedException();
        }
    }
}
