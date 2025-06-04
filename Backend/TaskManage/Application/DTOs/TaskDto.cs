using TaskStatus = Domain.Entities.TaskStatus;

namespace Application.Dtos {
    public class TaskDto {
        public int? Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public DateTimeOffset? Deadline { get; set; }

        public TaskStatus? Status { get; set; }

        public int? Priority { get; set; }
        public int? ProjectId { get; set; }

        public int? AssignedUid { get; set; }
        public int? CreateUserId { get; set; }

        public List<int>? DependencyTaskIds { get; set; }
        public List<int>? SuccessorTaskIds { get; set; }
    }
}
