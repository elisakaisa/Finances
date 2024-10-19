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

            var incomeUser1 = monthlyIncomeAfterTax.First(i => i.UserId == user1.Id).IncomeAfterTax;
            var incomeUser2 = monthlyIncomeAfterTax.First(i => i.UserId == user2.Id).IncomeAfterTax;

            //TODO: turn these into extension method
            var householdIncome = incomeUser1 + incomeUser2;
            var user1ShareOfHouseholdIncome = 0.5m;
            var user2ShareOfHouseholdIncome = 0.5m;
            if (householdIncome != 0)
            {
                user1ShareOfHouseholdIncome = incomeUser1 / householdIncome;
                user2ShareOfHouseholdIncome = incomeUser2 / householdIncome;
            }

            var user1ShouldPay = 0m;
            var user2ShouldPay = 0m;
            var totalCommonExpenses = 0m;
            var commonExpensesPaidByUser1 = 0m;
            var commonExpensesPaidByUser2 = 0m;
            foreach (var transaction in householdTransactions.Where(t => t.IsTransactionCommon()))
            {
                totalCommonExpenses += transaction.Amount;

                if (transaction.UserId == user1.Id) 
                {
                    commonExpensesPaidByUser1 += transaction.Amount;
                    if (transaction.SplitType == SplitType.Even)
                    {
                        user1ShouldPay += transaction.Amount/2;
                        user2ShouldPay += transaction.Amount/2;
                    }
                    else if (transaction.SplitType == SplitType.IncomeBased)
                    {
                        user1ShouldPay += transaction.Amount * user1ShareOfHouseholdIncome;
                        user2ShouldPay += transaction.Amount * user2ShareOfHouseholdIncome;
                    }
                    else if (transaction.SplitType == SplitType.Custom)
                    {
                        var user1Share = transaction.UserShare;
                        user1ShouldPay += transaction.Amount * (decimal)user1Share;
                        user2ShouldPay += transaction.Amount * (decimal)(1-user1Share);
                    }
                }
                else if (transaction.UserId == user2.Id)
                {
                    commonExpensesPaidByUser2 += transaction.Amount;
                    if (transaction.SplitType == SplitType.Even)
                    {
                        user1ShouldPay += transaction.Amount / 2;
                        user2ShouldPay += transaction.Amount / 2;
                    }
                    else if (transaction.SplitType == SplitType.IncomeBased)
                    {
                        user1ShouldPay += transaction.Amount * user1ShareOfHouseholdIncome;
                        user2ShouldPay += transaction.Amount * user2ShareOfHouseholdIncome;
                    }
                    else if (transaction.SplitType == SplitType.Custom)
                    {
                        var user2Share = transaction.UserShare;
                        user1ShouldPay += transaction.Amount * (decimal)(1 - user2Share);
                        user2ShouldPay += transaction.Amount * (decimal)user2Share;
                    }
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
                TotalCommonExpensesPaidByUser1 = Math.Round(commonExpensesPaidByUser1, 3),
                TotalCommonExpensesPaidByUser2 = Math.Round(commonExpensesPaidByUser2, 3),
                User1ShouldPay = Math.Round(user1ShouldPay, 3),
                User2ShouldPay = Math.Round(user2ShouldPay, 3),
                // TODO:
                TargetShareUser1 = 0,
                TargetShareUser2 = 0,
                ActualShareUser1 = 0,
                ActualShareUser2 = 0,
            };

            return repartition;
        }
    }
}
