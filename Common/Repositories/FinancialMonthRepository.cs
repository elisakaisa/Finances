using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;

namespace Common.Repositories
{
    internal class FinancialMonthRepository : IFinancialMonthRepository
    {
        public Task<FinancialMonth> CreateAsync(FinancialMonth financialMonth)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialMonth> GetFinancialMonthByHouseholdIdAsync(Guid householdId)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialMonth> GetFinancialMonthByUserIdAsync(User userId)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialMonth> GetYearlyFinancialMonthByHouseholdIdAsync(Guid householdId)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialMonth> GetYearlyFinancialMonthByUserIdAsync(User userId)
        {
            throw new NotImplementedException();
        }

        public Task<FinancialMonth> UpdateAsync(FinancialMonth financialMonth)
        {
            throw new NotImplementedException();
        }
    }
}
