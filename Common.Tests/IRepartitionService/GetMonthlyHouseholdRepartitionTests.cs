using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Moq;

namespace Common.Tests.IRepartitionService
{
    public class GetMonthlyHouseholdRepartitionTests
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
            var testTransactions = new List<Transaction>
            {
                new()
                {
                    Amount = 123.45,
                    TransactionType = TransactionType.Expenses,
                },
                new()
                {
                    Amount = 13.12,
                    TransactionType = TransactionType.Expenses,
                },
                new()
                {
                    Amount = 102,
                    TransactionType = TransactionType.Savings,
                },
                new()
                {
                    Amount = 50.2,
                    TransactionType = TransactionType.Payback,
                },
                new()
                {
                    Amount = 10000,
                    TransactionType = TransactionType.Income,
                }

            };
            var testHouseholdId = Guid.NewGuid();
            var expensesTotal = testTransactions.Where(t => t.TransactionType == TransactionType.Expenses).Sum(t => t.Amount);
            var paybackTotal = testTransactions.Where(t => t.TransactionType == TransactionType.Payback).Sum(t => t.Amount);

            var transactionRepo = new Mock<ITransactionRepository>();
            var financialMonthRepo = new Mock<IFinancialMonthRepository>();
            var monthlyIncomeAfterTax = new Mock<IMonthlyIncomeAfterTaxRepository>();
            transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new RepartitionService(transactionRepo.Object, financialMonthRepo.Object, monthlyIncomeAfterTax.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(testHouseholdId, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(result.TotalCommonPaidByUser1, expensesTotal - paybackTotal);
            Assert.Equals(result.TotalCommonPaidByUser2, 0);
            Assert.Equals(result.ShareUser1, 1);
            Assert.Equals(result.ShareUser2, 0);
        }
    }
}
