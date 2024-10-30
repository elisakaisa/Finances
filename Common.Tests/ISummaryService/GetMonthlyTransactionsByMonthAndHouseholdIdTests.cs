using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Moq;

namespace Common.Tests.ISummaryService
{
    public class GetMonthlyTransactionsByMonthAndHouseholdIdTests : TestDataBuilder
    {
        private Mock<ITransactionRepository> _transactionRepo;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();

            InitializeSubcategories();
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
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.GetMonthlyTransactionsByMonthAndHouseholdId("2024-12", Household1Id, User21));
        }

        [Test]
        public async Task GetMonthlyTransactionsByMonthAnsHouseholdId_ReturnsListOfSummaries_IfNotAllCategoriesHaveData()
        {
            // arrange
            var transactions = GetFourTransactionsByTwoUsersWithTwoSubcategories();
            var subcategories = GetListOfFourSubCategories();

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(transactions);

            // act
            var sut = new SummaryService(_transactionRepo.Object);
            var result = await sut.GetMonthlyTransactionsByMonthAndHouseholdId("2024-12", Household1Id, User11);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(subcategories.Count));
        }
    }
}
