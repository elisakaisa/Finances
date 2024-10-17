using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class TransactionService : ITransactionService
    {
        private ITransactionRepository _transactionRepository;
        public TransactionService(ITransactionRepository transactionRepository) 
        { 
            _transactionRepository = transactionRepository;
        }

        public Task<Transaction> CreateAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> UpdateAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
