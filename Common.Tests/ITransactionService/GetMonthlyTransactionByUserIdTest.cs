using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
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
        public async Task GetMonthlyTransactionByUserId_ReturnsMonthlyTransactions_IfUserIsPartOfHousehold()
        {
            // Arrange
            var financialMonth = "202412";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(GeneralTestData.TestTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GeneralTestData.User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, financialMonth, GeneralTestData.User11);


            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(GeneralTestData.TestTransactions, Has.Count.EqualTo(result.Count));
        }

        [Test]
        public void GetMonthlyTransactionByUserId_ThrowsError_IfUserNotPartOfHousehold()
        {
            // Arrange
            var financialMonth = "2024-12";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(GeneralTestData.TestTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GeneralTestData.User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            //Assert
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, financialMonth, GeneralTestData.User21));
        }

        [Test]
        public void GetMonthlyTransactionByUserId_ThrowsError_IfFinancialMonthOfWrongFormat()
        {
            // Arrange
            var financialMonth = "2024012";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(GeneralTestData.TestTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GeneralTestData.User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            //Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, financialMonth, GeneralTestData.User11));
        }
    }
}