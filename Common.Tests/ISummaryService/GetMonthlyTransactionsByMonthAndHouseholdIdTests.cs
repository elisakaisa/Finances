﻿using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Moq;

namespace Common.Tests.ISummaryService
{
    public class GetMonthlyTransactionsByMonthAndHouseholdIdTests : TestDataBuilder
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
        public void GetMonthlyTransactionsByMonthAndHouseholdId_ThrowsError_IfFinacialMonthOfWrongFormat()
        {
            // arrange
            var transactions = new List<Transaction>();
            var household = GetHouseholdWith2Users();
            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(transactions);
            _householdRepo.Setup(r => r.GetHouseholdByUserId(It.IsAny<Guid>())).ReturnsAsync(household);

            // act
            var sut = new SummaryService(_transactionRepo.Object, _subcetgoryRepo.Object, _householdRepo.Object);

            // assert
            Assert.ThrowsAsync<FinancialMonthOfWrongFormatException>(() => sut.GetMonthlyTransactionsByMonthAndHouseholdId("2024-12", User2Hh1Id));
        }

        [Test]
        public async Task GetMonthlyTransactionsByMonthAndHouseholdId_ReturnsListOfSummaries_IfNotAllCategoriesHaveData()
        {
            // arrange
            var transactions = GetFourTransactionsByTwoUsersWithThreeSubcategories();
            var subcategories = GetListOfFourSubCategories();
            var household = GetHouseholdWith2Users();
            var financialMonth = "202410";

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(transactions);
            _subcetgoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(subcategories);
            _householdRepo.Setup(r => r.GetHouseholdByUserId(It.IsAny<Guid>())).ReturnsAsync(household);

            // act
            var sut = new SummaryService(_transactionRepo.Object, _subcetgoryRepo.Object, _householdRepo.Object);
            var result = await sut.GetMonthlyTransactionsByMonthAndHouseholdId(financialMonth, User1Hh1Id);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(subcategories.Count));

            // by subcategory, household level
            var resultElectricity = result.Where(r => r.SubcategoryName == Electricity.Name).ToList();
            var resultSalary = result.Where(r => r.SubcategoryName == Salary.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity, Is.Not.Null);
                Assert.That(resultSalary, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity, Has.Count.EqualTo(1));
                Assert.That(resultSalary, Has.Count.EqualTo(1));
            });

            var resultElectricity2 = resultElectricity.First();
            var resultSalary2 = resultSalary.First();
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity2.FinancialMonth, Is.EqualTo(financialMonth));
                Assert.That(resultElectricity2.Year, Is.EqualTo(2024));
                Assert.That(resultSalary2.FinancialMonth, Is.EqualTo(financialMonth));
                Assert.That(resultSalary2.Year, Is.EqualTo(2024));
                Assert.That(resultElectricity2.Total, Is.EqualTo(20 + 35));
                Assert.That(resultElectricity2.CommonTotal, Is.EqualTo(20));
                Assert.That(resultSalary2.Total, Is.EqualTo(40));
                Assert.That(resultSalary2.CommonTotal, Is.EqualTo(0));
            });

            // by subcategory, user level
            var userSummaries = resultElectricity2.UserLevelMonthlySummary;
            Assert.That(userSummaries.Count, Is.EqualTo(household.Users.Count));
            var user1Summary = userSummaries.Where(s => s.UserId == User1Hh1Id).FirstOrDefault();
            Assert.That(user1Summary, Is.Not.Null);
            var user2Summary = userSummaries.Where(s => s.UserId == User2Hh1Id).FirstOrDefault();
            Assert.That(user2Summary, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(user1Summary.IndividualTotal, Is.EqualTo(0));
                Assert.That(user1Summary.CommonTotal, Is.EqualTo(20));
                Assert.That(user1Summary.Total, Is.EqualTo(20));
                Assert.That(user2Summary.IndividualTotal, Is.EqualTo(35));
                Assert.That(user2Summary.CommonTotal, Is.EqualTo(0));
                Assert.That(user2Summary.Total, Is.EqualTo(35));
            });
        }

        [TestCase(20, 40, 35, 25)]
        [TestCase(0, 0, 0, 0)]
        [TestCase(0, -20, 0, 0)]
        public async Task GetMonthlyTransactionsByMonthAndHouseholdId_SumsCorrectly_IfFourTransactionOfSameCategory(decimal amountEven1, decimal amountIncome, decimal amountEven2, decimal amountIndividual)
        {
            // arrange
            var transactions = GetFourTransactionsByTwoUsersWithOneSubcategories(amountEven1, amountIncome, amountEven2, amountIndividual);
            var subcategories = GetListOfFourSubCategories();
            var household = GetHouseholdWith2Users();
            var financialMonth = "202412";

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(transactions);
            _subcetgoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(subcategories);
            _householdRepo.Setup(r => r.GetHouseholdByUserId(It.IsAny<Guid>())).ReturnsAsync(household);

            // act
            var sut = new SummaryService(_transactionRepo.Object, _subcetgoryRepo.Object, _householdRepo.Object);
            var result = await sut.GetMonthlyTransactionsByMonthAndHouseholdId(financialMonth, User1Hh1Id);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(subcategories.Count));

            // by subcategory, household level
            var resultElectricity = result.Where(r => r.SubcategoryName == Electricity.Name).ToList();
            var resultSalary = result.Where(r => r.SubcategoryName == Salary.Name).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity, Is.Not.Null);
                Assert.That(resultSalary, Is.Not.Null);
            });
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity, Has.Count.EqualTo(1));
                Assert.That(resultSalary, Has.Count.EqualTo(1));
            });

            var resultElectricity2 = resultElectricity.First();
            var resultSalary2 = resultSalary.First();
            Assert.Multiple(() =>
            {
                Assert.That(resultElectricity2.FinancialMonth, Is.EqualTo(financialMonth));
                Assert.That(resultElectricity2.Year, Is.EqualTo(2024));
                Assert.That(resultSalary2.FinancialMonth, Is.EqualTo(financialMonth));
                Assert.That(resultSalary2.Year, Is.EqualTo(2024));
                Assert.That(resultElectricity2.Total, Is.EqualTo(amountEven1 + amountIncome + amountEven2 + amountIndividual));
                Assert.That(resultElectricity2.CommonTotal, Is.EqualTo(amountEven1 + amountIncome + amountEven2));
                Assert.That(resultSalary2.Total, Is.EqualTo(0));
                Assert.That(resultSalary2.CommonTotal, Is.EqualTo(0));
            });

            // by subcategory, user level
            var userSummaries = resultElectricity2.UserLevelMonthlySummary;
            Assert.That(userSummaries.Count, Is.EqualTo(household.Users.Count));
            var user1Summary = userSummaries.Where(s => s.UserId == User1Hh1Id).FirstOrDefault();
            Assert.That(user1Summary, Is.Not.Null);
            var user2Summary = userSummaries.Where(s => s.UserId == User2Hh1Id).FirstOrDefault();
            Assert.That(user2Summary, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(user1Summary.IndividualTotal, Is.EqualTo(0));
                Assert.That(user1Summary.CommonTotal, Is.EqualTo(amountEven1 + amountIncome));
                Assert.That(user1Summary.Total, Is.EqualTo(amountEven1 + amountIncome));
                Assert.That(user2Summary.IndividualTotal, Is.EqualTo(amountIndividual));
                Assert.That(user2Summary.CommonTotal, Is.EqualTo(amountEven2));
                Assert.That(user2Summary.Total, Is.EqualTo(amountIndividual + amountEven2));
            });
        }
    }
}
