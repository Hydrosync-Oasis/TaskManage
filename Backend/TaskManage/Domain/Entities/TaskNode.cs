namespace Domain.Entities
{
    public class TaskNode
    {
        public required string Title { get; set; }

        public string? Description { get; set; }

        public int Id { get; set; }

        public DateTimeOffset Deadline { get; set; }

        public TaskStatus? TaskStatus { get; set; }
        public User CreatedBy { get; set; }
        public required int CreateUserId { get; set; }

        public List<TaskNode> DependentNodes { get; set; } = [];
        public List<TaskNode> SuccessorNodes { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];

        public int Priority { get; set; }

        public Project Project { get; set; }
        public required int ProjectId { get; set; }

        public User? AssignedUser { get; set; }
        public int? AssignedUserId { get; set; }
    }

    public enum TaskStatus
    {
        Ready,
        Doing,
        Done
    }
}
