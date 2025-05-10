using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pomelo.EntityFrameworkCore.MySql.Storage.Internal;

namespace Entities {
    public class TaskNode {
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

    public enum TaskStatus {
        Ready,
        Doing,
        Done
    }

    public class TaskNodeConfiguration : IEntityTypeConfiguration<TaskNode> {
        public void Configure(EntityTypeBuilder<TaskNode> builder) {
            builder.ToTable("T_TaskNode");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.AssignedUser).WithMany(y => y.Tasks);
            builder.HasOne(x => x.Project).WithMany(y => y.Tasks);
            builder.HasMany(x => x.DependentNodes).WithMany();

        }
    }
}
