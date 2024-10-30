using Common.Model.DatabaseObjects;
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

            var householdLevelMonthlySummaries = new List<HouseholdLevelMonthlySummary>();
            

            foreach (var subcategory in subcategories)
            {
                var totalSum = 0m;
                var commonHouseholdSum = 0m;
                UserLevelMonthlySummary userLevelMonthlySummary;
                var userLevelMonthlySummaries = new List<UserLevelMonthlySummary>();

                foreach (var userInHousehold in household.Users)
                {
                    var transactionsByUserAndSubcategory = transactions.Where(t => t.UserId == userInHousehold.Id && t.Subcategory == subcategory);
                    totalSum += transactionsByUserAndSubcategory.Sum(t => t.Amount);
                    var commonPaidByUser = transactionsByUserAndSubcategory.Where(t => t.SplitType != Model.Enums.SplitType.Individual).Sum(t => t.Amount);
                    var individualPaidByUser = transactionsByUserAndSubcategory.Where(t => t.SplitType == Model.Enums.SplitType.Individual).Sum(t => t.Amount);
                    commonHouseholdSum += commonPaidByUser;

                    userLevelMonthlySummary = new UserLevelMonthlySummary()
                    {
                        CommonTotal = commonPaidByUser,
                        IndividualTotal = individualPaidByUser,
                        Total = commonPaidByUser + individualPaidByUser,
                        User = userInHousehold
                    };
                    userLevelMonthlySummaries.Add(userLevelMonthlySummary);
                }


                var transactionSum = new HouseholdLevelMonthlySummary
                { 
                    FinancialMonth = financialMonth,
                    Year = int.Parse(financialMonth.Substring(0, 4)),
                    TransactionType = subcategory.TransactionType,
                    CategoryId = subcategory.CategoryId,
                    SubcategoryId = subcategory.Id,
                    Total = totalSum,
                    CommonTotal = commonHouseholdSum,
                    UserLevelMonthlySummary = userLevelMonthlySummaries,
                    Household = household,
                };
                householdLevelMonthlySummaries.Add(transactionSum);
            }


            return householdLevelMonthlySummaries;
        }

        public Task<List<HouseholdLevelMonthlySummary>> GetMonthlyTransactionsByYearAndHouseholdId(int year, Guid householdId, User user)
        {
            ValidateThatUserIsInHousehold(user, householdId);
            throw new NotImplementedException();
        }
    }
}
