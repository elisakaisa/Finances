using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class CreateTransactionTest : GeneralTestData
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<IMonthlyIncomeAfterTaxRepository> _monthlyIncomeAfterTaxRepo;
        private Mock<IHouseholdRepository> _householdRepository;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<ISubcategoryRepository> _subcategoryRepository;
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _monthlyIncomeAfterTaxRepo = new Mock<IMonthlyIncomeAfterTaxRepository>();
            _householdRepository = new Mock<IHouseholdRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _subcategoryRepository = new Mock<ISubcategoryRepository>();
            _userRepository = new Mock<IUserRepository>();

            InitializeSubcategories();

            _userRepository.Setup(r => r.GetByIdAsync(User22.Id)).ReturnsAsync(User22);
            _userRepository.Setup(r => r.GetByIdAsync(User1Hh1Id)).ReturnsAsync(User11);
            _userRepository.Setup(r => r.GetByIdAsync(User2Hh1Id)).ReturnsAsync(User12);

            _subcategoryRepository.Setup(r => r.GetSubcategoryByName(IncomeMisc.Name)).ReturnsAsync(IncomeMisc);
            _subcategoryRepository.Setup(r => r.GetSubcategoryByName(Salary.Name)).ReturnsAsync(Salary);
            _subcategoryRepository.Setup(r => r.GetSubcategoryByName(Electricity.Name)).ReturnsAsync(Electricity);
        }

        [Test]
        public async Task CreateTransaction_IsSuccessful_IfContainsAllMandatoryFieldsAndDefaultValuesForNonMandatory()
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual,
                                                    subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc);

            _subcategoryRepository.Setup(r => r.GetSubcategoryByCategoryIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(newTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                // default values
                Assert.That(result.ExcludeFromSummary, Is.False);
                Assert.That(result.UserShare, Is.Null);
                Assert.That(result.ToVerify, Is.False);
                Assert.That(result.ModeOfPayment, Is.EqualTo(ModeOfPayment.NA));
            });
        }

        [Test]
        public async Task CreateTransaction_IsSuccessful_IisCreatedByUserInHousehold()
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual,
                                                   subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc);

            _subcategoryRepository.Setup(r => r.GetSubcategoryByCategoryIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(newTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.CreateAsync(newTransaction.ConvertToDto(), User2Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserId, Is.EqualTo(GeneralTestData.User1Hh1Id));
        }

        [Test]
        public void CreateTransaction_ThrowsError_IfIsCreatedByUserNotInHousehold()
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual, subcategory: IncomeMisc, subcategoryId: IncomeMisc.Id);

            _subcategoryRepository.Setup(r => r.GetSubcategoryByCategoryIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.CreateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(newTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User22.Id));
        }


        [TestCase("")]
        [TestCase("202400")]
        [TestCase("202413")]
        [TestCase("2024-11")]
        public void CreateTransaction_ThrowsError_IfFinancialMonthHasWrongFormat(string financialMonth)
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual, financialMonth: financialMonth, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        }

        [Test]
        public void CreateTransaction_ThrowsError_IfDataIsMissing()
        {
            // Arrange
            var newTransaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                UserId = GeneralTestData.User11.Id,
                Description = "test",
                FinancialMonth = "202412",
                SubcategoryName = "test",
                CategoryName = "test"
            };

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<MissingOrWrongDataException>(() => sut.CreateAsync(newTransaction, User1Hh1Id));
        }

        [Test]
        public void CreateTransaction_ThrowsError_IfUserShareIsNullForCustomSplitType()
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Expenses, SplitType.Custom, subcategoryId: Electricity.Id, subcategory: Electricity);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<MissingOrWrongDataException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        }

        [TestCase(-0.2)]
        [TestCase(2)]
        public void CreateTransaction_ThrowsError_IfUserShareHasInvalidValueForCustomSplitType(decimal userShare)
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Expenses, SplitType.Custom, userShare: userShare, subcategoryId: Electricity.Id, subcategory: Electricity);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<MissingOrWrongDataException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        }

        [TestCase(SplitType.IncomeBased)]
        [TestCase(SplitType.Even)]
        [TestCase(SplitType.Custom)]
        public void CreateTransaction_ThrowsError_IfTransactionIsIncomeAndSplitNotIndividual(SplitType splitType)
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, splitType, userShare: 0.5m, subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<MissingOrWrongDataException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        }

        [Test]
        public void CreateTransaction_ThrowsError_IfCategoryAndExpenseTypeDoNotMatch()
        {
            // Arrange
            var newTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual,
                                                   subcategoryId: Electricity.Id, subcategory: Electricity);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<MissingOrWrongDataException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        }

        //TODO: decide later what validation is wanted
        //[Test]
        //// TODO: add test cases?
        //public void CreateTransaction_ThrowsError_IfSubcategoryIsNotContainedInCategory()
        //{
        //    // Arrange
        //    var newTransaction = new Transaction
        //    {
        //        Id = GeneralTestData.User1Hh1Id,
        //        Description = "uukhash",
        //        Date = new DateOnly(2024, 01, 01),
        //        TransactionType = TransactionType.Expenses,
        //        SplitType = SplitType.Individual,
        //        Amount = 123m,
        //        FinancialMonth = "202412",
        //        SubcategoryId = GeneralTestData.Electricity.Id,
        //        UserId = GeneralTestData.User1Hh1Id,
        //        Subcategory = GeneralTestData.Electricity, // irrelevant what is here, bc mock returns income categories
        //        User = GeneralTestData.User11
        //    };

        //    // Act
        //    _subcategoryRepository.Setup(r => r.GetSubcategoryByCategoryIdAsync(It.IsAny<int>())).ReturnsAsync(GeneralTestData.Income.Subcategories);
        //    var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

        //    // Assert
        //    Assert.ThrowsAsync<SubcategoryNotContainedInCategoryException>(() => sut.CreateAsync(newTransaction.ConvertToDto(), User1Hh1Id));
        //}

    }
}
