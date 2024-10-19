using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services.Interfaces;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using System.Runtime.Intrinsics.X86;

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

            return CalculateRepartition(household, monthlyIncomeAfterTax, householdTransactions);
        }

        private Repartition CalculateRepartition(Household household, ICollection<MonthlyIncomeAfterTax> monthlyIncomeAfterTax, ICollection<Transaction> householdTransactions)
        {
            var user1 = household.Users.First();
            var user2 = household.Users.Last();

            var users = household.Users.ToDictionary(user => user.Id, user => user);

            var incomeUser = users.ToDictionary(
                user => user.Key, 
                user => GetMonthlyIncomeForUser(monthlyIncomeAfterTax, user.Value.Id));

            var householdIncome = incomeUser.Values.Sum();
            var userSharesOfHouseholdIncome = CalculateHouseholdIncomeShares(incomeUser, householdIncome);

            var commonExpensesPaidByUser = users.ToDictionary(user => user.Key, user => 0m);
            var userShouldPay = users.ToDictionary(user => user.Key, user => 0m);

            Repartition repartition = new()
            {
                Household = household,
                Users = users,
                IncomeAfterTax = incomeUser,
                UserSharesOfHouseholdIncome = userSharesOfHouseholdIncome,
                TotalCommonExpenses = Math.Round(0m, 3),
                TotalCommonExpensesPaidByUser = commonExpensesPaidByUser,
                UserShouldPay = userShouldPay,
                // TODO:target shares
            };

            ProcessTransactions(householdTransactions, repartition, user1.Id, user2.Id);

            return RoundRepartitionSums(repartition);
        }

        private void ProcessTransactions(ICollection<Transaction> householdTransactions, Repartition repartition, Guid user1Id, Guid user2Id)
        {
            foreach (var transaction in householdTransactions.Where(t => t.IsTransactionCommon()))
            {
                repartition.TotalCommonExpenses += transaction.Amount;

                repartition.TotalCommonExpensesPaidByUser[transaction.UserId] += transaction.Amount;

                switch (transaction.SplitType)
                {
                    case SplitType.Even:
                        repartition.UserShouldPay[user1Id] += transaction.Amount * 0.5m;
                        repartition.UserShouldPay[user2Id] += transaction.Amount * 0.5m;
                        break;
                    case SplitType.IncomeBased:
                        repartition.UserShouldPay[user1Id] += transaction.Amount * repartition.UserSharesOfHouseholdIncome[user1Id];
                        repartition.UserShouldPay[user2Id] += transaction.Amount * repartition.UserSharesOfHouseholdIncome[user2Id];
                        break;
                    case SplitType.Custom:
                        var userShare = (decimal)transaction.UserShare;
                        var user1Share = transaction.UserId == user1Id ? userShare : (1 - userShare);
                        var user2Share = 1 - userShare;
                        repartition.UserShouldPay[user1Id] += transaction.Amount * user1Share;
                        repartition.UserShouldPay[user2Id] += transaction.Amount * user2Share;
                        break;
                }
            }
        }

        private Repartition RoundRepartitionSums(Repartition repartition)
        {
            foreach (var userId in repartition.Users.Keys.ToList())
            {
                repartition.TotalCommonExpensesPaidByUser[userId] = Math.Round(repartition.TotalCommonExpensesPaidByUser[userId], 3);
                repartition.UserShouldPay[userId] = Math.Round(repartition.UserShouldPay[userId], 3);
            }
            return repartition;
        }

        private Dictionary<Guid, decimal> CalculateHouseholdIncomeShares(Dictionary<Guid, decimal> incomeUser, decimal householdIncome)
        {
            return incomeUser.ToDictionary(
                kvp => kvp.Key,
                kvp => householdIncome == 0 ? 0.5m : kvp.Value / householdIncome
            );
        }

        private decimal GetMonthlyIncomeForUser(ICollection<MonthlyIncomeAfterTax> monthlyIncomes, Guid userId)
        {
            return monthlyIncomes.First(i => i.UserId == userId).IncomeAfterTax;
        }
    }
}
