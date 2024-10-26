using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;

        public TransactionService(ITransactionRepository transactionRepository, ICategoryRepository categoryRepo, ISubcategoryRepository subcategoryRepo, IUserRepository userRepo) 
        { 
            _transactionRepository = transactionRepository;
            _categoryRepository = categoryRepo;
            _subcategoryRepository = subcategoryRepo;
            _userRepository = userRepo;
        }

        public Task<Transaction> CreateAsync(Transaction transaction)
        {
            // start with validation passed data
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);

            throw new NotImplementedException();
        }

        public async Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId, User user)
        {
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            ValidateThatUserIsInHousehold(user, userToGetTransactionsFrom.HouseholdId);
            throw new NotImplementedException();
        }

        public Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            throw new NotImplementedException();
        }

        public async Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId, User user)
        {
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            ValidateThatUserIsInHousehold(user, userToGetTransactionsFrom.HouseholdId);
            throw new NotImplementedException();
        }

        public Task<Transaction> UpdateAsync(Transaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}
