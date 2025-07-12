namespace Common.Utils.Exceptions
{
    public class MissingOrWrongDataException : Exception
    {
        public MissingOrWrongDataException() { }
        public MissingOrWrongDataException(string message) : base(message) { }
        public MissingOrWrongDataException(string message, Exception innerException) : base(message, innerException) { }
    }
}
