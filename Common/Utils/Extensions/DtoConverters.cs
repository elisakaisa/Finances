using Common.Model.DatabaseObjects;
using Common.Model.Dtos;
using Common.Model.Enums;
using Common.Utils.Exceptions;

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
                TransactionType = transaction.TransactionType.ToString(),
                SplitType = transaction.SplitType.ToString(),
                UserShare = transaction.UserShare,
                Amount = transaction.Amount,
                ToVerify = transaction.ToVerify,
                ModeOfPayment = transaction.ModeOfPayment.ToString(),
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

        public static MonthlyIncomeAfterTaxDto ConvertToDto(this MonthlyIncomeAfterTax monthlyIncomeAfterTax)
        {
            return new MonthlyIncomeAfterTaxDto
            {
                Id = monthlyIncomeAfterTax.Id,
                IncomeAfterTax = monthlyIncomeAfterTax.IncomeAfterTax,
                FinancialMonth = monthlyIncomeAfterTax.FinancialMonth,
                UserId = monthlyIncomeAfterTax.UserId
            };
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
                TransactionType = transactionDto.TransactionType.ConvertTransactionTypeToDb(),
                SplitType = transactionDto.SplitType.ConvertSplitTypeToDb(),
                UserShare = transactionDto.UserShare,
                Amount = transactionDto.Amount,
                ToVerify = transactionDto.ToVerify,
                ModeOfPayment = transactionDto.ModeOfPayment.ConvertModeOfPaymentToDb(),
                FinancialMonth = transactionDto.FinancialMonth,
                SubcategoryId = subcategory.Id,
                Subcategory = subcategory,
                UserId = user.Id,
                User = user
            };
        }

        public static MonthlyIncomeAfterTax ConvertToDbObject(this MonthlyIncomeAfterTaxDto monthlyIncomeAfterTaxDto, User user)
        {
            return new MonthlyIncomeAfterTax
            {
                Id = monthlyIncomeAfterTaxDto.Id,
                IncomeAfterTax = monthlyIncomeAfterTaxDto.IncomeAfterTax,
                FinancialMonth = monthlyIncomeAfterTaxDto.FinancialMonth,
                UserId = monthlyIncomeAfterTaxDto.UserId,
                User = user
            };
        }

        public static TransactionType ConvertTransactionTypeToDb(this string transactionType)
        {
            return transactionType switch
            {
                "Expenses" => TransactionType.Expenses,
                "Income" => TransactionType.Income,
                "Savings" => TransactionType.Savings,
                _ => throw new MissingOrWrongDataException(),
            };
        }

        // option 2, to be tested
        public static SplitType ConvertSplitTypeToDb2(this string splitType)
        {
            if (Enum.TryParse(splitType, true, out SplitType result))
            {
                return result;
            }
            throw new MissingOrWrongDataException($"Invalid split type: {splitType}");
        }

        public static SplitType ConvertSplitTypeToDb(this string splitType)
        {
            return splitType switch
            {
                "Individual" => SplitType.Individual,
                "Even" => SplitType.Even,
                "IncomeBased" => SplitType.IncomeBased,
                "Custom" => SplitType.Custom,
                _ => throw new MissingOrWrongDataException(),
            };
        }

        public static ModeOfPayment ConvertModeOfPaymentToDb(this string modeOfPayment)
        {
            return modeOfPayment switch
            {
                "NA" => ModeOfPayment.NA,
                "Debit" => ModeOfPayment.Debit,
                "Transfer" => ModeOfPayment.Transfer,
                "Swish" => ModeOfPayment.Swish,
                "Autogiro" => ModeOfPayment.Autogiro,
                "Other" => ModeOfPayment.Other,
                _ => throw new MissingOrWrongDataException(),
            };
        }
    }
}
