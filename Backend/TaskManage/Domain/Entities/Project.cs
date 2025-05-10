using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

        public required User Owner { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public List<TaskNode> Tasks { get; set; } = [];
    }

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("T_Project");
            builder.HasKey(t => t.Id);
            builder.HasOne(x => x.Owner).WithMany(y => y.Projects);
        }
    }
}
