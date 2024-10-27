using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

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

        /// <summary>
        /// Creates a trasaction
        /// </summary>
        /// <param name="transaction">The values of the new transaction</param>
        /// <param name="user">The user creating the transaction</param>
        /// <returns>The created transaction</returns>
        public async Task<Transaction> CreateAsync(Transaction transaction, User user)
        {
            ValidateThatUserIsInHousehold(transaction, user);
            await ValidateTransactionData(transaction);

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            return createdTransaction;
        }

        /// <summary>
        /// Deletes a transaction
        /// </summary>
        /// <param name="transaction">The transaction to be deleted</param>
        /// <param name="user">The user deleting the transaction</param>
        /// <returns>True/False if the transaction has been deleted</returns>
        public async Task<bool> DeleteAsync(Transaction transaction, User user)
        {
            ValidateThatUserIsInHousehold(transaction, user);
            return await _transactionRepository.DeleteAsync(transaction);
        }

        public async Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId, string financialMonth, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            ValidateFinancialMonth(financialMonth);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(householdId, financialMonth);
            return transactions;
        }

        public async Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, User user)
        {
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            ValidateThatUserIsInHousehold(user, userToGetTransactionsFrom.HouseholdId);
            ValidateFinancialMonth(financialMonth);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByUserIdAsync(userId, financialMonth);
            return transactions;
        }

        public async Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId, int year, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            var transactions = await _transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(householdId, year);
            return transactions;
        }

        public async Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId, int year, User user)
        {
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            ValidateThatUserIsInHousehold(user, userToGetTransactionsFrom.HouseholdId);
            var transactions = await _transactionRepository.GetYearlyTransactionsByUserIdAsync(userId, year);
            return transactions;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction, User user)
        {
            ValidateThatUserIsInHousehold(transaction, user);
            await ValidateTransactionData(transaction);

            var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);
            return updatedTransaction;
        }



        private async Task ValidateTransactionData(Transaction transaction)
        {
            if (!MandatoryFieldsAreFilled(transaction) || !UserShareHasValidValues(transaction))
            {
                throw new MissingOrWrongTransactionDataException("Mandatory fields are not filled");
            }

            if (!transaction.FinancialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }

            if (transaction.TransactionType == TransactionType.Income && transaction.SplitType != SplitType.Individual)
            {
                throw new MissingOrWrongTransactionDataException("Income can only be an individual expense");
            }

            if (transaction.TransactionType != transaction.Category.TransactionType)
            {
                throw new MissingOrWrongTransactionDataException("Category is of the wrong transaction type");
            }

            var subcategoriesInCategory = await _categoryRepository.GetCategorysSubcategories(transaction.CategoryId);
            if (!IsSubcategoryInCategory(subcategoriesInCategory, transaction.SubcategoryId))
            {
                throw new SubcategoryNotContainedInCategoryException();
            }
        }


        private static void ValidateFinancialMonth(string financialMonth)
        {
            if (!financialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }
        }

        private static bool MandatoryFieldsAreFilled(Transaction transaction)
        {
            return transaction.Id != Guid.Empty
                && transaction.Description != string.Empty
                && transaction.Amount != 0
                && transaction.CategoryId != 0
                && transaction.SubcategoryId != 0
                && transaction.UserId != Guid.Empty;
        }

        private static bool UserShareHasValidValues(Transaction transaction)
        {
            if (transaction.SplitType == SplitType.Custom && transaction.UserShare == null) return false;
            else if (transaction.SplitType == SplitType.Custom && (transaction.UserShare < 0 || transaction.UserShare > 1)) return false;
            return true;
        }

        // TODO: should this be an extension method? Or in category object?
        private static bool IsSubcategoryInCategory(ICollection<Subcategory> subcategoriesInCategory, int subcategoryId)
        {
            return subcategoriesInCategory.Any(sc => sc.Id == subcategoryId);
        }
    }
}
