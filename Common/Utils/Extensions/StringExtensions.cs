using System.Globalization;

namespace Common.Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsFinancialMonthOfCorrectFormat(this string financialMonth)
        {
            return DateTime.TryParseExact(
            financialMonth,
            "yyyyMM",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
        }

        public static bool IsYearOfCorrectFormat(this int year)
        {
            return year >= 1 && year <= 9999;
        }
    }
}
