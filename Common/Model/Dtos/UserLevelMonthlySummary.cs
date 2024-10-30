using Common.Model.DatabaseObjects;

namespace Common.Model.Dtos
{
    public class UserLevelMonthlySummary
    {
        public required decimal IndividualTotal { get; set; }
        public required decimal CommonTotal { get; set; }
        public required decimal Total { get; set; }
        public required User User { get; set; }
    }
}
