using AutoFixture;
using Common.Model.DatabaseObjects;
using System.Net;

namespace ApiTests.TransactionScenarios
{
    public class TransactionsScenarios : ApiTestBase
    {
        private readonly string _transactionBaseUrl = "api/transaction";

        [Test]
        public async Task CreateMonthlyTransactions_NoHousehold_Returns404()
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
        public async Task CreateMonthlyTransactions_UserNotInHousehold_Returns404() // TODO: this should probably not return a 404?
        {
            // Arrange
            var financialMonth = "202507";
            var invalidUserId = Guid.NewGuid();
            var household = new Household { Id = Guid.NewGuid(), Name = "household"};

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
        public async Task CreateMonthlyTransactions_UserInHouseholdNoTransactions_ReturnsEmptyList()
        {
            // Arrange
            var financialMonth = "202507";
            var household = new Household { Id = Guid.NewGuid(), Name = "household"};
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
        public async Task CreateMonthlyTransactions_UserInHouseholdWithTransactions_ReturnsTransactions()
        {
            // Arrange
            var financialMonth = "202507";
            var household = new Household { Id = Guid.NewGuid(), Name = "household" };
            var user = new User { HouseholdId = household.Id, Id = Guid.NewGuid(), Name = "name" };
            var category = Fixture.Build<Category>().Create();
            var subcategory = Fixture.Build<Subcategory>().With(x => x.CategoryId, category.Id).Create();
            var transaction = Fixture.Build<Transaction>()
                .With(x => x.UserId, user.Id)
                .With(x => x.SubcategoryId, subcategory.Id)
                .Create();

            await ExecuteScopedContextAction(context =>
            {
                context.Categories.Add(category);
                context.Subcategories.Add(subcategory);
                context.Users.Add(user);
                context.Households.Add(household);
                context.Transactions.Add(transaction);
            });

            var requestUri = $"{_transactionBaseUrl}/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", user.Id.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            //Assert.That(content, Is.EqualTo("[]"));
        }
    }
}