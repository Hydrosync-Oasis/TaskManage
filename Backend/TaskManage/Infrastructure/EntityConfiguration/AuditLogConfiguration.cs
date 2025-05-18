using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfiguration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLogs>
{
    public void Configure(EntityTypeBuilder<AuditLogs> builder)
    {
        builder.ToTable("T_Logs");
        builder.HasKey(e => e.Id);
        builder.HasOne(x => x.Owner).WithMany(y => y.AuditLogs);

    }
}
