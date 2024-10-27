using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Utils.Exceptions;
using Moq;

namespace Common.Tests.ITransactionService
{
    public class UpdateTransactionTests
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
            var oldTransaction = new Transaction
            {
                Id = GeneralTestData.User1Hh1Id,
                Description = "uukhash",
                Date = new DateOnly(2024, 01, 01),
                TransactionType = TransactionType.Income,
                SplitType = SplitType.Even,
                Amount = 123m,
                FinancialMonth = "202412",
                CategoryId = GeneralTestData.Income.Id,
                SubcategoryId = GeneralTestData.IncomeMisc.Id,
                UserId = GeneralTestData.User1Hh1Id,
                User = GeneralTestData.User11
            };
            var updatedTransaction = new Transaction
            {
                Id = GeneralTestData.User1Hh1Id,
                Description = "uukhash",
                Date = new DateOnly(2024, 01, 01),
                TransactionType = TransactionType.Income,
                SplitType = SplitType.Individual,
                Amount = 123m,
                FinancialMonth = "202412",
                CategoryId = GeneralTestData.Income.Id,
                SubcategoryId = GeneralTestData.IncomeMisc.Id,
                UserId = GeneralTestData.User1Hh1Id,
                User = GeneralTestData.User11
            };
            _categoryRepository.Setup(r => r.GetCategorysSubcategories(It.IsAny<int>()))
                .ReturnsAsync(GeneralTestData.Income.Subcategories);
            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);
            var result = await sut.UpdateAsync(oldTransaction, GeneralTestData.User11);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.SplitType, Is.Not.EqualTo(oldTransaction.SplitType));
        }

        [TestCase("")]
        [TestCase("202400")]
        [TestCase("202413")]
        [TestCase("2024-11")]
        public void UpdateTransaction_ThrowsError_IfFinancialMonthHasWrongFormat(string financialMonth)
        {
            // Arrange
            var oldTransaction = new Transaction
            {
                Id = GeneralTestData.User1Hh1Id,
                Description = "uukhash",
                Date = new DateOnly(2024, 01, 01),
                TransactionType = TransactionType.Income,
                SplitType = SplitType.Individual,
                Amount = 123m,
                FinancialMonth = financialMonth,
                CategoryId = GeneralTestData.Income.Id,
                SubcategoryId = GeneralTestData.IncomeMisc.Id,
                UserId = GeneralTestData.User1Hh1Id,
                User = GeneralTestData.User11
            };
            var updatedTransaction = new Transaction
            {
                Id = GeneralTestData.User1Hh1Id,
                Description = "uukhash",
                Date = new DateOnly(2024, 01, 01),
                TransactionType = TransactionType.Income,
                SplitType = SplitType.Individual,
                Amount = 123m,
                FinancialMonth = financialMonth,
                CategoryId = GeneralTestData.Income.Id,
                SubcategoryId = GeneralTestData.IncomeMisc.Id,
                UserId = GeneralTestData.User1Hh1Id,
                User = GeneralTestData.User11
            };

            _transactionRepo.Setup(r => r.UpdateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(updatedTransaction);

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object, _userRepository.Object);

            // Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.UpdateAsync(oldTransaction, GeneralTestData.User11));
        }
    }
}
