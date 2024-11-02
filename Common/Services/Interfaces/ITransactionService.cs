using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateAsync(TransactionDto transactionDto, Guid requestingUserId);
        Task<Transaction> UpdateAsync(TransactionDto transactionDto, Guid requestingUserId); 
        Task<bool> DeleteAsync(Transaction transactionDto, Guid requestingUserId);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, Guid requestingUserId);
        Task<ICollection<Transaction>> GetMonthlyTransactionsByHouseholdId(Guid householdId, string financialMonth, Guid requestingUserId);
        Task<ICollection<Transaction>> GetYearlyTransactionsByUserId(Guid userId, int year, Guid requestingUserId);
        Task<ICollection<Transaction>> GetYearlyTransactionsByHouseholdId(Guid householdId, int year, Guid requestingUserId);
    }
}
