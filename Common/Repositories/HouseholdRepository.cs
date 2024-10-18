using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;

namespace Common.Repositories
{
    public class HouseholdRepository : IHouseholdRepository
    {
        public Task<Household> CreateAsync(Household entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Household entity)
        {
            throw new NotImplementedException();
        }

        public Task<Household> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Household> UpdateAsync(Household entity)
        {
            throw new NotImplementedException();
        }
    }
}
