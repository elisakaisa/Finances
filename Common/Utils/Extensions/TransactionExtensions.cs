using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Utils.Extensions
{
    public static class TransactionExtensions
    {
        #region Sum extensions
        public static decimal SumTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId)
        {
            return transactions.Where(t => t.Subcategory.CategoryId == categoryId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId, Guid userId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId && t.UserId == userId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId, Guid userId)
        {
            return transactions.Where(t => t.Subcategory.CategoryId == categoryId && t.UserId == userId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsBySubCategory(this ICollection<Transaction> transactions, int subcategoryId, Guid householdId)
        {
            return transactions.Where(t => t.SubcategoryId == subcategoryId && t.User.HouseholdId == householdId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsByCategory(this ICollection<Transaction> transactions, int categoryId, Guid householdId)
        {
            return transactions.Where(t => t.Subcategory.CategoryId == categoryId && t.User.HouseholdId == householdId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumTotalByTransactionType(this ICollection<Transaction> transactions, TransactionType type)
        {
            return transactions.Where(t => t.TransactionType == type).ToList().Sum(t => t.Amount);
        }

        public static decimal SumUserTransactionsByType(this ICollection<Transaction> transactions, TransactionType type, Guid userId)
        {
            return transactions.Where(t => t.TransactionType == type && t.UserId == userId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumHouseholdTransactionsByType(this ICollection<Transaction> transactions, TransactionType type, Guid householdId)
        {
            return transactions.Where(t => t.TransactionType == type && t.User.HouseholdId == householdId).ToList().Sum(t => t.Amount);
        }

        public static decimal SumCommonExpensesByUser(this ICollection<Transaction> transactions, Guid userId)
        {
            return transactions.Where(t => t.IsTransactionCommon() && t.UserId == userId).ToList().Sum(t => t.Amount);
        }
        public static decimal SumCommonExpensesByHousehold(this ICollection<Transaction> transactions, Guid householdId)
        {
            return transactions.Where(t => t.IsTransactionCommon() && t.User.HouseholdId == householdId).ToList().Sum(t => t.Amount);
        }
        #endregion


        public static bool IsTransactionCommon(this Transaction transaction) 
        {
            if (transaction.TransactionType != TransactionType.Expenses) return false;
            if (transaction.SplitType == SplitType.Individual) return false;
            return true;
        }
    }
}
