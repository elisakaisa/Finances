using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface IRepartitionService
    {
        Task<Repartition> GetMonthlyHouseholdRepartition(string monthYear, Guid requestingUser);
        Task<List<Repartition>> GetYearlyHouseholdRepartition(int year, Guid requestingUser);
    }
}
