using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Tests.IRepartitionService.TestData
{
    public class HouseholdRepartitionTestData
    {
        public List<Transaction> GetTransactionsForSingleHousehold()
        {
            return
            [
                new()
                {
                    Amount = 123.45m,
                    TransactionType = TransactionType.Expenses,
                },
                new()
                {
                    Amount = 13.12m,
                    TransactionType = TransactionType.Expenses,
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                },
                new()
                {
                    Amount = -50.2m,
                    TransactionType = TransactionType.Expenses,
                },
                new()
                {
                    Amount = 10000m,
                    TransactionType = TransactionType.Income,
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
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = expense2U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = expense1U2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User2Hh1Id
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = -payback1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = 1234,
                    TransactionType = TransactionType.Income,
                    SplitType = SplitType.Even,
                    UserId = GeneralTestData.User1Hh1Id
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
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = expense2U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = expense1U2,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User2Hh1Id
                },
                new()
                {
                    Amount = 102m,
                    TransactionType = TransactionType.Savings,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = -payback1U1,
                    TransactionType = TransactionType.Expenses,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id
                },
                new()
                {
                    Amount = 1234,
                    TransactionType = TransactionType.Income,
                    SplitType = SplitType.IncomeBased,
                    UserId = GeneralTestData.User1Hh1Id
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
                    UserId = GeneralTestData.User1Hh1Id
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
                    UserId = GeneralTestData.User1Hh1Id
                },
                new ()
                {
                    IncomeAfterTax = income2,
                    UserId = GeneralTestData.User1Hh1Id
                }
            ];
        }
    }
}
