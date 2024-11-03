namespace Common.Model.Dtos
{
    public class UserLevelMonthlySummary
    {
        public required decimal IndividualTotal { get; set; }
        public required decimal CommonTotal { get; set; }
        public required decimal Total { get; set; }
        public required Guid UserId { get; set; }
    }
}
