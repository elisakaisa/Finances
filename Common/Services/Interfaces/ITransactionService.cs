using Common.Model.DatabaseObjects;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction); 
        Task<bool> DeleteAsync(Transaction transaction);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, User user);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId, string financialMonth, User user);
        Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId, int year, User user);
        Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId, int year, User user);
    }
}
