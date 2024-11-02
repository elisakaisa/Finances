using Common.Model.DatabaseObjects;
using Common.Model.Dtos;

namespace Common.Utils.Extensions
{
    public static class DtoConverters
    {
        public static TransactionDto ConvertToDto(this Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Date = transaction.Date,
                FromOrTo = transaction.FromOrTo,
                Location = transaction.Location,
                ExcludeFromSummary = transaction.ExcludeFromSummary,
                TransactionType = transaction.TransactionType,
                SplitType = transaction.SplitType,
                UserShare = transaction.UserShare,
                Amount = transaction.Amount,
                ToVerify = transaction.ToVerify,
                ModeOfPayment = transaction.ModeOfPayment,
                FinancialMonth = transaction.FinancialMonth,
                SubcategoryName = transaction.Subcategory.Name,
                CategoryName = transaction.Subcategory.Category.Name,
                UserId = transaction.UserId
            };
        }

        public static ICollection<TransactionDto> ConvertToDto(this ICollection<Transaction> transactions)
        {
            return transactions.Select(transaction => transaction.ConvertToDto()).ToList();
        }

        public static Transaction ConvertToDbObject(this TransactionDto transactionDto, User user, Subcategory subcategory)
        {
            return new Transaction
            {
                Id = transactionDto.Id,
                Description = transactionDto.Description,
                Date = transactionDto.Date,
                FromOrTo = transactionDto.FromOrTo,
                Location = transactionDto.Location,
                ExcludeFromSummary = transactionDto.ExcludeFromSummary,
                TransactionType = transactionDto.TransactionType,
                SplitType = transactionDto.SplitType,
                UserShare = transactionDto.UserShare,
                Amount = transactionDto.Amount,
                ToVerify = transactionDto.ToVerify,
                ModeOfPayment = transactionDto.ModeOfPayment,
                FinancialMonth = transactionDto.FinancialMonth,
                SubcategoryId = subcategory.Id,
                Subcategory = subcategory,
                UserId = user.Id,
                User = user
            };
        }
    }
}
