namespace Domain.Entities
{
    public class Comment
    {
        public required TaskNode Task { get; set; }
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public required User Owner { get; set; }
        public bool IsHidden { get; set; }
    }
}
