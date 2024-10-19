using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;

namespace Common.Services
{
    public class RepartitionService : IRepartitionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IFinancialMonthRepository _financialMonthRepository;
        private readonly IMonthlyIncomeAfterTaxRepository _monthlyIncomeAfterTaxRepository;
        private readonly IHouseholdRepository _householdRepository;

        public RepartitionService(
            ITransactionRepository transactionRepo, IFinancialMonthRepository financialMonthRepo,
            IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepo, IHouseholdRepository householdRepo) 
        { 
            _transactionRepository = transactionRepo;
            _financialMonthRepository = financialMonthRepo;
            _monthlyIncomeAfterTaxRepository = monthlyIncomeAfterTaxRepo;
            _householdRepository = householdRepo;
        }

        public async Task<Repartition> GetMonthlyHouseholdRepartition(Guid householdId, string monthYear)
        {
            var household = await _householdRepository.GetByIdAsync(householdId);
            if (household.Users.Count > 2) 
            {
                throw new HouseholdWithMoreThanTwoUsersNotSupportedException();
            }

            if (household.Users.Count == 1)
            {
                throw new NotImplementedException();
            }

            var householdTransactions = await _transactionRepository.GetMonthlyTransactionsByHouseholdIdAsync(householdId, monthYear);
            var monthlyIncomeAfterTax = await _monthlyIncomeAfterTaxRepository.GetMonthlyIncomeAfterTaxByHouseholdIdAndByMonthAsync(householdId, monthYear);

            var user1 = household.Users.First();
            var user2 = household.Users.Last();

            var incomeUser1 = GetMonthlyIncomeForUser(monthlyIncomeAfterTax, user1.Id);
            var incomeUser2 = GetMonthlyIncomeForUser(monthlyIncomeAfterTax, user2.Id);

            var householdIncome = incomeUser1 + incomeUser2;

            var userSharesOfHouseholdIncome = new Dictionary<Guid, decimal>
            {
                { user1.Id, householdIncome == 0 ? 0.5m : incomeUser1 / householdIncome },
                { user2.Id, householdIncome == 0 ? 0.5m : incomeUser2 / householdIncome },
            };

            var totalCommonExpenses = 0m;

            var commonExpensesPaidByUser = new Dictionary<Guid, decimal>
            {
                { user1.Id, 0m },
                { user2.Id, 0m }
            };
            var userShouldPay = new Dictionary<Guid, decimal>
            {
                { user1.Id, 0m },
                { user2.Id, 0m }
            };

            foreach (var transaction in householdTransactions.Where(t => t.IsTransactionCommon()))
            {
                totalCommonExpenses += transaction.Amount;

                if (commonExpensesPaidByUser.ContainsKey(transaction.UserId))
                {
                    commonExpensesPaidByUser[transaction.UserId] += transaction.Amount;
                }

                if (transaction.SplitType == SplitType.Even)
                {
                    userShouldPay[user1.Id] += transaction.Amount * 0.5m;
                    userShouldPay[user2.Id] += transaction.Amount * 0.5m;
                }
                else if (transaction.SplitType == SplitType.IncomeBased)
                {
                    userShouldPay[user1.Id] += transaction.Amount * userSharesOfHouseholdIncome[user1.Id];
                    userShouldPay[user2.Id] += transaction.Amount * userSharesOfHouseholdIncome[user2.Id];
                }
                else if (transaction.SplitType == SplitType.Custom)
                {
                    var userShare = (decimal) transaction.UserShare;
                    var user1Share = transaction.UserId == user1.Id ? userShare : (1 - userShare);
                    var user2Share = 1 - userShare;
                    userShouldPay[user1.Id] += transaction.Amount * user1Share;
                    userShouldPay[user2.Id] += transaction.Amount * user2Share;
                }
            }

            Repartition repartition = new()
            {
                Household = household,
                User1 = household.Users.First(),
                User2 = household.Users.Last(),
                IncomeAfterTaxUser1 = Math.Round(incomeUser1, 3),
                IncomeAfterTaxUser2 = Math.Round(incomeUser2, 3),
                TotalCommonExpenses = Math.Round(totalCommonExpenses, 3),
                TotalCommonExpensesPaidByUser1 = Math.Round(commonExpensesPaidByUser[user1.Id], 3),
                TotalCommonExpensesPaidByUser2 = Math.Round(commonExpensesPaidByUser[user2.Id], 3),
                User1ShouldPay = Math.Round(userShouldPay[user1.Id], 3),
                User2ShouldPay = Math.Round(userShouldPay[user2.Id], 3),
                // TODO:
                TargetShareUser1 = 0,
                TargetShareUser2 = 0,
                ActualShareUser1 = 0,
                ActualShareUser2 = 0,
            };

            return repartition;
        }

        private decimal GetMonthlyIncomeForUser(ICollection<MonthlyIncomeAfterTax> monthlyIncomes, Guid userId)
        {
            return monthlyIncomes.First(i => i.UserId == userId).IncomeAfterTax;
        }
    }
}
