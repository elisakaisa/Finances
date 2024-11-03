using Common.Database;
using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Common.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FinancesDbContext _dbContext;

        public UserRepository(FinancesDbContext dbContext)
        {
            _dbContext = dbContext;
        }
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
            var user = await _dbContext.Users
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
