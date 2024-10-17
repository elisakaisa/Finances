﻿using Common.Model;
using Common.Model.Enums;

namespace Common.Tests
{
    public class TestData
    {
        public static readonly Guid Household1 = Guid.Parse("00000000-0000-0000-000000000010");
        public static readonly Guid Household2 = Guid.Parse("00000000-0000-0000-000000000020");

        public static readonly Guid User1Hh1 = Guid.Parse("00000000-0000-0000-000000000011");
        public static readonly Guid User2Hh1 = Guid.Parse("00000000-0000-0000-000000000012");

        public static readonly Guid FinancialMonthDecHh1 = Guid.Parse("00000000-0012-0000-000000000010");
        public static readonly Guid FinancialMonthJanHh1 = Guid.Parse("00000000-0001-0000-000000000010");
        public static readonly Guid FinancialMonthFebHh1 = Guid.Parse("00000000-0002-0000-000000000010");

        public Household Household10 = new()
        {
            Id = Household1,
            Name = "Household10"
        };

        public Household Household20 = new()
        {
            Id = Household2,
            Name = "Household20"
        };


        // Users in household 1
        public User User11 = new()
        {
            Id = User1Hh1,
            Name = "User11",
            HouseholdId = Household1
        };

        public User User12 = new()
        {
            Id = User2Hh1,
            Name = "User12",
            HouseholdId = Household1
        };


        // Users in household 2
        public User User21 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000021"),
            Name = "User21",
            HouseholdId = Household2
        };

        public User User22 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000022"),
            Name = "User22",
            HouseholdId = Household2
        };


        // FinancialMonths
        public FinancialMonth financialMonthDecHh1 = new()
        {
            Id = FinancialMonthDecHh1,
            Name = "2024-12",
            FinancialMonthName = FinancialMonthName.DEC,
            FinancialYear = 2024,

            HouseholdId = Household1
        };

        public FinancialMonth financialMonthDecHh2 = new()
        {
            Id = FinancialMonthJanHh1,
            Name = "2025-01",
            FinancialMonthName = FinancialMonthName.JAN,
            FinancialYear = 2025,

            HouseholdId = Household1
        };

        public FinancialMonth financialMonthFebHh2 = new()
        {
            Id = FinancialMonthJanHh1,
            Name = "2025-02",
            FinancialMonthName = FinancialMonthName.JAN,
            FinancialYear = 2025,

            HouseholdId = Household1
        };


        // Categories and subcategories
        public static Subcategory Salary = new()
        {
            Id = 11,
            Name = "Salary",
            CategoryId = 1,
            State = Model.Enums.State.Active
        };
        public static Subcategory IncomeMisc = new()
        {
            Id = 12,
            Name = "IncomeMisc",
            CategoryId = 1,
            State = Model.Enums.State.Active
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
            State = Model.Enums.State.Active
        };
        public static Subcategory HomeInsurance = new()
        {
            Id = 22,
            Name = "HomeInsurance",
            CategoryId = 1,
            State = Model.Enums.State.Active
        };

        public Category Utilities = new()
        {
            Id = 2,
            Name = "Income",
            Subcategories = [Electricity, HomeInsurance]
        };



        // Transactions
        public static Transaction transaction1 = new()
        {
            Id = Guid.Parse("10000000-0000-0000-000000000000"),
            Description = "Home insurance",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Insur",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Expenses,
            SplitType = SplitType.Individual,
            Split = null,
            Amount = 150.75,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Debit,

            // Foreign Keys
            CategoryId = 2,
            SubcategoryId = 22, 
            UserId = User1Hh1,
            FinancialMonthId = FinancialMonthDecHh1
        };
        public static Transaction transaction2 = new()
        {
            Id = Guid.Parse("20000000-0000-0000-000000000000"),
            Description = "Salary",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Employer",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Income,
            SplitType = SplitType.Individual,
            Split = null,
            Amount = 15000,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Transfer,

            // Foreign Keys
            CategoryId = 1,
            SubcategoryId = 11,
            UserId = User1Hh1,
            FinancialMonthId = FinancialMonthDecHh1
        };
        public static Transaction transaction3 = new()
        {
            Id = Guid.Parse("30000000-0000-0000-000000000000"),
            Description = "Electricity",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "El",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Expenses,
            SplitType = SplitType.Individual,
            Split = null,
            Amount = 120,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Debit,

            // Foreign Keys
            CategoryId = 2,
            SubcategoryId = 21,
            UserId = User1Hh1,
            FinancialMonthId = FinancialMonthDecHh1
        };
        public static Transaction transaction4 = new()
        {
            Id = Guid.Parse("40000000-0000-0000-000000000000"),
            Description = "Interest",
            Date = new DateOnly(2024, 10, 17),
            FromOrTo = "Bank",
            Location = "Stockholm",
            ExcludeFromSummary = false,
            TransactionType = TransactionType.Income,
            SplitType = SplitType.Individual,
            Split = null,
            Amount = 15.23,
            ToVerify = false,
            ModeOfPayment = ModeOfPayment.Transfer,

            // Foreign Keys
            CategoryId = 1,
            SubcategoryId = 12,
            UserId = User1Hh1,
            FinancialMonthId = FinancialMonthDecHh1
        };


    }
}
