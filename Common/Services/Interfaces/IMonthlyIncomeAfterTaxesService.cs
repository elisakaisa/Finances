using Common.Model.DatabaseObjects;

namespace Common.Services.Interfaces
{
    public interface IMonthlyIncomeAfterTaxesService
    {
        Task<MonthlyIncomeAfterTax> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, User user);
        Task<MonthlyIncomeAfterTax> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, User user);
    }
}
