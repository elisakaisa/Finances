using AutoFixture;
using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using System.Net;
using System.Text.Json;

namespace ApiTests.TransactionScenarios
{
    public class TransactionsScenarios : ApiTestBase
    {
        private readonly string _transactionBaseUrl = "api/transaction";

        [Test]
        public async Task GetMonthlyTransactions_NoHousehold_Returns404()
        {
            // Arrange
            var financialMonth = "202507";
            var invalidUserId = Guid.NewGuid();

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", invalidUserId.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetMonthlyTransactions_UserNotInHousehold_Returns404() // TODO: this should probably not return a 404?
        {
            // Arrange
            var financialMonth = "202507";
            var invalidUserId = Guid.NewGuid();
            var household = new Household { Id = Guid.NewGuid(), Name = "household" };

            await ExecuteScopedContextAction(context =>
            {
                context.Households.Add(household);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", invalidUserId.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetMonthlyTransactions_UserInHouseholdNoTransactions_ReturnsEmptyList()
        {
            // Arrange
            var financialMonth = "202507";
            var household = new Household { Id = Guid.NewGuid(), Name = "household" };
            var user = new User { HouseholdId = household.Id, Id = Guid.NewGuid(), Name = "name" };

            await ExecuteScopedContextAction(context =>
            {
                context.Users.Add(user);
                context.Households.Add(household);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", user.Id.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.EqualTo("[]"));
        }

        [Test]
        public async Task GetMonthlyTransactions_UserInHouseholdWithTwoTransactionsInMonth_ReturnsTransactions()
        {
            // Arrange
            var financialMonth = "202507";
            var household = Fixture.Build<Household>().Create();
            var user = Fixture.Build<User>().With(x => x.HouseholdId, household.Id).With(x => x.Household, household).Create();
            var category = Fixture.Build<Category>().Create();
            var subcategory = Fixture.Build<Subcategory>().With(x => x.CategoryId, category.Id).Create();
            var transaction1 = Fixture.Build<Transaction>()
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.SubcategoryId, subcategory.Id)
                .With(x => x.FinancialMonth, financialMonth)
                .Create();
            var transaction2 = Fixture.Build<Transaction>()
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.SubcategoryId, subcategory.Id)
                .With(x => x.FinancialMonth, financialMonth)
                .Create();

            await ExecuteScopedContextAction(context =>
            {
                context.Categories.Add(category);
                context.Subcategories.Add(subcategory);
                context.Users.Add(user);
                context.Households.Add(household);
                context.Transactions.Add(transaction1);
                context.Transactions.Add(transaction2);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", user.Id.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var output = JsonSerializer.Deserialize<List<TransactionDto>>(content, JsonSerilaizerOptions);
            Assert.That(output, Is.Not.Null);
            Assert.That(output.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetMonthlyTransactions_UserInHouseholdWithTwoTransactionsInOtherMonth_ReturnsEmptyList()
        {
            // Arrange
            var financialMonth = "202507";
            var household = Fixture.Build<Household>().Create();
            var user = Fixture.Build<User>().With(x => x.HouseholdId, household.Id).With(x => x.Household, household).Create();
            var category = Fixture.Build<Category>().Create();
            var subcategory = Fixture.Build<Subcategory>().With(x => x.CategoryId, category.Id).Create();
            var transaction1 = Fixture.Build<Transaction>()
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.SubcategoryId, subcategory.Id)
                .Create();
            var transaction2 = Fixture.Build<Transaction>()
                .With(x => x.UserId, user.Id)
                .With(x => x.User, user)
                .With(x => x.SubcategoryId, subcategory.Id)
                .Create();

            await ExecuteScopedContextAction(context =>
            {
                context.Categories.Add(category);
                context.Subcategories.Add(subcategory);
                context.Users.Add(user);
                context.Households.Add(household);
                context.Transactions.Add(transaction1);
                context.Transactions.Add(transaction2);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", user.Id.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.EqualTo("[]"));
        }

        [Test]
        public async Task GetMonthlyTransactions_UserInHouseholdWithTransactionsInOtherHousehold_ReturnsEmptyList()
        {
            // Arrange
            var financialMonth = "202507";
            var household = Fixture.Build<Household>().Create();
            var user = Fixture.Build<User>().With(x => x.HouseholdId, household.Id).With(x => x.Household, household).Create();
            var category = Fixture.Build<Category>().Create();
            var subcategory = Fixture.Build<Subcategory>().With(x => x.CategoryId, category.Id).Create();
            var transaction1 = Fixture.Build<Transaction>()
                .With(x => x.SubcategoryId, subcategory.Id)
                .With(x => x.FinancialMonth, financialMonth)
                .Create();
            var transaction2 = Fixture.Build<Transaction>()
                .With(x => x.SubcategoryId, subcategory.Id)
                .With(x => x.FinancialMonth, financialMonth)
                .Create();

            await ExecuteScopedContextAction(context =>
            {
                context.Categories.Add(category);
                context.Subcategories.Add(subcategory);
                context.Users.Add(user);
                context.Households.Add(household);
                context.Transactions.Add(transaction1);
                context.Transactions.Add(transaction2);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", user.Id.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.EqualTo("[]"));
        }
    }
}