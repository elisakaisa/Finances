using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class HouseholdRepository(FinancesDbContext dbContext) : IHouseholdRepository
    {
        public Task<Household> CreateAsync(Household entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Household entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Household> GetByIdAsync(Guid id)
        {
            var household = await dbContext.Households
                            .AsNoTracking()
                            .Include(h => h.Users)
                            .FirstOrDefaultAsync(h => h.Id == id);

            return household ?? throw new KeyNotFoundException($"Transaction with ID {id} was not found.");
        }

        public async Task<Household?> GetHouseholdByUserId(Guid userId)
        {
            return await dbContext.Households
                .AsNoTracking()
                .Include(h => h.Users)
                .FirstOrDefaultAsync(h => h.Users.Any(u => u.Id == userId));
        }

        public Task<Household> UpdateAsync(Household entity)
        {
            throw new NotImplementedException();
        }
    }
}
