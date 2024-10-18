using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Utils.Extensions
{
    public static class TransactionExtensions
    {
        public static decimal SumTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId).Sum(t => t.Amount);
        }

        public static decimal SumTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId)
        {
            return transactions.Where(t => t.CategoryId == categoryId).Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId, Guid userId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId && t.UserId == userId).Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId, Guid userId)
        {
            return transactions.Where(t => t.CategoryId == categoryId && t.UserId == userId).Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId, Guid householdId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId && t.User.HouseholdId == householdId).Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId, Guid householdId)
        {
            return transactions.Where(t => t.CategoryId == categoryId && t.User.HouseholdId == householdId).Sum(t => t.Amount);
        }

        public static decimal SumTotalByTransactionType(this ICollection<Transaction> transactions, TransactionType type)
        {
            return transactions.Where(t => t.TransactionType == type).Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsByType(this ICollection<Transaction> transactions, TransactionType type, Guid userId)
        {
            return transactions.Where(t => t.TransactionType == type && t.UserId == userId).Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsByType(this ICollection<Transaction> transactions, TransactionType type, Guid householdId)
        {
            return transactions.Where(t => t.TransactionType == type && t.User.HouseholdId == householdId).Sum(t => t.Amount);
        }
    }
}
