using Common.Model.Dtos;

namespace Common.Services.Interfaces
{
    public interface IMonthlyIncomeAfterTaxesService
    {
        Task<MonthlyIncomeAfterTaxDto> AddMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId);
        Task<MonthlyIncomeAfterTaxDto> UpdateMonthlyIncomeAfterTaxAsync(MonthlyIncomeAfterTaxDto monthlyIncomeAfterTax, Guid requestingUserId);
    }
}
