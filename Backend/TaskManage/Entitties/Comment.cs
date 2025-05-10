using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities {
    public class Comment {
        public TaskNode Task { get; set; }

        public int Id { get; set; }

        public required string Content { get; set; }

        public DateTimeOffset CreatedTime { get; set; }

        public required User Owner { get; set; }
    }

    public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            builder.ToTable("T_Comment");
            builder.HasOne(x => x.Owner).WithMany(y => y.Comments);
            builder.HasOne(x => x.Task).WithMany(y => y.Comments);
        }
    }
}
