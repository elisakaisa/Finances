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
            var repo = new Mock<ITransactionRepository>();
            repo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(TestData.TestTransactions);

            // Act
            var sut = new TransactionService(repo.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(TestData.User1Hh1);


            //Assert
            Assert.Equals(TestData.TestTransactions.Count, result.Count);
        }
    }
}