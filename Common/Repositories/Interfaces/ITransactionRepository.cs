﻿using Common.Model;

namespace Common.Repositories.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        // GET METHODS
        Task<ICollection<Transaction>> GetYearlyTransactionsByUserIdAsync(int userId, int year);
        Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdIdAsync(Guid householdId, int year);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByUserIdAsync(int userId, string monthYear);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdIdAsync(Guid householdId, string monthYear);

        // CREATE METHODS
        Task<ICollection<Transaction>> CreateMultipleAsync(ICollection<Transaction> transactionList);
    }
}