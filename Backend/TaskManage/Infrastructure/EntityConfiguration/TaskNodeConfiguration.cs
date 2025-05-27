using System.Security.Cryptography.X509Certificates;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class TaskNodeConfiguration : IEntityTypeConfiguration<TaskNode>
{
    public void Configure(EntityTypeBuilder<TaskNode> builder)
    {
        builder.ToTable("T_TaskNode");
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.AssignedUser)
            .WithMany(y => y.AssignedTasks)
            .HasForeignKey(x => x.AssignedUserId)
            //.IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Project).WithMany(y => y.Tasks);
        builder.HasMany(x => x.DependentNodes).WithMany();
        builder.HasOne(x => x.CreatedBy)
            .WithMany((x) => x.OwnTaskNodes)
            .HasForeignKey(x => x.CreateUserId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
