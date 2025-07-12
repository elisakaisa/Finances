using AutoFixture.Kernel;
using Common.Model.DatabaseObjects;
using System.Reflection;

namespace ApiTests.Helpers
{
    public class UserShareSpecimenBuilder : ISpecimenBuilder
    {
        private static readonly decimal MaxValue = 99.99m;
        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi &&
                pi.Name == nameof(Transaction.UserShare) &&
                (pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?)))
            {
                return GenerateRandomDecimal(0m, MaxValue, 2);
            }

            return new NoSpecimen();
        }

        private static decimal GenerateRandomDecimal(decimal min, decimal max, int decimals)
        {
            var randomDouble = Random.Shared.NextDouble();
            decimal scaled = (decimal)randomDouble * (max - min) + min;
            return Math.Round(scaled, decimals);
        }
    }
}
