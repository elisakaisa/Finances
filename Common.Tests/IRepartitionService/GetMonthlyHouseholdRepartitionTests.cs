using Common.Model.Enums;
using Common.Repositories.Interfaces;
using Common.Services;
using Common.Tests.TestData;
using Common.Utils.Exceptions;
using Common.Utils.Extensions;
using Moq;

namespace Common.Tests.IRepartitionService
{
    public class GetMonthlyHouseholdRepartitionTests : TestDataBuilder
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
        public Task GetMonthlyHouseholdRepartition_ThrowsError_IfUserIsNotInHousehold()
        {
            // Arrange
            var testTransactions = GetTransactionsForSingleHousehold();

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith3Users());

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);

            // Assert
            Assert.ThrowsAsync<UserNotInHouseholdException>(() => sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User2Hh1Id));
            return Task.CompletedTask;
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
            Assert.ThrowsAsync<HouseholdWithMoreThanTwoUsersNotSupportedException>(() => sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id));
            return Task.CompletedTask;
        }

        [Test]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHasOneUser()
        {
            // Arrange
            var testTransactions = GetTransactionsForSingleHousehold();
            var testUserId = Guid.NewGuid();
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTaxForOneUserHousehold(1m);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith1User());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumHouseholdTransactionsByType(TransactionType.Expenses, Household1Id)));
                Assert.That(result.ActualUserShare[User1Hh1Id], Is.EqualTo(1));
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(1));
            });
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
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(0.5m)); //TODO: to be implemented
                Assert.That(result.TargetUserShare[User2Hh1Id], Is.EqualTo(0.5m));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
            });
        }

        [TestCase(1, 2, 3, 4, 0.5, 0.5)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        public async Task GetMonthlyHouseholdRepartition_ReturnsCorrectRepartition_IfHouseholdHas2UsersWithEqualIncomeAndAllTransactionSplitIncomeBased
            (decimal expense1U1, decimal expense2U1, decimal expense1U2, decimal payback1U1, decimal actualShareUser1, decimal actualShareUser2)
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
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(actualShareUser1));
                Assert.That(result.TargetUserShare[User2Hh1Id], Is.EqualTo(actualShareUser2));
            });
            //Assert.That(result.ActualUserShare[GeneralTestData.User1Hh1Id], Is.EqualTo(0.5m));
            //Assert.That(result.AtualUserShare[GeneralTestData.User2Hh1Id], Is.EqualTo(0.5m));
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
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.UserShouldPay[User1Hh1Id], Is.EqualTo(user1ShouldPay));
                Assert.That(result.UserShouldPay[User2Hh1Id], Is.EqualTo(user2ShouldPay));
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(user1ShareOfTransaction));
                Assert.That(result.TargetUserShare[User2Hh1Id], Is.EqualTo(1 - user1ShareOfTransaction));
                Assert.That(result.ActualUserShare[User1Hh1Id], Is.EqualTo(1));
                Assert.That(result.ActualUserShare[User2Hh1Id], Is.EqualTo(0));
            });
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

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "202412"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "202412"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "202412", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.UserShouldPay[User1Hh1Id], Is.EqualTo(user1ShouldPay));
                Assert.That(result.UserShouldPay[User2Hh1Id], Is.EqualTo(user2ShouldPay));
            });
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

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "202412"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "202412"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "202412", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.UserShouldPay[User1Hh1Id], Is.EqualTo(user1ShouldPay));
                Assert.That(result.UserShouldPay[User2Hh1Id], Is.EqualTo(user2ShouldPay));
                var householdIncome = income1 + income2;
                var incomeUserShare1 = householdIncome == 0 ? 0.5m : income1 / householdIncome;
                var incomeUserShare2 = householdIncome == 0 ? 0.5m : income2 / householdIncome;
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(Math.Round(incomeUserShare1, 3)));
                Assert.That(result.TargetUserShare[User2Hh1Id], Is.EqualTo(Math.Round(incomeUserShare2, 3)));
                Assert.That(result.ActualUserShare[User1Hh1Id], Is.EqualTo(1));
                Assert.That(result.ActualUserShare[User2Hh1Id], Is.EqualTo(0));
                Assert.That(result.IncomeAfterTax[User1Hh1Id], Is.EqualTo(income1));
                Assert.That(result.IncomeAfterTax[User2Hh1Id], Is.EqualTo(income2));
                Assert.That(result.UserSharesOfHouseholdIncome[User1Hh1Id], Is.EqualTo(incomeUserShare1));
                Assert.That(result.UserSharesOfHouseholdIncome[User2Hh1Id], Is.EqualTo(incomeUserShare2));
            });
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
            var testTransactions = GetTwoTransactionByUser1WithSplitByIncome(transactionAmount1, transactionAmount2);
            var montlyIncomeAfterTaxUser = GetMonthlyIncomesAfterTax(income1, income2);

            _transactionRepo.Setup(r => r.GetMonthlyTransactionsByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(testTransactions);
            _householdRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetHouseholdWith2Users());
            _monthlyIncomeAfterTaxRepo.Setup(r => r.GetMonthlyIncomeAfterTaxByHouseholdIdAsync(It.IsAny<Guid>(), "2024-12"))
                .ReturnsAsync(montlyIncomeAfterTaxUser);

            // Act
            var sut = new RepartitionService(_transactionRepo.Object, _monthlyIncomeAfterTaxRepo.Object, _householdRepository.Object);
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.UserShouldPay[User1Hh1Id], Is.EqualTo(user1ShouldPay));
                Assert.That(result.UserShouldPay[User2Hh1Id], Is.EqualTo(user2ShouldPay));
                var householdIncome = income1 + income2;
                var incomeUserShare1 = householdIncome == 0 ? 0.5m : income1 / householdIncome;
                var incomeUserShare2 = householdIncome == 0 ? 0.5m : income2 / householdIncome;
                Assert.That(result.TargetUserShare[User1Hh1Id], Is.EqualTo(Math.Round(incomeUserShare1, 3)));
                Assert.That(result.TargetUserShare[User2Hh1Id], Is.EqualTo(Math.Round(incomeUserShare2, 3)));
                Assert.That(result.ActualUserShare[User1Hh1Id], Is.EqualTo(1));
                Assert.That(result.ActualUserShare[User2Hh1Id], Is.EqualTo(0));
                Assert.That(result.IncomeAfterTax[User1Hh1Id], Is.EqualTo(income1));
                Assert.That(result.IncomeAfterTax[User2Hh1Id], Is.EqualTo(income2));
                Assert.That(result.UserSharesOfHouseholdIncome[User1Hh1Id], Is.EqualTo(incomeUserShare1));
                Assert.That(result.UserSharesOfHouseholdIncome[User2Hh1Id], Is.EqualTo(incomeUserShare2));
            });
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
            var result = await sut.GetMonthlyHouseholdRepartition(Household1Id, "2024-12", User1Hh1Id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(result.TotalCommonExpenses, Is.EqualTo(testTransactions.SumCommonExpensesByHousehold(Household1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User1Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User1Hh1Id)));
                Assert.That(result.TotalCommonExpensesPaidByUser[User2Hh1Id], Is.EqualTo(testTransactions.SumCommonExpensesByUser(User2Hh1Id)));
                Assert.That(result.UserShouldPay[User1Hh1Id], Is.EqualTo(user1ShouldPay));
                Assert.That(result.UserShouldPay[User2Hh1Id], Is.EqualTo(user2ShouldPay));
            });
        }
    }
}
