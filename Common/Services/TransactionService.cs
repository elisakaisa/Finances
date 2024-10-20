using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        public TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepo, ISubcategoryRepository subcategoryRepo) 
        { 
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepo;
            _subcategoryRepository = subcategoryRepo;
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
