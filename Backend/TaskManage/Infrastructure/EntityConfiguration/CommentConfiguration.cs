using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("T_Comment");
        builder.HasOne(x => x.Owner).WithMany(y => y.Comments);
        builder.HasOne(x => x.Task).WithMany(y => y.Comments);
    }
}
