using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class SummaryService(ITransactionRepository transactionRepository, ISubcategoryRepository subcategoryRepository, IHouseholdRepository householdRepository) : ISummaryService
    {
        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid requestingUserId)
        {
            financialMonth.ValidateFinancialMonthFormat();
            var household = await householdRepository.GetHouseholdByUserId(requestingUserId) ?? throw new KeyNotFoundException();

            var transactions = await transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(household.Id, financialMonth);
            var subcategories = await subcategoryRepository.GetAllAsync();

            return CalculateHouseholdLevelMonthlySummaries(transactions, subcategories, household, financialMonth);
        }

        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid requestingUserId)
        {
            var household = await householdRepository.GetHouseholdByUserId(requestingUserId) ?? throw new KeyNotFoundException();

            var transactions = await transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(household.Id, year);
            var subcategories = await subcategoryRepository.GetAllAsync();

            var householdLevelMonthlySummaries = new List<HouseholdLevelMonthlySummary>();

            for (int month = 1; month <= 12; month++)
            {
                string financialMonth = $"{year}{month:D2}";
                var householdLevelMonthlySummary = CalculateHouseholdLevelMonthlySummaries(transactions, subcategories, household, financialMonth);
                householdLevelMonthlySummaries.AddRange(householdLevelMonthlySummary);
            }

            return householdLevelMonthlySummaries;
        }


        private static List<HouseholdLevelMonthlySummary> CalculateHouseholdLevelMonthlySummaries(ICollection<Transaction> transactions, ICollection<Subcategory> subcategories, Household household, string financialMonth)
        {
            return subcategories.Select(subcategory =>
                CalculateHouseholdSummaryForSubcategory(subcategory, transactions, household, financialMonth))
                .ToList();
        }

        private static HouseholdLevelMonthlySummary CalculateHouseholdSummaryForSubcategory(Subcategory subcategory, ICollection<Transaction> transactions, Household household, string financialMonth)
        {
            var userLevelMonthlySummaries = household.Users
                .Select(user => CalculateUserSummaryForSubcategory(transactions, user, subcategory))
                .ToList();

            var totalSum = userLevelMonthlySummaries.Sum(summary => summary.Total);
            var commonHouseholdSum = userLevelMonthlySummaries.Sum(summary => summary.CommonTotal);

            return new HouseholdLevelMonthlySummary
            {
                FinancialMonth = financialMonth,
                Year = int.Parse(financialMonth[..4]),
                TransactionType = subcategory.TransactionType.ToString(),
                SubcategoryName = subcategory.Name,
                CategoryName = subcategory.Category.Name,
                Total = totalSum,
                CommonTotal = commonHouseholdSum,
                UserLevelMonthlySummary = userLevelMonthlySummaries,
                HouseholdId = household.Id
            };
        }

        private static UserLevelMonthlySummary CalculateUserSummaryForSubcategory(ICollection<Transaction> transactions, User user,  Subcategory subcategory)
        {
            var userTransactionsBySubcategory = transactions.Where(t => t.UserId == user.Id && t.SubcategoryId == subcategory.Id);
            var individualTotal = userTransactionsBySubcategory.Where(t => t.SplitType == SplitType.Individual).Sum(t => t.Amount);
            var commonTotal = userTransactionsBySubcategory.Where(t => t.SplitType != SplitType.Individual).Sum(t => t.Amount);

            return new UserLevelMonthlySummary
            {
                CommonTotal = commonTotal,
                IndividualTotal = individualTotal,
                Total = commonTotal + individualTotal,
                UserId = user.Id
            };
        }
    }
}
