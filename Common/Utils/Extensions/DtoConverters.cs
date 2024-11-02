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
    }
}
