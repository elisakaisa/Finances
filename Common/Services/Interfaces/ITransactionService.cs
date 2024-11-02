using Common.Model.DatabaseObjects;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateAsync(Transaction transaction, Guid requestingUser);
        Task<Transaction> UpdateAsync(Transaction transaction, Guid requestingUser); 
        Task<bool> DeleteAsync(Transaction transaction, Guid requestingUser);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, Guid requestingUser);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId, string financialMonth, Guid requestingUser);
        Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId, int year, Guid requestingUser);
        Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId, int year, Guid requestingUser);
    }
}
