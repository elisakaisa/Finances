namespace Common.Utils.Exceptions
{
    public class HouseholdWithMoreThanTwoUsersNotSupportedException : Exception
    {
        public HouseholdWithMoreThanTwoUsersNotSupportedException() { }

        public HouseholdWithMoreThanTwoUsersNotSupportedException(string message) : base(message) { }

        public HouseholdWithMoreThanTwoUsersNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
