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
        private readonly IMonthlyIncomeAfterTaxRepository _monthlyIncomeAfterTaxRepository;
        private readonly IHouseholdRepository _householdRepository;

        public RepartitionService(
            ITransactionRepository transactionRepo,
            IMonthlyIncomeAfterTaxRepository monthlyIncomeAfterTaxRepo, IHouseholdRepository householdRepo) 
        { 
            _transactionRepository = transactionRepo;
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
            var monthlyIncomeAfterTax = await _monthlyIncomeAfterTaxRepository.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(householdId, monthYear);

            return CalculateMonthlyRepartition(household, monthlyIncomeAfterTax, householdTransactions, monthYear);
        }

        public async Task<List<Repartition>> GetYearlyHouseholdRepartition(Guid householdId, int year)
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

            return await CalculateYearlyRepartition(household, year);
        }

        private async Task<List<Repartition>> CalculateYearlyRepartition(Household household, int year)
        {
            var householdTransactions = await _transactionRepository.GetYearlyTransactionsByHouseholdIdAsync(household.Id, year);
            var monthlyIncomesAfterTax = await _monthlyIncomeAfterTaxRepository.GetYearlyIncomeAfterTaxByHouseholdIdAsync(household.Id, year);
            var financialMonths = InitializeFinancialMonths(year);

            var yearlyRepartition = new List<Repartition>();

            foreach (var financialMonth in financialMonths)
            {
                var monthlyTransactions = householdTransactions.Where(t => t.FinancialMonth == financialMonth).ToList();
                var monthlyIncomes = monthlyIncomesAfterTax.Where(i => i.FinancialMonth == financialMonth).ToList();
                var repartition = CalculateMonthlyRepartition(household, monthlyIncomes, monthlyTransactions, financialMonth);
                yearlyRepartition.Add(repartition);
            }
            return yearlyRepartition;
        }

        private Repartition CalculateMonthlyRepartition(Household household, ICollection<MonthlyIncomeAfterTax> monthlyIncomeAfterTax, ICollection<Transaction> householdTransactions, string monthYear)
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
                MonthYear = monthYear,
                IncomeAfterTax = incomeUser,
                UserSharesOfHouseholdIncome = userSharesOfHouseholdIncome,
                TotalCommonExpenses = Math.Round(0m, 3),
                TotalCommonExpensesPaidByUser = commonExpensesPaidByUser,
                UserShouldPay = userShouldPay,
                // TODO:target shares
            };

            ProcessMonthlyTransactions(householdTransactions, repartition, user1.Id, user2.Id);

            return RoundRepartitionSums(repartition);
        }

        private void ProcessMonthlyTransactions(ICollection<Transaction> householdTransactions, Repartition repartition, Guid user1Id, Guid user2Id)
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
            var monthlyIncome = monthlyIncomes.FirstOrDefault(i => i.UserId == userId);
            if (monthlyIncome == null) return 0m;
            return monthlyIncome.IncomeAfterTax;
        }

        private List<string> InitializeFinancialMonths(int year)
        {
            // TODO: this can be easily updated to show past 12 months instead of current year
            var financialMonths = new List<string>();

            for (int i = 1; i <= 12; i++)
            {
                financialMonths.Add($"{year}{i:D2}");
            }
            return financialMonths;
        }
    }
}
