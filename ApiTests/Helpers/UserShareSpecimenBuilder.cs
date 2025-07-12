using AutoFixture.Kernel;
using Common.Model.DatabaseObjects;
using System.Reflection;

namespace ApiTests.Helpers
{
    public class UserShareSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is PropertyInfo pi &&
                pi.Name == nameof(Transaction.UserShare) &&
                pi.PropertyType == typeof(decimal))
            {
                // Generate a value between 0 and 99.99
                var value = Math.Round((decimal)(Random.Shared.NextDouble() * 99), 2);
                return value;
            }

            return new NoSpecimen();
        }
    }
}
