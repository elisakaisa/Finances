using Common.Utils.Exceptions;

namespace Common.Utils.Extensions
{
    public static class ValidationExtensions
    {
        public static void ValidateFinancialMonthFormat(this string financialMonth)
        {
            if (!financialMonth.IsFinancialMonthOfCorrectFormat())
            {
                throw new FinancialMonthOfWrongFormatException();
            }
        }
    }
}
