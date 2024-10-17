using Common.Model.DatabaseObjects;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction); 
        Task<bool> DeleteAsync(Transaction transaction);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId);
        Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId);
        Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId);
    }
}
