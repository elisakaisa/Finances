using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Utils.Extensions;

namespace Common.Tests.ExtensionTests
{
    public class IsTransactionCommonTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(TransactionType.Savings)]
        [TestCase(TransactionType.Income)]
        public void IsTransactionCommon_ReturnsFalse_WhenTransactionIsNotExpense(TransactionType type)
        {
            // Arrange
            var testTransaction = new Transaction()
            {
                TransactionType = type,
                Description = "test",
                FinancialMonth = "202412"
            };

            // Act
            var isTransactionCommon = testTransaction.IsTransactionCommon();

            //Assert
            Assert.That(isTransactionCommon, Is.False);
        }

        [Test]
        public void IsTransactionCommon_ReturnsFalse_WhenTransactionIsIndividual()
        {
            // Arrange
            var testTransaction = new Transaction()
            {
                TransactionType = TransactionType.Expenses,
                SplitType = SplitType.Individual,
                Description = "test",
                FinancialMonth = "202412"
            };

            // Act
            var isTransactionCommon = testTransaction.IsTransactionCommon();

            //Assert
            Assert.That(isTransactionCommon, Is.False);
        }

        [TestCase(SplitType.IncomeBased)]
        [TestCase(SplitType.Even)]
        [TestCase(SplitType.Custom)]
        public void IsTransactionCommon_ReturnsTrue_WhenTransactionIsNotIndividual(SplitType splitType)
        {
            // Arrange
            var testTransaction = new Transaction()
            {
                TransactionType = TransactionType.Expenses,
                SplitType = splitType,
                Description = "test",
                FinancialMonth = "202412"
            };

            // Act
            var isTransactionCommon = testTransaction.IsTransactionCommon();

            //Assert
            Assert.That(isTransactionCommon, Is.True);
        }
    }
}
