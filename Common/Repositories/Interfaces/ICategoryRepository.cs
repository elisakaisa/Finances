using Common.Model.DatabaseObjects;

namespace Common.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();
    }
}
