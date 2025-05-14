using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Domain.Entities
{
    public class TaskNode
    {
        public required string TaskTitle { get; set; }

        public string? TaskDescription { get; set; }

        public int Id { get; set; }

        public DateTimeOffset Deadline { get; set; }

        public TaskStatus? TaskStatus { get; set; }

        public List<TaskNode> DependentNodes { get; set; } = [];
        public List<Comment> Comments { get; set; }

        public int Priority { get; set; }

        public required Project Project { get; set; }

        public User? AssignedUser { get; set; }
    }

    public enum TaskStatus
    {
        Ready,
        Doing,
        Done
    }
}
