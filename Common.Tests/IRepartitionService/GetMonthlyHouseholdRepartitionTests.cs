using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.IRepartitionService.TestData;
using Common.Utils.Extensions;
using Moq;

namespace Common.Tests.IRepartitionService
{
    public class GetMonthlyHouseholdRepartitionTests : HouseholdRepartitionTestData
    {
        [SetUp]
        public void Setup()
        {
        }

        //[TestCase(12, 4, ExpectedResult = 3)]
        // TODO: add test cases with different amount values
        [Test]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHasOneUser()
        {
            // Arrange
            var testTransactions = GetTransactionsForSingleHousehold();
            var testUserId = Guid.NewGuid();

            var transactionRepo = new Mock<ITransactionRepository>();
            var financialMonthRepo = new Mock<IFinancialMonthRepository>();
            var monthlyIncomeAfterTax = new Mock<IMonthlyIncomeAfterTaxRepository>();
            transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new RepartitionService(transactionRepo.Object, financialMonthRepo.Object, monthlyIncomeAfterTax.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(result.TotalCommonExpensesPaidByUser1, testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser1, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, testUserId));
            Assert.Equals(result.TotalCommonExpensesPaidByUser2, 0);
            Assert.Equals(result.ActualShareUser1, 1);
            Assert.Equals(result.ActualShareUser2, 0);
            Assert.Equals(result.TargetShareUser1, 1);
            Assert.Equals(result.TargetShareUser2, 0);
        }


        [TestCase(1, 2, 3, 4, 5, 6)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHas2UsersAndAllTransactionEvenlySplit
            (decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1, decimal actualShareU1, decimal actualShareU2)
        {
            // Arrange
            var testTransactions = GetTransactionsWithOnlyEvenSplits(expense1U1, expense2U1, expense1U2, payback1U1);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(1m, 1m);

            var transactionRepo = new Mock<ITransactionRepository>();
            var financialMonthRepo = new Mock<IFinancialMonthRepository>();
            var monthlyIncomeAfterTax = new Mock<IMonthlyIncomeAfterTaxRepository>();
            transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new RepartitionService(transactionRepo.Object, financialMonthRepo.Object, monthlyIncomeAfterTax.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(result.TotalCommonExpenses, testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser1, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser2, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.ActualShareUser1, 0.5m);
            Assert.Equals(result.ActualShareUser2, 0.5m);
            Assert.Equals(result.TargetShareUser1, actualShareU1);
            Assert.Equals(result.TargetShareUser2, actualShareU2);
        }

        [TestCase(1, 2, 3, 4)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHas2UsersWithEqualIncomeAndAllTransactionSplitIncomeBased
            (decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1)
        {
            // Arrange
            var testTransactions = GetTransactionsWithOnlyIncomeBasedSplits(expense1U1, expense2U1, expense1U2, payback1U1);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(1m, 1m);

            var transactionRepo = new Mock<ITransactionRepository>();
            var financialMonthRepo = new Mock<IFinancialMonthRepository>();
            var monthlyIncomeAfterTax = new Mock<IMonthlyIncomeAfterTaxRepository>();
            transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new RepartitionService(transactionRepo.Object, financialMonthRepo.Object, monthlyIncomeAfterTax.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(result.TotalCommonExpenses, testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser1, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser2, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.ActualShareUser1, 0.5m);
            Assert.Equals(result.ActualShareUser2, 0.5m);
            Assert.Equals(result.TargetShareUser1, 0.5m);
            Assert.Equals(result.TargetShareUser2, 0.5m);
        }


        //Transaction by user 1 is an individual expense
        [TestCase(1, 1, 0, 0, 1, 0)]
        [TestCase(1, 1, 1, 0, 1, 0)]
        [TestCase(1, 1, 0, 1, 1, 0)]
        [TestCase(1, 1, 5, 6, 1, 0)]
        //User 1 buys for user 2
        [TestCase(1, 0, 0, 0, 0, 1)]
        [TestCase(1, 0, 1, 0, 0, 1)]
        [TestCase(1, 0, 0, 1, 0, 1)]
        [TestCase(1, 0, 5, 6, 0, 1)]
        // evenly split transaction
        [TestCase(1, 0.5, 0, 0, 0.5, 0.5)]
        [TestCase(1, 0.5, 1, 0, 0.5, 0.5)]
        [TestCase(1, 0.5, 0, 1, 0.5, 0.5)]
        [TestCase(1, 0.5, 5, 6, 0.5, 0.5)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForOneTransaction_IfHouseholdHas2UsersWithInequalIncomeAnd1TransactionSplitCustomBased
            (decimal transactionAmount, decimal user1ShareOfTransaction, decimal income1, decimal income2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetSingleTransactionByUser1WithCustomSplit(transactionAmount, user1ShareOfTransaction);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2); // income doesn't matter in custom split

            var transactionRepo = new Mock<ITransactionRepository>();
            var financialMonthRepo = new Mock<IFinancialMonthRepository>();
            var monthlyIncomeAfterTax = new Mock<IMonthlyIncomeAfterTaxRepository>();
            transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new RepartitionService(transactionRepo.Object, financialMonthRepo.Object, monthlyIncomeAfterTax.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(result.TotalCommonExpenses, testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser1, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.TotalCommonExpensesPaidByUser2, testTransactions.SumUserTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id));
            Assert.Equals(result.User1ShouldPay, user1ShouldPay);
            Assert.Equals(result.User2ShouldPay, user2ShouldPay);
        }
    }
}
