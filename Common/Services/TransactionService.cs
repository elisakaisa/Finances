﻿using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class TransactionService : ITransactionService
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
        public async Task<Transaction> CreateAsync(TransactionDto transactionDto, Guid requestingUser)
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUser, user.HouseholdId);
            ValidateTransactionData(transactionDto);

            var subcategory = await _subcategoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);

            var createdTransaction = await _transactionRepository.CreateAsync(transaction);
            return createdTransaction;
        }

        /// <summary>
        /// Deletes a transaction
        /// </summary>
        /// <param name="transaction">The transaction to be deleted</param>
        /// <param name="user">The user deleting the transaction</param>
        /// <returns>True/False if the transaction has been deleted</returns>
        public async Task<bool> DeleteAsync(TransactionDto transactionDto, Guid requestingUser)
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUser, user.HouseholdId);

            var subcategory = await _subcategoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);
            // TODO: double check if whole transaction is needed or if id is fine
            return await _transactionRepository.DeleteAsync(transaction);
        }

        public async Task<ICollection<TransactionDto>> GetMonthlyTransactionsByHouseholdId(Guid householdId, string financialMonth, Guid requestingUserId)
        {
            ValidateFinancialMonth(financialMonth);
            await ValidateThatUserIsInHousehold(requestingUserId, householdId);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(householdId, financialMonth);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, Guid requestingUserId)
        {
            ValidateFinancialMonth(financialMonth);
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            await ValidateThatUserIsInHousehold(requestingUserId, userToGetTransactionsFrom.HouseholdId);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByUserIdAsync(userId, financialMonth);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetYearlyTransactionsByHouseholdId(Guid householdId, int year, Guid requestingUser)
        {
            await ValidateThatUserIsInHousehold(requestingUser, householdId);
            var transactions = await _transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(householdId, year);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetYearlyTransactionsByUserId(Guid userId, int year, Guid requestingUser)
        {
            var userToGetTransactionsFrom = await _userRepository.GetByIdAsync(userId);
            await ValidateThatUserIsInHousehold(requestingUser, userToGetTransactionsFrom.HouseholdId);
            var transactions = await _transactionRepository.GetYearlyTransactionsByUserIdAsync(userId, year);
            return transactions.ConvertToDto();
        }

        public async Task<Transaction> UpdateAsync(TransactionDto transactionDto, Guid requestingUser)
        {
            var user = await _userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUser, user.HouseholdId);
            ValidateTransactionData(transactionDto);

            var subcategory = await _subcategoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);

            var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);
            return updatedTransaction;
        }

        private async Task ValidateThatUserIsInHousehold(Guid requestingUserId, Guid householdId)
        {
            var requestingUser = await _userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }

        }

        private async Task ValidateThatUserIsInHousehold(Guid requestingUserId, User user)
        {
            var requestingUser = await _userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != user.HouseholdId)
            {
                throw new UserNotInHouseholdException();
            }
        }


        private void ValidateTransactionData(TransactionDto transactionDto)
        {
            if (!MandatoryFieldsAreFilled(transactionDto) || !UserShareHasValidValues(transactionDto))
            {
                throw new MissingOrWrongTransactionDataException("Mandatory fields are not filled");
            }

            if (!transactionDto.FinancialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }

            if (transactionDto.TransactionType == TransactionType.Income && transactionDto.SplitType != SplitType.Individual)
            {
                throw new MissingOrWrongTransactionDataException("Income can only be an individual expense");
            }
        }

        private void ValidateTransactionCategory(TransactionDto transactionDto, Subcategory subcategory)
        {
            if (transactionDto.TransactionType != subcategory.Category.TransactionType)
            {
                throw new MissingOrWrongTransactionDataException("Category is of the wrong transaction type");
            }

            // TODO figure out if this is still needed
            //var subcategoriesInCategory = await _subcategoryRepository.GetSubcategoryByCategoryIdAsync(transactionDto.Subcategory.CategoryId);
            //if (!IsSubcategoryInCategory(subcategoriesInCategory, transactionDto.SubcategoryId))
            //{
            //    throw new SubcategoryNotContainedInCategoryException();
            //}
        }


        private static void ValidateFinancialMonth(string financialMonth)
        {
            if (!financialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }
        }

        private static bool MandatoryFieldsAreFilled(TransactionDto transaction)
        {
            return transaction.Id != Guid.Empty
                && transaction.Description != string.Empty
                && transaction.Amount != 0
                && transaction.UserId != Guid.Empty;
        }

        private static bool UserShareHasValidValues(TransactionDto transaction)
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
