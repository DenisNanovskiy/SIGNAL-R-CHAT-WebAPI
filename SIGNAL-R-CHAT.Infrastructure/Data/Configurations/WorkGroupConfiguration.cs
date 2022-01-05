using SIGNAL_R_CHAT.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Web.Data.Configurations
{
    public class WorkGroupConfiguration : IEntityTypeConfiguration<WorkGroup>
    {
        public void Configure(EntityTypeBuilder<WorkGroup> builder)
        {
            builder.ToTable("WorkGroups");

            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(s => s.Admin)
                .WithMany(u => u.WorkGroups)
                .IsRequired();
        }
    }
}
