namespace Domain.Entities
{
    public class AuditLogs
    {
        public int Id { get; set; }

        public required User Owner { get; set; }

        public required string LogContent { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
