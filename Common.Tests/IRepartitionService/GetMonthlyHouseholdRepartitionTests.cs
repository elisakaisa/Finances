﻿using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.IRepartitionService.TestData;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Moq;

namespace Common.Tests.IRepartitionService
{
    public class GetMonthlyHouseholdRepartitionTests : HouseholdRepartitionTestData
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
        public Task GetMonthlyHouseholdRepartition_ThrowsError_IfHouseholdHasThreeUser()
        {
            // Arrange
            var testTransactions = GetTransactionsForSingleHousehold();

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith3Users());

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);

            // Assert
            Assert.ThrowsAsync<HouseholdWithMoreThanTwoUsersNotSupportedException>(() => sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12"));
            return Task.CompletedTask;
        }

        [Test]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHasOneUser()
        {
            // Arrange
            var testTransactions = GetTransactionsForSingleHousehold();
            var testUserId = Guid.NewGuid();

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith1User());

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumUserTransactionsByType(TransactionType.Expenses, testUserId)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(0));
            Assert.That(result.ActualUserShare[GeneralTestData.User1Hh1Id], Is.EqualTo(1));
            Assert.That(result.ActualUserShare[GeneralTestData.User1Hh1Id], Is.EqualTo(0));
            Assert.That(result.TargetUserShare[GeneralTestData.User2Hh1Id], Is.EqualTo(1));
            Assert.That(result.TargetUserShare[GeneralTestData.User2Hh1Id], Is.EqualTo(0));
        }


        [TestCase(1, 2, 3, 4, 1)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHas2UsersAndAllTransactionEvenlySplit
            (decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1, decimal userShouldPay)
        {
            // Arrange
            var testTransactions = GetTransactionsWithOnlyEvenSplits(expense1U1, expense2U1, expense1U2, payback1U1);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(1m, 1m);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            //Assert.That(result.TargetShareUser1, Is.EqualTo(0.5m)); //TODO: to be implemented
            //Assert.That(result.TargetShareUser2, Is.EqualTo(0.5m));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
        }

        [TestCase(1, 2, 3, 4)]
        [TestCase(0, 0, 0, 0)] // TODO: fix this case. Is it even needed?
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHas2UsersWithEqualIncomeAndAllTransactionSplitIncomeBased
            (decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1)
        {
            // Arrange
            var testTransactions = GetTransactionsWithOnlyIncomeBasedSplits(expense1U1, expense2U1, expense1U2, payback1U1);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(1m, 1m);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            //Assert.That(result.ActualShareUser1, Is.EqualTo(0.5m));
            //Assert.That(result.ActualShareUser2, Is.EqualTo(0.5m));
            //Assert.That(result.TargetShareUser1, Is.EqualTo(0.5m));
            //Assert.That(result.TargetShareUser2, Is.EqualTo(0.5m));
        }


        //Transaction by user 1 entirely on user 1
        [TestCase(1, 1, 0, 0, 1, 0)]
        [TestCase(1, 1, 1, 0, 1, 0)]
        [TestCase(1, 1, 0, 1, 1, 0)]
        [TestCase(1, 1, 5, 6, 1, 0)]
        //Tramsaction by user 1 entirely on user 2
        [TestCase(1, 0, 0, 0, 0, 1)]
        [TestCase(1, 0, 1, 0, 0, 1)]
        [TestCase(1, 0, 0, 1, 0, 1)]
        [TestCase(1, 0, 5, 6, 0, 1)]
        // evenly split transaction
        [TestCase(1, 0.5, 0, 0, 0.5, 0.5)]
        [TestCase(1, 0.5, 1, 0, 0.5, 0.5)]
        [TestCase(1, 0.5, 0, 1, 0.5, 0.5)]
        [TestCase(1, 0.5, 5, 6, 0.5, 0.5)]
        // unevenly split transaction
        [TestCase(1, 0.7, 0, 0, 0.7, 0.3)]
        [TestCase(1, 0.7, 1, 0, 0.7, 0.3)]
        [TestCase(1, 0.7, 0, 1, 0.7, 0.3)]
        [TestCase(1, 0.7, 5, 6, 0.7, 0.3)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForOneTransaction_IfHouseholdHas2UsersWithInequalIncomeAnd1TransactionSplitCustomBased
            (decimal transactionAmount, decimal user1ShareOfTransaction, decimal income1, decimal income2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetSingleTransactionByUser1WithCustomSplit(transactionAmount, user1ShareOfTransaction);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2); // income doesn't matter in custom split

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            Assert.That(result.UserShouldPay[GeneralTestData.User1Hh1Id], Is.EqualTo(user1ShouldPay));
            Assert.That(result.UserShouldPay[GeneralTestData.User2Hh1Id], Is.EqualTo(user2ShouldPay));
        }




        //Transaction by user 1 is on user1
        [TestCase(1, 1, 0, 0, 1, 0)]
        [TestCase(1, 1, 1, 1, 2, 0)]
        [TestCase(1, 0, 1, 0, 0, 2)]
        // Expense split evenly
        [TestCase(1, 0.5, 1, 0.5, 1, 1)]
        // random shares
        [TestCase(1, 0.2, 1, 0.3, 0.5, 1.5)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForTwoTransaction_IfHouseholdHas2UsersAndTransactionsSplitCustomBased
            (decimal transactionAmount1, decimal user1ShareOfTransaction1, decimal transactionAmount2, decimal user1ShareOfTransaction2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetTwoTransactionByUser1WithCustomSplit(transactionAmount1, user1ShareOfTransaction1, transactionAmount2, user1ShareOfTransaction2);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(1m, 32m); // income doesn't matter in custom split

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            Assert.That(result.UserShouldPay[GeneralTestData.User1Hh1Id], Is.EqualTo(user1ShouldPay));
            Assert.That(result.UserShouldPay[GeneralTestData.User2Hh1Id], Is.EqualTo(user2ShouldPay));
        }


        [TestCase(1, 0, 0, 0.5, 0.5)]
        [TestCase(1, 1, 1, 0.5, 0.5)]
        [TestCase(1, 2, 1, 0.667, 0.333)]
        [TestCase(1, 1, 2, 0.333, 0.667)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForOneTransaction_IfHouseholdHas2UsersWithInequalIncomeAnd1TransactionSplitIncomeBased
            (decimal transactionAmount, decimal income1, decimal income2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetSingleTransactionByUser1WithSplitByIncome(transactionAmount);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            Assert.That(result.UserShouldPay[GeneralTestData.User1Hh1Id], Is.EqualTo(user1ShouldPay));
            Assert.That(result.UserShouldPay[GeneralTestData.User2Hh1Id], Is.EqualTo(user2ShouldPay));
        }


        [TestCase(1, 0, 0, 0, 0.5, 0.5)]
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(2, 1, 1, 1, 1.5, 1.5)]
        [TestCase(2, 1, 2, 1, 2, 1)]
        [TestCase(2, 1, 1, 2, 1, 2)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForTwoTransactions_IfHouseholdHas2UsersWithInequalIncomeAndTransactionsSplitIncomeBased
            (decimal transactionAmount1, decimal transactionAmount2, decimal income1, decimal income2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetTwoTransactionByUsersWithSplitByIncome(transactionAmount1, transactionAmount2);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            Assert.That(result.UserShouldPay[GeneralTestData.User1Hh1Id], Is.EqualTo(user1ShouldPay));
            Assert.That(result.UserShouldPay[GeneralTestData.User2Hh1Id], Is.EqualTo(user2ShouldPay));
        }


        [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0)]
        [TestCase(1, 1, 1, 1, 1, 0, 0, 2, 1)]
        [TestCase(1, 1, 1, 1, 1, 1, 0, 2.5, 0.5)]
        [TestCase(1, 1, 1, 1, 1, 2, 1, 2.167, 0.833)]
        [TestCase(1, 1, 1, 1, 1, 1, 2, 1.833, 1.167)]
        [TestCase(1, 1, 1, 0.4, 1, 2, 1, 1.567, 1.433)]
        [TestCase(1, 1, 1, 0, 1, 2, 1, 1.167, 1.833)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartitionForMultipleTransactions_IfHouseholdHas2UsersWithInequalIncomeAndUser1HasTransactionsSplitInDifferentWays
            (decimal amountIncomeBased, decimal amountEven, decimal amountCustom, decimal shareCustom, decimal amountIndividual, decimal income1, decimal income2, decimal user1ShouldPay, decimal user2ShouldPay)
        {
            // Arrange
            var testTransactions = GetMultipleTransactionsByUser1WithVariousSplits(amountIncomeBased, amountEven, amountCustom, shareCustom, amountIndividual);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(GeneralTestData.Household1Id, "2024-12");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(GeneralTestData.Household1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User1Hh1Id)));
            Assert.That(result.TotalCommonExpensesPaidByUser[GeneralTestData.User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(GeneralTestData.User2Hh1Id)));
            Assert.That(result.UserShouldPay[GeneralTestData.User1Hh1Id], Is.EqualTo(user1ShouldPay));
            Assert.That(result.UserShouldPay[GeneralTestData.User2Hh1Id], Is.EqualTo(user2ShouldPay));
        }
    }
}
