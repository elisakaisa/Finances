using Common.Model;

namespace Common.Tests
{
    public class TestData
    {
        public Household Household10 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000010"),
            Name = "Household10"
        };

        public Household Household20 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000020"),
            Name = "Household20"
        };


        // Users in household 1
        public User User11 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000011"),
            Name = "User11",
            HouseholdId = Guid.Parse("00000000-0000-0000-000000000010")
        };

        public User User12 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000012"),
            Name = "User12",
            HouseholdId = Guid.Parse("00000000-0000-0000-000000000010")
        };

        public User User13 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000013"),
            Name = "User13",
            HouseholdId = Guid.Parse("00000000-0000-0000-000000000010")
        };


        // Users in household 2
        public User User21 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000021"),
            Name = "User21",
            HouseholdId = Guid.Parse("00000000-0000-0000-000000000020")
        };

        public User User22 = new()
        {
            Id = Guid.Parse("00000000-0000-0000-000000000022"),
            Name = "User22",
            HouseholdId = Guid.Parse("00000000-0000-0000-000000000020")
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


    }
}
