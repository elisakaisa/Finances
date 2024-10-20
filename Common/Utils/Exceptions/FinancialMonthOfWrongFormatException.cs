namespace Common.Utils.Exceptions
{
    public class FinancialMonthOfWrongFormatException : Exception
    {
        public FinancialMonthOfWrongFormatException() { }

        public FinancialMonthOfWrongFormatException(string message) : base(message) { }

        public FinancialMonthOfWrongFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
