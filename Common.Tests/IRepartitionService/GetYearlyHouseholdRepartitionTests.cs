﻿using Common.Model.DatabaseObjects;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Moq;

namespace Common.Tests.IRepartitionService
{
    public class GetYearlyHouseholdRepartitionTests : TestDataBuilder
    {
        private Mock<ITransactionRepository> _transactionRepo;
        private Mock<IMonthlyIncomeAfterTaxRepository> _monthlyIncomeAfterTaxRepo;
        private Mock<IHouseholdRepository> _householdRepository;

        [SetUp]
        public void Setup()
        {
            _transactionRepo = new Mock<ITransactionRepository>();
            _monthlyIncomeAfterTaxRepo = new Mock<IMonthlyIncomeAfterTaxRepository>();
            _householdRepository = new Mock<IHouseholdRepository>();
        }

        [Test]
        public async Task GetYearlyHouseholdRepartition_Returns12Months_IfDataForAYearIsAvailable()
        {
            // Arrange

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);

            // Assert
        }

        [Test]
        public async Task GetYearlyHouseholdRepartition_Returns12Months_IfNoTransactionsHaveBeenAdded()
        {
            // Arrange
            var emptyIncomeList = new List<MonthlyIncomeAfterTax>();
            var emptyTransactionList = new List<Transaction>();

            _transactionRepo.Setup(r => r.GetYearlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), 2024))
                .ReturnsAsync(emptyTransactionList);
            _householdRepository.Setup(r => r.GetHouseholdByUserId(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetYearlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), 2024))
                .ReturnsAsync(emptyIncomeList);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetYearlyHouseholdRepartition(2024, User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(12));
        }
    }
}
