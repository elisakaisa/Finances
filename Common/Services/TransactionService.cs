﻿using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class TransactionService(
        ITransactionRepository transactionRepository,
        ICategoryRepository subcategoryRepository,
        ISubcategoryRepository categoryRepository,
        IUserRepository userRepository,
        IHouseholdRepository householdRepository) : ITransactionService
    {
        /// <summary>
        /// Creates a trasaction
        /// </summary>
        /// <param name="transaction">The values of the new transaction</param>
        /// <param name="user">The user creating the transaction</param>
        /// <returns>The created transaction</returns>
        public async Task<Transaction> CreateAsync(TransactionDto transactionDto, Guid requestingUserId)
        {
            var user = await userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);
            ValidateTransactionData(transactionDto);

            var subcategory = await categoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);

            var createdTransaction = await transactionRepository.CreateAsync(transaction);
            return createdTransaction;
        }

        /// <summary>
        /// Deletes a transaction
        /// </summary>
        /// <param name="transaction">The transaction to be deleted</param>
        /// <param name="user">The user deleting the transaction</param>
        /// <returns>True/False if the transaction has been deleted</returns>
        public async Task<bool> DeleteAsync(TransactionDto transactionDto, Guid requestingUserId)
        {
            var user = await userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);

            var subcategory = await categoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);
            // TODO: double check if whole transaction is needed or if id is fine
            return await transactionRepository.DeleteAsync(transaction);
        }

        public async Task<ICollection<TransactionDto>> GetMonthlyTransactionsByHousehold(string financialMonth, Guid requestingUserId)
        {
            financialMonth.ValidateFinancialMonthFormat();
            var household = await householdRepository.GetHouseholdByUserId(requestingUserId) ?? throw new KeyNotFoundException();

            var transactions = await transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(household.Id, financialMonth);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, Guid requestingUserId)
        {
            financialMonth.ValidateFinancialMonthFormat();
            var userToGetTransactionsFrom = await userRepository.GetByIdAsync(userId);
            await ValidateThatUserIsInHousehold(requestingUserId, userToGetTransactionsFrom.HouseholdId);

            var transactions = await transactionRepository.GetMonthlyTransactionsByUserIdAsync(userId, financialMonth);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetYearlyTransactionsByHousehold(int year, Guid requestingUserId)
        {
            var household = await householdRepository.GetHouseholdByUserId(requestingUserId) ?? throw new KeyNotFoundException();
            var transactions = await transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(household.Id, year);
            return transactions.ConvertToDto();
        }

        public async Task<ICollection<TransactionDto>> GetYearlyTransactionsByUserId(Guid userId, int year, Guid requestingUserId)
        {
            var userToGetTransactionsFrom = await userRepository.GetByIdAsync(userId);
            await ValidateThatUserIsInHousehold(requestingUserId, userToGetTransactionsFrom.HouseholdId);
            var transactions = await transactionRepository.GetYearlyTransactionsByUserIdAsync(userId, year);
            return transactions.ConvertToDto();
        }

        public async Task<Transaction> UpdateAsync(TransactionDto transactionDto, Guid requestingUserId)
        {
            var user = await userRepository.GetByIdAsync(transactionDto.UserId);
            await ValidateThatUserIsInHousehold(requestingUserId, user.HouseholdId);
            ValidateTransactionData(transactionDto);

            var subcategory = await categoryRepository.GetSubcategoryByName(transactionDto.SubcategoryName);
            ValidateTransactionCategory(transactionDto, subcategory);
            var transaction = transactionDto.ConvertToDbObject(user, subcategory);

            var updatedTransaction = await transactionRepository.UpdateAsync(transaction);
            return updatedTransaction;
        }


        private async Task ValidateThatUserIsInHousehold(Guid requestingUserId, Guid householdId)
        {
            var requestingUser = await userRepository.GetByIdAsync(requestingUserId);
            if (requestingUser.HouseholdId != householdId)
            {
                throw new UserNotInHouseholdException();
            }

        }

        private static void ValidateTransactionData(TransactionDto transactionDto)
        {
            if (!MandatoryFieldsAreFilled(transactionDto) || !UserShareHasValidValues(transactionDto))
            {
                throw new MissingOrWrongDataException("Mandatory fields are not filled");
            }
            transactionDto.FinancialMonth.ValidateFinancialMonthFormat();

            var transactionTypeEnumValue = transactionDto.TransactionType.ConvertTransactionTypeToDb();
            var splitTypeEnumValue = transactionDto.SplitType.ConvertSplitTypeToDb();
            transactionDto.ModeOfPayment.ConvertModeOfPaymentToDb();

            if (transactionTypeEnumValue == TransactionType.Income && splitTypeEnumValue != SplitType.Individual)
            {
                throw new MissingOrWrongDataException("Income can only be an individual expense");
            }
        }

        private static void ValidateTransactionCategory(TransactionDto transactionDto, Subcategory subcategory)
        {
            var transactionTypeEnumValue = transactionDto.TransactionType.ConvertTransactionTypeToDb();
            if (transactionTypeEnumValue != subcategory.Category.TransactionType)
            {
                throw new MissingOrWrongDataException("Category is of the wrong transaction type");
            }

            // TODO figure out if this is still needed
            //var subcategoriesInCategory = await _subcategoryRepository.GetSubcategoryByCategoryIdAsync(transactionDto.Subcategory.CategoryId);
            //if (!IsSubcategoryInCategory(subcategoriesInCategory, transactionDto.SubcategoryId))
            //{
            //    throw new SubcategoryNotContainedInCategoryException();
            //}
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
            var splitTypeEnumValue = transaction.SplitType.ConvertSplitTypeToDb();
            if (splitTypeEnumValue == SplitType.Custom && transaction.UserShare == null) return false;
            else if (splitTypeEnumValue == SplitType.Custom && (transaction.UserShare < 0 || transaction.UserShare > 1)) return false;
            return true;
        }

        // TODO: should this be an extension method? Or in category object?
        private static bool IsSubcategoryInCategory(ICollection<Subcategory> subcategoriesInCategory, int subcategoryId)
        {
            return subcategoriesInCategory.Any(sc => sc.Id == subcategoryId);
        }
    }
}
