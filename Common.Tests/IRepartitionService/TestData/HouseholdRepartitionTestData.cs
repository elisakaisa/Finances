using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Tests.IRepartitionService.TestData
{
    public class HouseholdRepartitionTestData
    {
        public Household GetHouseholdWith1User()
        {
            return new()
            {
                Id = GeneralTestData.Household1Id,
                Name = "Household10",
                Users = [ new User() { Id = GeneralTestData.User1Hh1Id }]
            };
        }

        public Household GetHouseholdWith2Users()
        {
            return new()
            {
                Id = GeneralTestData.Household1Id,
                Name = "Household10",
                Users = [new User() { Id = GeneralTestData.User1Hh1Id }, new User() { Id = GeneralTestData.User2Hh1Id }]
            };
        }

        public Household GetHouseholdWith3Users()
        {
            return new()
            {
                Id = GeneralTestData.Household1Id,
                Name = "Household10",
                Users = [new User() { Id = GeneralTestData.User1Hh1Id }, new User() { Id = GeneralTestData.User2Hh1Id }, new User() { Id = GeneralTestData.User1Hh1Id }]
            };
        }

        public List<Transaction> GetTransactionsForSingleHousehold()
        {
            return
            [
                new()
                {
                    Amount = 123.45m,
                    TransactionType = TransactionType.Expenses,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = 13.12m,
                    TransactionType = TransactionType.Expenses,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = -50.2m,
                    TransactionType = TransactionType.Expenses,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = 10000m,
                    TransactionType = TransactionType.Income,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }

            ];
        }

        public List<Transaction> GetTransactionsWithOnlyEvenSplits(decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1)
        {
            return
            [
                new()
                {
                    Amount = expense1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = expense2U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = expense1U2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User2Hh1Id,
                    User = GeneralTestData.User12,
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = -payback1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = 1234,
                    TransactionType = TransactionType.Income,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }

            ];
        }

        public List<Transaction> GetTransactionsWithOnlyIncomeBasedSplits(decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1)
        {
            return
            [
                new()
                {
                    Amount = expense1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = expense2U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = expense1U2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User2Hh1Id,
                    User = GeneralTestData.User12,
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = -payback1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = 1234,
                    TransactionType = TransactionType.Income,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }

            ];
        }

        public List<Transaction> GetSingleTransactionByUser1WithCustomSplit(decimal amount, decimal userShare)
        {
            return
            [
                new()
                {
                    Amount = amount,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Custom,
                    UserShare = userShare,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }
            ];
        }

        public List<Transaction> GetTwoTransactionByUser1WithCustomSplit(decimal amount1, decimal userShare1, decimal amount2, decimal userShare2)
        {
            return
            [
                new()
                {
                    Amount = amount1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Custom,
                    UserShare = userShare1,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = amount2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Custom,
                    UserShare = userShare2,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }
            ];
        }

        public List<Transaction> GetSingleTransactionByUser1WithSplitByIncome(decimal amount)
        {
            return
            [
                new()
                {
                    Amount = amount,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }
            ];
        }

        public List<Transaction> GetTwoTransactionByUsersWithSplitByIncome(decimal amount1, decimal amount2)
        {
            return
            [
                new()
                {
                    Amount = amount1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = amount2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }
            ];
        }

        public List<Transaction> GetMultipleTransactionsByUser1WithVariousSplits(decimal amountIncomeBased, decimal amountEven, decimal amountCustom, decimal shareCustom, decimal amountIndividual)
        {
            return
            [
                new()
                {
                    Amount = amountIncomeBased,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = amountEven,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = amountCustom,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Custom,
                    UserShare = shareCustom,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new()
                {
                    Amount = amountIndividual,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Individual,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                }
            ];
        }

        public List<MonthlyIncomeAfterTax> GetMonthlyIncomesAfterTax(decimal income1, decimal income2)
        {
            return
            [
                new ()
                {
                    IncomeAfterTax = income1,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
                new ()
                {
                    IncomeAfterTax = income2,
                    UserId = GeneralTestData.User2Hh1Id,
                    User = GeneralTestData.User12,
                }
            ];
        }

        public List<MonthlyIncomeAfterTax> GetMonthlyIncomesAfterTaxForOneUserHousehold(decimal income1)
        {
            return
            [
                new ()
                {
                    IncomeAfterTax = income1,
                    UserId = GeneralTestData.User1Hh1Id,
                    User = GeneralTestData.User11,
                },
            ];
        }
    }
}
