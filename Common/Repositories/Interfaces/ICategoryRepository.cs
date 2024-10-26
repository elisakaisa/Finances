using Common.Model.DatabaseObjects;

namespace Common.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAllAsync();
        Task<ICollection<Subcategory>> GetCategorysSubcategories(int categoryId);
    }
}
