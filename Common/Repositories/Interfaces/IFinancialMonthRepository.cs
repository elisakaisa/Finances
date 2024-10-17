using Common.Model;

namespace Common.Repositories.Interfaces
{
    public interface IFinancialMonthRepository
    {
        Task<FinancialMonth> UpdateAsync(FinancialMonth financialMonth);
        Task<FinancialMonth> CreateAsync(FinancialMonth financialMonth);
        Task<FinancialMonth> GetFinancialMonthByHouseholdIdAsync(Guid householdId);
        Task<FinancialMonth> GetFinancialMonthByUserIdAsync(User userId);
        Task<FinancialMonth> GetYearlyFinancialMonthByHouseholdIdAsync(Guid householdId);
        Task<FinancialMonth> GetYearlyFinancialMonthByUserIdAsync(User userId);
    }
}
