using Common.Repositories.Interfaces;
using Common.Services;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class GetMonthlyTransactionByUserIdTest
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<ISubcategoryRepository> _subcategoryRepository;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _subcategoryRepository = new Mock<ISubcategoryRepository>();
        }

        [Test]
        public async Task GetMonthlyTransactionByUserId_ReturnsMonthlyTransactions()
        {
            // Arrange
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(GeneralTestData.TestTransactions);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id);


            //Assert
            Assert.Equals(GeneralTestData.TestTransactions.Count, result.Count);
        }
    }
}