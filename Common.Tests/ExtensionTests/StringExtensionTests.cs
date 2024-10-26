using Common.Utils.Extensions;

namespace Common.Tests.ExtensionTests
{
    public class StringExtensionTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("")]
        [TestCase("2024012")]
        [TestCase("202413")]
        [TestCase("2024-03")]
        public void IsFinancialMonthOfCorrectFormat_ReturnsFalse_WhenStringIsOfWrongFormat(string financialMonth)
        {
            // Arrange

            // Act
            var isCorrectFormat = financialMonth.IsFinancialMonthOfCorrectFormat();

            //Assert
            Assert.That(isCorrectFormat, Is.False);
        }

        [TestCase("202412")]
        [TestCase("199902")]
        [TestCase("202403")]
        public void IsFinancialMonthOfCorrectFormat_ReturnsTrue_WhenStringIsOfCorrectFormat(string financialMonth)
        {
            // Arrange

            // Act
            var isCorrectFormat = financialMonth.IsFinancialMonthOfCorrectFormat();

            //Assert
            Assert.That(isCorrectFormat, Is.True);
        }
    }
}
