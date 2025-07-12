using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateAsync(TransactionDto transactionDto, Guid requestingUserId);
        Task<Transaction> UpdateAsync(TransactionDto transactionDto, Guid requestingUserId);
        Task<bool> DeleteAsync(TransactionDto transactionDto, Guid requestingUserId);
        Task<ICollection<TransactionDto>> GetMonthlyTransactionsByUserId(Guid userId, string financialMonth, Guid requestingUserId);
        Task<ICollection<TransactionDto>> GetMonthlyTransactionsByHousehold(string financialMonth, Guid requestingUserId);
        Task<ICollection<TransactionDto>> GetYearlyTransactionsByUserId(Guid userId, int year, Guid requestingUserId);
        Task<ICollection<TransactionDto>> GetYearlyTransactionsByHousehold(int year, Guid requestingUserId);
    }
}
