using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Tests.TestData
{
    public class GeneralTestData
    {
        public static readonly Guid Household1Id = Guid.Parse("00000000-0000-0000-0000-000000000010");
        public static readonly Guid Household2Id = Guid.Parse("00000000-0000-0000-0000-000000000020");

        public static readonly Guid User1Hh1Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        public static readonly Guid User2Hh1Id = Guid.Parse("00000000-0000-0000-0000-000000000012");

        public Household Household10 = new()
        {
            Id = Household1Id,
            Name = "Household10"
        };

        public Household Household20 = new()
        {
            Id = Household2Id,
            Name = "Household20"
        };


        // Users in household 1
        public static User User11 = new()
        {
            Id = User1Hh1Id,
            Name = "User11",
            HouseholdId = Household1Id
        };

        public static User User12 = new()
        {
            Id = User2Hh1Id,
            Name = "User12",
            HouseholdId = Household1Id
        };


        // Users in household 2
        public static User User21 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
            Name = "User21",
            HouseholdId = Household2Id
        };

        public static User User22 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Name = "User22",
            HouseholdId = Household2Id
        };


        // Categories and subcategories
        public static Category Income = new()
        {
            Id = 1,
            Name = "Income",
            TransactionType = TransactionType.Income,
            Subcategories = new List<Subcategory>()
        };

        public static Subcategory Salary = new()
        {
            Id = 11,
            Name = "Salary",
            CategoryId = 1,
            State = State.Active
        };

        public static Subcategory IncomeMisc = new()
        {
            Id = 12,
            Name = "IncomeMisc",
            CategoryId = 1,
            State = State.Active
        };


        public static Category Utilities = new()
        {
            Id = 2,
            Name = "Utilities",
            TransactionType = TransactionType.Expenses,
            Subcategories = new List<Subcategory>()
        };


        public static Subcategory Electricity = new()
        {
            Id = 21,
            Name = "Electricity",
            CategoryId = 2,
            State = State.Active,
            Category = Utilities
        };

        public static Subcategory HomeInsurance = new()
        {
            Id = 22,
            Name = "HomeInsurance",
            CategoryId = 2,
            State = State.Active,
            Category = Utilities
        };

        protected static void InitializeSubcategories()
        {
            // Set the Category for each subcategory
            Electricity.Category = Utilities;
            HomeInsurance.Category = Utilities;

            // Add subcategories to Utilities
            Utilities.Subcategories.Add(Electricity);
            Utilities.Subcategories.Add(HomeInsurance);

            // Set the Category for each subcategory
            Salary.Category = Income;
            IncomeMisc.Category = Income;

            // Add subcategories to the Income category
            Income.Subcategories.Add(Salary);
            Income.Subcategories.Add(IncomeMisc);
        }



        protected Transaction CreateTransaction(
            decimal amount,
            TransactionType type,
            SplitType splitType = SplitType.Even,
            Guid? userId = null,
            User user = null,
            string financialMonth = "202412",
            decimal? userShare = null,
            int? subcategoryId = null,
            Subcategory? subcategory = null) => new()
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                TransactionType = type,
                SplitType = splitType,
                UserId = userId ?? User1Hh1Id,
                User = user ?? User11,
                Description = "test",
                FinancialMonth = financialMonth,
                UserShare = userShare,
                SubcategoryId = subcategoryId ?? 1,
                Subcategory = subcategory ?? new Subcategory
                {
                    Name = "test",
                }
            };

        protected MonthlyIncomeAfterTax CreateMonthlyIncome(decimal income, Guid userId, User user) => new()
        {
            IncomeAfterTax = income,
            UserId = userId,
            User = user
        };



        // Generic Transactions without user / household
        public static List<Transaction> TestTransactions =
        [
            genericExpense1, genericIncome1, genericExpense2, genericIncome2
        ];

        public static Transaction genericExpense1 = new()
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
            Description = "Home insurance",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Insur",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Expenses,
            SplitType = SplitType.Individual,
            UserShare = null,
            Amount = 150.75m,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Debit,
            FinancialMonth = "202412",

            // Foreign Keys
            SubcategoryId = 22,
        };
        public static Transaction genericIncome1 = new()
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
            Description = "Salary",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Employer",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Income,
            SplitType = SplitType.Individual,
            UserShare = null,
            Amount = 15000,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Transfer,
            FinancialMonth = "202412",

            // Foreign Keys
            SubcategoryId = 11,
        };
        public static Transaction genericExpense2 = new()
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
            Description = "Electricity",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "El",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Expenses,
            SplitType = SplitType.Individual,
            UserShare = null,
            Amount = 120,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Debit,
            FinancialMonth = "202412",

            // Foreign Keys
            SubcategoryId = 21,
        };
        public static Transaction genericIncome2 = new()
        {
            Id = Guid.Parse("40000000-0000-0000-0000-000000000000"),
            Description = "Interest",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Bank",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Income,
            SplitType = SplitType.Individual,
            UserShare = null,
            Amount = 15.23m,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Transfer,
            FinancialMonth = "202412",

            // Foreign Keys
            SubcategoryId = 12,
        };


    }
}
