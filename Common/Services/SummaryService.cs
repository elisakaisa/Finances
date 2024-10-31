using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Model.Dtos;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;

namespace Common.Services
{
    public class SummaryService : BaseService, ISummaryService
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

        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByMonthAndHouseholdId(string financialMonth, Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);

            var transactions = await _transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(householdId, financialMonth);
            var subcategories = await _subcategoryRepository.GetAllAsync();
            var household = await _householdRepository.GetByIdAsync(householdId);

            return CalculateHouseholdLevelMonthlySummaries(transactions, subcategories, household, financialMonth);
        }

        public async Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);

            var transactions = await _transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(householdId, year);
            var subcategories = await _subcategoryRepository.GetAllAsync();
            var household = await _householdRepository.GetByIdAsync(householdId);

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
                .Select(user => CalculateUserSummary(transactions, user, subcategory))
                .ToList();

            var totalSum = userLevelMonthlySummaries.Sum(summary => summary.Total);
            var commonHouseholdSum = userLevelMonthlySummaries.Sum(summary => summary.CommonTotal);

            return new HouseholdLevelMonthlySummary
            {
                FinancialMonth = financialMonth,
                Year = int.Parse(financialMonth[..4]),
                TransactionType = subcategory.TransactionType,
                Subcategory = subcategory,
                Total = totalSum,
                CommonTotal = commonHouseholdSum,
                UserLevelMonthlySummary = userLevelMonthlySummaries,
                Household = household
            };
        }

        private static UserLevelMonthlySummary CalculateUserSummary(ICollection<Transaction> transactions, User user,  Subcategory subcategory)
        {
            var userTransactions = transactions.Where(t => t.UserId == user.Id && t.Subcategory == subcategory);
            var individualTotal = userTransactions.Where(t => t.SplitType == SplitType.Individual).Sum(t => t.Amount);
            var commonTotal = userTransactions.Where(t => t.SplitType != SplitType.Individual).Sum(t => t.Amount);

            return new UserLevelMonthlySummary
            {
                CommonTotal = commonTotal,
                IndividualTotal = individualTotal,
                Total = commonTotal + individualTotal,
                User = user
            };
        }
    }
}
