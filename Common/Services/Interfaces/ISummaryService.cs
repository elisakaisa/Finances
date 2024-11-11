using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface ISummaryService
    {
        Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid requestingUser);
        Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid requestingUser);
    }
}
