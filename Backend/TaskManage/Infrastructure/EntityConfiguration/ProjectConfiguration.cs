using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("T_Project");
            builder.HasKey(t => t.Id);

            // 配置 Project -> User (Owner) 的关系
            builder.HasOne(x => x.Owner)
                   .WithMany(y => y.Projects)
                   .HasForeignKey("OwnerId"); // 建议明确指定外键名称

            // 配置 Project -> TaskNode 的一对多关系
            builder.HasMany(p => p.Tasks)
                   .WithOne(t => t.Project)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade); // 项目删除时，任务一起删除
        }
    }
}
