using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Tests.TestData
{
    public class TestDataBuilder : GeneralTestData
    {
        public Household GetHouseholdWith1User()
        {
            return new()
            {
                Id = Household1Id,
                Name = "Household10",
                Users = [new User() { Id = User1Hh1Id, Name = "user1" }]
            };
        }

        public Household GetHouseholdWith2Users()
        {
            return new()
            {
                Id = Household1Id,
                Name = "Household10",
                Users = [new User() { Id = User1Hh1Id, Name = "user1" }, new User() { Id = User2Hh1Id, Name = "user2" }]
            };
        }

        public Household GetHouseholdWith3Users()
        {
            return new()
            {
                Id = Household1Id,
                Name = "Household10",
                Users = [new User() { Id = User1Hh1Id, Name = "user1" }, new User() { Id = User2Hh1Id, Name = "user2" }, new User() { Id = User1Hh1Id, Name = "user3" }]
            };
        }

        public List<Transaction> GetTransactionsForSingleHousehold() =>
        [
            CreateTransaction(123.45m, TransactionType.Expenses),
            CreateTransaction(13.12m, TransactionType.Expenses),
            CreateTransaction(102m, TransactionType.Savings),
            CreateTransaction(-50.2m, TransactionType.Expenses),
            CreateTransaction(10000m, TransactionType.Income)
        ];

        public List<Transaction> GetTransactionsWithOnlyEvenSplits(decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1) =>
        [
            CreateTransaction(expense1U1, TransactionType.Expenses, SplitType.Even),
            CreateTransaction(expense2U1, TransactionType.Expenses, SplitType.Even),
            CreateTransaction(expense1U2, TransactionType.Expenses, SplitType.Even, User2Hh1Id, User12),
            CreateTransaction(102m, TransactionType.Savings, SplitType.Even),
            CreateTransaction(-payback1U1, TransactionType.Expenses, SplitType.Even),
            CreateTransaction(1234m, TransactionType.Income, SplitType.Even)

        ];

        public List<Transaction> GetTransactionsWithOnlyIncomeBasedSplits(decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1) =>
        [
            CreateTransaction(expense1U1, TransactionType.Expenses, SplitType.IncomeBased),
            CreateTransaction(expense2U1, TransactionType.Expenses, SplitType.IncomeBased),
            CreateTransaction(expense1U2, TransactionType.Expenses, SplitType.IncomeBased, User2Hh1Id, User12),
            CreateTransaction(102m, TransactionType.Savings, SplitType.IncomeBased),
            CreateTransaction(-payback1U1, TransactionType.Expenses, SplitType.IncomeBased),
            CreateTransaction(1234m, TransactionType.Income, SplitType.IncomeBased)
        ];

        public List<Transaction> GetSingleTransactionByUser1WithCustomSplit(decimal amount, decimal userShare) =>
        [
            CreateTransaction(amount, TransactionType.Expenses, SplitType.Custom, userShare: userShare)
        ];

        public List<Transaction> GetTwoTransactionByUser1WithCustomSplit(decimal amount1, decimal userShare1, decimal amount2, decimal userShare2) =>
        [
            CreateTransaction(amount1, TransactionType.Expenses, SplitType.Custom, userShare: userShare1),
            CreateTransaction(amount2, TransactionType.Expenses, SplitType.Custom, userShare: userShare2)
        ];

        public List<Transaction> GetSingleTransactionByUser1WithSplitByIncome(decimal amount) =>
        [
            CreateTransaction(amount, TransactionType.Expenses, SplitType.IncomeBased)
        ];

        public List<Transaction> GetTwoTransactionByUser1WithSplitByIncome(decimal amount1, decimal amount2) =>
        [
            CreateTransaction(amount1, TransactionType.Expenses, SplitType.IncomeBased),
            CreateTransaction(amount2, TransactionType.Expenses, SplitType.IncomeBased)
        ];

        public List<Transaction> GetMultipleTransactionsByUser1WithVariousSplits(decimal amountIncomeBased, decimal amountEven, decimal amountCustom, decimal shareCustom, decimal amountIndividual) =>
        [
            CreateTransaction(amountIncomeBased, TransactionType.Expenses, SplitType.IncomeBased),
            CreateTransaction(amountEven, TransactionType.Expenses, SplitType.Even),
            CreateTransaction(amountCustom, TransactionType.Expenses, SplitType.Custom, userShare: shareCustom),
            CreateTransaction(amountIndividual, TransactionType.Expenses, SplitType.Individual)
        ];

        public List<Transaction> GetFourTransactionsByTwoUsersWithThreeSubcategories() =>
        [
            CreateTransaction(20m, TransactionType.Expenses, SplitType.Even, subcategory: Electricity, subcategoryId: Electricity.Id),
            CreateTransaction(40m, TransactionType.Income, SplitType.Individual, subcategory: Salary, subcategoryId: Salary.Id),
            CreateTransaction(35m, TransactionType.Expenses, SplitType.Individual, subcategory: Electricity, subcategoryId: Electricity.Id, userId: User2Hh1Id, user: User12),
            CreateTransaction(25m, TransactionType.Income, SplitType.Individual, subcategory: IncomeMisc, subcategoryId: IncomeMisc.Id, userId: User2Hh1Id, user: User12),
        ];

        public List<Transaction> GetFourTransactionsByTwoUsersWithOneSubcategories(decimal amountEven1, decimal amountIncome, decimal amountEven2, decimal amountIndividual) =>
        [
            CreateTransaction(amountEven1, TransactionType.Expenses, SplitType.Even, subcategory: Electricity, subcategoryId: Electricity.Id),
            CreateTransaction(amountIncome, TransactionType.Expenses, SplitType.IncomeBased, subcategory: Electricity, subcategoryId: Electricity.Id),
            CreateTransaction(amountEven2, TransactionType.Expenses, SplitType.Even, subcategory: Electricity, subcategoryId: Electricity.Id, userId: User2Hh1Id, user: User12),
            CreateTransaction(amountIndividual, TransactionType.Expenses, SplitType.Individual, subcategory: Electricity, subcategoryId: Electricity.Id, userId: User2Hh1Id, user: User12),
        ];


        public List<Subcategory> GetListOfFourSubCategories() =>
        [
            Electricity, HomeInsurance, Salary, IncomeMisc
        ];

        public List<MonthlyIncomeAfterTax> GetMonthlyIncomesAfterTax(decimal income1, decimal income2) =>
        [
            CreateMonthlyIncome(income1, User1Hh1Id, User11),
            CreateMonthlyIncome(income2, User2Hh1Id, User12)
        ];

        public List<MonthlyIncomeAfterTax> GetMonthlyIncomesAfterTaxForOneUserHousehold(decimal income1) =>
        [
            CreateMonthlyIncome(income1, User1Hh1Id, User11)
        ];
    }
}
