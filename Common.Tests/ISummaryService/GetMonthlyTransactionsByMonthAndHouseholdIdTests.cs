using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Utils.Exceptions;
using Moq;

namespace Common.Tests.ISummaryService
{
    public class GetMonthlyTransactionsByMonthAndHouseholdIdTests
    {
        private Mock<ITransactionRepository> _transactionRepo;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
        }

        [Test]
        public void GetMonthlyTransactionsByMonthAnsHouseholdId_ThrowsError_IfUserNotInHousehold()
        {
            // arrange
            var transactions = new List<Transaction>();
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(transactions);

            // act
            var sut = new SummaryService(_transactionRepo.Object);

            // assert
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.GetMonthlyTransactionsByMonthAndHouseholdId("2024-12", GeneralTestData.Household1Id, GeneralTestData.User21));
        }
    }
}
