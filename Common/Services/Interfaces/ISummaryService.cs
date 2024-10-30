using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface ISummaryService
    {
        Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid householdId, User user);
        Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid householdId, User user);
    }
}
