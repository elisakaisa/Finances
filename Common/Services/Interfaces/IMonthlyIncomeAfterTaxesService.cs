using Common.Model.DatabaseObjects;

namespace Common.Services.Interfaces
{
    public interface IMonthlyIncomeAfterTaxesService
    {
        Task<MonthlyIncomeAfterTax> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, Guid requestingUserId);
        Task<MonthlyIncomeAfterTax> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTax monthlyIncomeAfterTax, Guid requestingUserId);
    }
}
