using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using payment_system.Domain.Entities;

namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            //defining table name
            builder.ToTable("Customers").HasQueryFilter(x => !x.IsDeleted);

            //defining primary key
            builder.HasKey(x => x.Id);

            //foreign key for user one user one customer
            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);



            // configurating columns 
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.NationalId)
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();

            //checks if NationalId is in the correct format
            builder.ToTable("Customers", t =>
            {
                t.HasCheckConstraint("CK_Customer_NationalId_Format", "length(NationalId) = 11 AND NationalId NOT GLOB '*[^0-9]*'");
            });
            builder.HasIndex(x => x.NationalId).HasFilter("IsDeleted = 0").IsUnique();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email).HasFilter("IsDeleted = 0").IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.DateOfBirth)
                .IsRequired();

        }
    }
}