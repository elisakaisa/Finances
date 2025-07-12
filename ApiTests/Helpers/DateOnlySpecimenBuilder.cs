using AutoFixture.Kernel;

namespace ApiTests.Helpers
{
    public class DateOnlySpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(DateOnly))
            {
                return new DateOnly(
                    year: Random.Shared.Next(2000, 2100),
                    month: Random.Shared.Next(1, 13),
                    day: Random.Shared.Next(1, 28)
                );
            }

            return new NoSpecimen();
        }
    }
}
