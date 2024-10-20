using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface IRepartitionService
    {
        Task<Repartition> GetMonthlyHouseholdRepartition(Guid householdId, string monthYear);
        Task<List<Repartition>> GetYearlyHouseholdRepartition(Guid householdId, int year);
    }
}
