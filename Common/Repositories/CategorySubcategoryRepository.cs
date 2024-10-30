using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class CategorySubcategoryRepository : ICategoryRepository, ISubcategoryRepository
    {
        public readonly FinancesDbContext _dbContext;

        public CategorySubcategoryRepository(FinancesDbContext context)
        {
            _dbContext = context;
        }

        async Task<ICollection<Category>> ICategoryRepository.GetAllAsync()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories;
        }

        async Task<ICollection<Category>> ICategoryRepository.GetAllAsyncByTransactionType(TransactionType type)
        {
            var categories = await _dbContext.Categories
                .AsNoTracking()
                .Include(c => c.Subcategories) //TODO: figure this out
                .Where(c => c.TransactionType == type)
                .ToListAsync();
            return categories;
        }

        public async Task<ICollection<Subcategory>> GetSubcategoryByCategoryIdAsync(int categoryId)
        {
            var categorysSubcategories = await _dbContext.Subcategories
                .AsNoTracking()
                .Include(s => s.Category)
                .Where(s => s.CategoryId == categoryId)
                .ToListAsync();
            return categorysSubcategories;
        }

        async Task<ICollection<Subcategory>> ISubcategoryRepository.GetAllAsync()
        {
            var subcategories = await _dbContext.Subcategories
                .AsNoTracking()
                .Include(s => s.Category)
                .ToListAsync();
            return subcategories;
        }
    }
}
