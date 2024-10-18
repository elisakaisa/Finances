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
                .ReturnsAsync(GeneralTestData.TestTransactions);

            // Act
            var sut = new TransactionService(repo.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id);


            //Assert
            Assert.Equals(GeneralTestData.TestTransactions.Count, result.Count);
        }
    }
}