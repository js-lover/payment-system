using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using payment_system.Domain.Entities;

namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email).HasFilter("IsDeleted = 0").IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Role)
                .IsRequired();
        }
    }
}