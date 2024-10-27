using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Utils.Exceptions;
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
        }

        [Test]
        public async Task UpdateTransaction_IsSuccessful_IfContainsAllMandatoryFieldsAndDefaultValuesForNonMandatory()
        {
            // Arrange
            var oldTransaction = CreateTransaction(123m, TransactionType.Income, SplitType.Individual,
                                                   categoryId: Income.Id, subcategoryId: IncomeMisc.Id,
                                                   category: Income, subcategory: IncomeMisc);
            var updatedTransaction = CreateTransaction(1000m, TransactionType.Income, SplitType.Individual,
                                                   categoryId: Income.Id, subcategoryId: IncomeMisc.Id,
                                                   category: Income, subcategory: IncomeMisc);

            _categoryRepository.Setup(r => r.GetCategorysSubcategories(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.UpdateAsync(updatedTransaction, GeneralTestData.User11);

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
                                                   categoryId: Income.Id, subcategoryId: IncomeMisc.Id,
                                                   category: Income, subcategory: IncomeMisc);

            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.UpdateAsync(updatedTransaction, GeneralTestData.User11));
        }
    }
}
