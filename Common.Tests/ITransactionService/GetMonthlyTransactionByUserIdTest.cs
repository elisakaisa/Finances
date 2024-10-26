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
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _subcategoryRepository = new Mock<ISubcategoryRepository>();
            _userRepository = new Mock<IUserRepository>();
        }

        [Test]
        public async Task GetMonthlyTransactionByUserId_ReturnsMonthlyTransactions()
        {
            // Arrange
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(GeneralTestData.TestTransactions);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, GeneralTestData.User11);


            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.Equals(GeneralTestData.TestTransactions.Count, result.Count);
        }
    }
}