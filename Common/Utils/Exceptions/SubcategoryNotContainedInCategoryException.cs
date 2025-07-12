namespace Common.Utils.Exceptions
{
    public class SubcategoryNotContainedInCategoryException : Exception
    {
        public SubcategoryNotContainedInCategoryException() { }

        public SubcategoryNotContainedInCategoryException(string message) : base(message) { }

        public SubcategoryNotContainedInCategoryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
