using Common.Model;
using Common.Repositories.Interfaces;

namespace Common.Repositories
{
    public class CategorySubcategoryRepository : ICategoryRepository, ISubcategoryRepository
    {
        public Task<ICollection<Category>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Subcategory>> GetSubcategoryByCategoryIdAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        Task<ICollection<Subcategory>> ISubcategoryRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
