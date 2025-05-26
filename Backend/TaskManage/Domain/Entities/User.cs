namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string UserName { get; set; }

        public required string PasswordHash { get; set; }

        public string? AvatarUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public bool IsActive { get; set; }

        public UserRole UserRole { get; set; }

        public List<TaskNode>? AssignedTasks { get; set; } = [];

        public List<TaskNode>? OwnTaskNodes { get; set; } = [];

        public List<Comment>? Comments { get; set; } = [];

        public List<Project>? Projects { get; set; } = [];

        public List<AuditLogs>? AuditLogs { get; set; } = [];
    }

    public enum UserRole
    {
        Admin,
        ProjectAdmin,
        ProjectUser
    }
}
