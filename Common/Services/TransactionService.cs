using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;

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

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            if (!MandatoryFieldsAreFilled(transaction))
            {
                throw new MissingOrWrongTransactionDataException();
            }


            var subcategoriesInCategory = await _categoryRepository.GetCategorysSubcategories(transaction.CategoryId);
            if (!IsSubcategoryInCategory(subcategoriesInCategory, transaction.SubcategoryId))
            {
                throw new SubcategoryNotContainedInCategoryException();
            }

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

        private bool MandatoryFieldsAreFilled(Transaction transaction)
        {
            if (transaction.Id == Guid.Empty) return false;
            if (transaction.Description == string.Empty) return false;
            if (transaction.Amount == 0) return false;
            if (transaction.CategoryId == 0) return false;
            if (transaction.SubcategoryId == 0) return false;
            if (transaction.UserId == Guid.Empty) return false;
            if (transaction.FinancialMonth == string.Empty) return false;
            if (transaction.SplitType == SplitType.Custom && transaction.UserShare == null) return false;
            return true;
        }

        // TODO: should this be an extension method? Or in category object?
        private bool IsSubcategoryInCategory(ICollection<Subcategory> subcategoriesInCategory, int subcategoryId)
        {
            var isIncludedInCategory = subcategoriesInCategory.Select(sc => sc.Id ==  subcategoryId).ToList().FirstOrDefault();
            return isIncludedInCategory;
        }
    }
}
