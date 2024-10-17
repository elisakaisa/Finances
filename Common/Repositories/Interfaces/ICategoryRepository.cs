using Common.Model;

namespace Common.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();
    }
}
