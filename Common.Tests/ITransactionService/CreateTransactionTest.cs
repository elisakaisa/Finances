using Common.Model.DatabaseObjects;
using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Utils.Exceptions;
using Moq;
using System.Runtime.Intrinsics.Arm;

namespace Common.Tests.ITransactionService
{
    public class CreateTransactionTest
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<IMonthlyIncomeAfterTaxRepository> _monthlyIncomeAfterTaxRepo;
        private Mock<IHouseholdRepository> _householdRepository;
        private Mock<ICategoryRepository> _categoryRepository;
        private Mock<ISubcategoryRepository> _subcategoryRepository;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _monthlyIncomeAfterTaxRepo = new Mock<IMonthlyIncomeAfterTaxRepository>();
            _householdRepository = new Mock<IHouseholdRepository>();
            _categoryRepository = new Mock<ICategoryRepository>();
            _subcategoryRepository = new Mock<ISubcategoryRepository>();
        }

        [Test]
        public async Task CreateTransaction_IsSuccessful_IfContainsAllMandatoryFieldsAndDefaultValuesForNonMandatory()
        {
            // Arrange
            var newTransaction = new Transaction
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
                UserId = GeneralTestData.User1Hh1Id
            };

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object);
            var result = await sut.CreateAsync(newTransaction);

            // Assert
            Assert.That(result, Is.Not.Null);
            // default values
            Assert.That(result.ExcludeFromSummary, Is.False);
            Assert.That(result.UserShare, Is.Null);
            Assert.That(result.ToVerify, Is.False);
            Assert.That(result.ModeOfPayment, Is.EqualTo(ModeOfPayment.NA));
        }

        [TestCase("")]
        [TestCase("202400")]
        [TestCase("202413")]
        [TestCase("2024-11")]
        public async Task CreateTransaction_ThrowsError_IfFinancialMonthHasWrongFormat(string financialMonth)
        {
            // Arrange
            var newTransaction = new Transaction
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
                UserId = GeneralTestData.User1Hh1Id
            };

            // Act
            var sut = new TransactionService(_transactionRepo.Object, _categoryRepository.Object, _subcategoryRepository.Object);

            // Assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.CreateAsync(newTransaction));
        }
    }
}
