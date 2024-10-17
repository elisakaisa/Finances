using Common.Model;

namespace Common.Repositories.Interfaces
{
    public interface IMonthlyIncomeAfterTaxRepository : IRepository<MonthlyIncomeAfterTax>
    {
        Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAndByMonthAsync(Guid userId, string monthYear);
        Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAndByMonthAsync(Guid userId, string monthYear);
        Task<MonthlyIncomeAfterTax> GetMonthlyIncomeAfterTaxByUserIdAndByYearAsync(Guid userId, int year);
        Task<ICollection<MonthlyIncomeAfterTax>> GetMonthlyIncomeAfterTaxByHouseholdIdAndByYearAsync(Guid userId, int year);
    }
}
