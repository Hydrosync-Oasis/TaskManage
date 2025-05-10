using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Entities {
    public class AuditLogs {
        public int Id { get; set; }

        public required User Owner { get; set; }

        public required string LogContent { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogs> {
        public void Configure(EntityTypeBuilder<AuditLogs> builder) {
            builder.ToTable("T_Logs");
            builder.HasKey(e => e.Id);
            builder.HasOne(x => x.Owner).WithMany(y => y.AuditLogs);

        }
    }
}
