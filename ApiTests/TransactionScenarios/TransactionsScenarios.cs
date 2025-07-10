using System.Net;

namespace ApiTests.TransactionScenarios
{
    public class TransactionsScenarios : ApiTests
    {
        [Test]
        public async Task CreateMonthlyTransactions_NoHousehold_Returns404()
        {
            // Arrange
            var financialMonth = "202507";
            var invalidUserId = Guid.NewGuid();

            var requestUri = $"api/transaction/household/monthly-transactions?financialMonth={financialMonth}";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Add("requestingUserId", invalidUserId.ToString());

            // Act
            var response = await HttpClient.SendAsync(requestMessage);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}