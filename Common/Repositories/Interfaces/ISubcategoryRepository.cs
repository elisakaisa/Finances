using Common.Model;

namespace Common.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<ICollection<Subcategory>> GetAllAsync();
        Task<ICollection<Subcategory>> GetSubcategoryByCategoryIdAsync(int categoryId);
    }
}
