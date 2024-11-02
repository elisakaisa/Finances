using Common.Model.DatabaseObjects;

namespace Common.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<ICollection<Subcategory>> GetAllAsync();
        Task<ICollection<Subcategory>> GetSubcategoryByCategoryIdAsync(int categoryId);
        Task<Subcategory> GetSubcategoryByName(string name);
    }
}
