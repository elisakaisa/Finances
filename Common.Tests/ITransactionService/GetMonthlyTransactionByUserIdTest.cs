using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class GetMonthlyTransactionByUserIdTest : GeneralTestData
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<ISubcategoryRepository> _subcategoryRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IHouseholdRepository> _householdRepository;

        private List<Transaction> _testTransactions = new List<Transaction>();

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _subcategoryRepository = new Mock<ISubcategoryRepository>();
            _userRepository = new Mock<IUserRepository>();
            _householdRepository = new Mock<IHouseholdRepository>();

            InitializeTestTransactions();
        }


        [Test]
        public async Task GetMonthlyTransactionByUserId_ReturnsMonthlyTransactions_IfUserIsPartOfHousehold()
        {
            // Arrange
            var financialMonth = "202412";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(_testTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GeneralTestData.User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, financialMonth, User1Hh1Id);


            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(_testTransactions.Count));
        }

        [Test]
        public void GetMonthlyTransactionByUserId_ThrowsError_IfUserNotPartOfHousehold()
        {
            // Arrange
            var financialMonth = "202412";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(_testTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(User22.Id)).ReturnsAsync(User22);
            _userRepository.Setup(r => r.GetByIdAsync(User1Hh1Id)).ReturnsAsync(User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object, _householdRepository.Object);

            //Assert
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.GetMonthlyTransactionsByUserId(User1Hh1Id, financialMonth, User22.Id));
        }

        [Test]
        public void GetMonthlyTransactionByUserId_ThrowsError_IfFinancialMonthOfWrongFormat()
        {
            // Arrange
            var financialMonth = "2024012";
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(_testTransactions);
            _userRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GeneralTestData.User11);

            //Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object, _householdRepository.Object);

            //Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.GetMonthlyTransactionsByUserId(GeneralTestData.User1Hh1Id, financialMonth, User1Hh1Id));
        }
        private void InitializeTestTransactions()
        {
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc));
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc));
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc));
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Expenses, SplitType.Even, subcategoryId: Electricity.Id, subcategory: Electricity));
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc));
            _testTransactions.Add(CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc));
        }
    }
}