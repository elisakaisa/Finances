using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class UpdateTransactionTests : GeneralTestData
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
        public async Task UpdateTransaction_IsSuccessful_IfContainsAllMandatoryFieldsAndDefaultValuesForNonMandatory()
        {
            // Arrange
            var oldTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual,
                                                   subcategoryId: IncomeMisc.Id,
                                                   subcategory: IncomeMisc);
            var updatedTransaction = CreateTransaction(1000m, TransactionType.Income, SplitType.Individual,
                                                   subcategoryId: IncomeMisc.Id, subcategory: IncomeMisc);

            _subcategoryRepository.Setup(r => r.GetSubcategoryByCategoryIdAsync(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.UpdateAsync(updatedTransaction.ConvertToDto(), User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Amount, Is.Not.EqualTo(oldTransaction.Amount));
        }

        [TestCase("")]
        [TestCase("202400")]
        [TestCase("202413")]
        [TestCase("2024-11")]
        public void UpdateTransaction_ThrowsError_IfFinancialMonthHasWrongFormat(string financialMonth)
        {
            // Arrange
            var updatedTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual, financialMonth: financialMonth,
                                                   subcategoryId: IncomeMisc.Id,
                                                   subcategory: IncomeMisc);

            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.UpdateAsync(updatedTransaction.ConvertToDto(), User1Hh1Id));
        }
    }
}
