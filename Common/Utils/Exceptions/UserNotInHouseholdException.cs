namespace Common.Utils.Exceptions
{
    public class UserNotInHouseholdException : Exception
    {
        public UserNotInHouseholdException() { }
        public UserNotInHouseholdException(string message) : base(message) { }
        public UserNotInHouseholdException(string message, Exception innerException) : base(message, innerException) { }
    }
}
