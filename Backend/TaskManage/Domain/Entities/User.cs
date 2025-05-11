using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public required string UserName { get; set; }

        public required string PasswordHash { get; set; }

        public string? Avatar { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public bool IsActive { get; set; }

        public List<TaskNode> Tasks { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];

        public List<Project> Projects { get; set; } = [];

        public List<AuditLogs> AuditLogs { get; set; } = [];
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("T_User");
            builder.HasKey(x => x.Id);

        }
    }
}
