using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();
        Task<ICollection<Category>> GetAllAsyncByTransactionType(TransactionType type);
    }
}
