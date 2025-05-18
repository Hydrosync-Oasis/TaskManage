using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using TaskStatus = Domain.Entities.TaskStatus;

namespace Application.DTOs {
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
    }
}
