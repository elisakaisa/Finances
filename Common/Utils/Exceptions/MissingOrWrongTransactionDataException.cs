namespace Common.Utils.Exceptions
{
    public class MissingOrWrongTransactionDataException : Exception
    {
        public MissingOrWrongTransactionDataException() { }
        public MissingOrWrongTransactionDataException(string message) : base(message) { }
        public MissingOrWrongTransactionDataException(string message, Exception innerException) : base(message, innerException){ }
    }
}
