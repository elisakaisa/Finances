using Common.Model.DatabaseObjects;

namespace Common.Model.Dtos
{
    public class MonthlyUserTransactionSum
    {
        public required decimal IndividualTotal { get; set; }
        public required decimal CommonTotal { get; set; }
        public required User User { get; set; }
    }
}
