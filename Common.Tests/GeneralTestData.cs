using Common.Model.DatabaseObjects;
using Common.Model.Enums;

namespace Common.Tests
{
    public class GeneralTestData
    {
        public static readonly Guid Household1Id = Guid.Parse("00000000-0000-0000-0000-000000000010");
        public static readonly Guid Household2Id = Guid.Parse("00000000-0000-0000-0000-000000000020");

        public static readonly Guid User1Hh1Id = Guid.Parse("00000000-0000-0000-0000-000000000011");
        public static readonly Guid User2Hh1Id = Guid.Parse("00000000-0000-0000-0000-000000000012");

        public static readonly Guid FinancialMonthDecHh1Id = Guid.Parse("00000000-0000-0012-0000-000000000010");
        public static readonly Guid FinancialMonthJanHh1Id = Guid.Parse("00000000-0000-0001-0000-000000000010");
        public static readonly Guid FinancialMonthFebHh1Id = Guid.Parse("00000000-0000-0002-0000-000000000010");

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
        public User User21 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
            Name = "User21",
            HouseholdId = Household2Id
        };

        public User User22 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
            Name = "User22",
            HouseholdId = Household2Id
        };


        // FinancialMonths
        public FinancialMonth financialMonthDecHh1 = new()
        {
            Id = FinancialMonthDecHh1Id,
            Name = "2024-12",
            FinancialMonthName = FinancialMonthName.DEC,
            FinancialYear = 2024,

            HouseholdId = Household1Id
        };

        public FinancialMonth financialMonthDecHh2 = new()
        {
            Id = FinancialMonthJanHh1Id,
            Name = "2025-01",
            FinancialMonthName = FinancialMonthName.JAN,
            FinancialYear = 2025,

            HouseholdId = Household1Id
        };

        public FinancialMonth financialMonthFebHh2 = new()
        {
            Id = FinancialMonthJanHh1Id,
            Name = "2025-02",
            FinancialMonthName = FinancialMonthName.JAN,
            FinancialYear = 2025,

            HouseholdId = Household1Id
        };


        // Categories and subcategories
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

        public Category Income = new()
        {
            Id = 1,
            Name = "Income",
            Subcategories = [Salary, IncomeMisc]
        };
        public static Subcategory Electricity = new()
        {
            Id = 21,
            Name = "Electricity",
            CategoryId = 1,
            State = State.Active
        };
        public static Subcategory HomeInsurance = new()
        {
            Id = 22,
            Name = "HomeInsurance",
            CategoryId = 1,
            State = State.Active
        };

        public Category Utilities = new()
        {
            Id = 2,
            Name = "Income",
            Subcategories = [Electricity, HomeInsurance]
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

            // Foreign Keys
            CategoryId = 2,
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

            // Foreign Keys
            CategoryId = 1,
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

            // Foreign Keys
            CategoryId = 2,
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

            // Foreign Keys
            CategoryId = 1,
            SubcategoryId = 12,
        };


    }
}
