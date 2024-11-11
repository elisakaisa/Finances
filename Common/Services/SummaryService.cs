using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class SummaryService : ISummaryService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly IHouseholdRepository _householdRepository;

        public SummaryService(ITransactionRepository transactionRepo, ISubcategoryRepository subcategoryRepo, IHouseholdRepository householdRepo)
        {
            _transactionRepository = transactionRepo;
            _subcategoryRepository = subcategoryRepo;
            _householdRepository = householdRepo;
        }

        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid requestingUserId)
        {
            ValidateFinancialMonth(financialMonth);
            var household = await _householdRepository.GetHouseholdByUserId(requestingUserId);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(household.Id, financialMonth);
            var subcategories = await _subcategoryRepository.GetAllAsync();

            return CalculateHouseholdLevelMonthlySummaries(transactions, subcategories, household, financialMonth);
        }

        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid requestingUserId)
        {
            var household = await _householdRepository.GetHouseholdByUserId(requestingUserId);

            var transactions = await _transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(household.Id, year);
            var subcategories = await _subcategoryRepository.GetAllAsync();

            var householdLevelMonthlySummaries = new List<HouseholdLevelMonthlySummary>();

            for (int month = 1; month <= 12; month++)
            {
                string financialMonth = $"{year}{month:D2}";
                var householdLevelMonthlySummary = CalculateHouseholdLevelMonthlySummaries(transactions, subcategories, household, financialMonth);
                householdLevelMonthlySummaries.AddRange(householdLevelMonthlySummary);
            }

            return householdLevelMonthlySummaries;
        }

        private void ValidateFinancialMonth(string financialMonth)
        {
            if (!financialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }
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
