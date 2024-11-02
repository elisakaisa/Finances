using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface IRepartitionService
    {
        Task<Repartition> GetMonthlyHouseholdRepartition(Guid householdId, string monthYear, Guid requestingUser);
        Task<List<Repartition>> GetYearlyHouseholdRepartition(Guid householdId, int year, Guid requestingUser);
    }
}
