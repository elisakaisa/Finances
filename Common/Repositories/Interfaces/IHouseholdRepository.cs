using Common.Model.DatabaseObjects;

namespace Common.Repositories.Interfaces
{
    public interface IHouseholdRepository : IRepository<Household>
    {
        Task<Household> GetHouseholdByUserId(Guid userId);
    }
}
