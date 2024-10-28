using Common.Model.DatabaseObjects;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class MonthlyIncomeAfterTaxService : BaseService, IMonthlyIncomeAfterTaxesService
    {
        public Task<MonthlyIncomeAfterTax> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, User user)
        {
            ValidateThatUserIsInHousehold(user, monthlyIncomeAfterTax.User.HouseholdId);
            throw new NotImplementedException();
        }

        public Task<MonthlyIncomeAfterTax> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, User user)
        {
            ValidateThatUserIsInHousehold(user, monthlyIncomeAfterTax.User.HouseholdId);
            throw new NotImplementedException();
        }
    }
}
