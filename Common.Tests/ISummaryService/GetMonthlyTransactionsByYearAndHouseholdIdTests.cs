﻿using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Moq;
using System.Text.RegularExpressions;

namespace Common.Tests.ISummaryService
{
    public partial class GetMonthlyTransactionsByYearAndHouseholdIdTests : TestDataBuilder
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<ISubcategoryRepository> _subcetgoryRepo;
        private Mock<IHouseholdRepository> _householdRepo;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _subcetgoryRepo = new Mock<ISubcategoryRepository>();
            _householdRepo = new Mock<IHouseholdRepository>();

            InitializeSubcategories();
        }

        [Test]
        public async Task GetMonthlyTransactionByYearAndHousehold_Returns12MonthsOfData_IfCorrectDataFilled()
        {
            // arrange
            var transactions = GetFourTransactionsByTwoUsersWithThreeSubcategories();
            var subcategories = GetListOfFourSubCategories();
            var household = GetHouseholdWith2Users();

            _transactionRepo.Setup(r => r.GetYearlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(transactions);
            _subcetgoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(subcategories);
            _householdRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(household);

            // act
            var sut = new SummaryService(_transactionRepo.Object, _subcetgoryRepo.Object, _householdRepo.Object);
            var result = await sut.GetMonthlyTransactionsByYearAndHouseholdId(2024, Household1Id, User1Hh1Id);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(12*subcategories.Count));
            foreach(var row in result)
            {
                Assert.That(HasyyyyMMShape().IsMatch(row.FinancialMonth), Is.True);
            }
        }

        [GeneratedRegex(@"^2024\d{2}$")]
        private static partial Regex HasyyyyMMShape();
    }
}
