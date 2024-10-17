using Common.Model;
using Common.Repositories.Interfaces;
using Common.Services;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class GetMonthlyTransactionByUserIdTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetMonthlyTransactionByUserId_ReturnsMonthlyTransactions()
        {
            // Arrange
            var testTransactions = new List<Transaction>() 
            { 
                TestData.transaction1, TestData.transaction2, TestData.transaction3, TestData.transaction4 
            };
            var repo = new Mock<ITransactionRepository>();
            repo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);

            // Act
            var sut = new TransactionService(repo.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(TestData.User1Hh1);


            //Assert
            Assert.Equals(testTransactions.Count, result.Count);
        }
    }
}