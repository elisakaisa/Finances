using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Repositories
{
    public class UserRepository(FinancesDbContext dbContext) : IUserRepository
    {
        public Task<User> CreateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Include(u => u.Household)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user ?? throw new KeyNotFoundException();
        }

        public Task<User> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
