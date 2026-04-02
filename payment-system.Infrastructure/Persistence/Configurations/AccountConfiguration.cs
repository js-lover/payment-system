using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;


namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {

            //define the table
            builder.ToTable("Accounts")
                .HasQueryFilter(x => !x.IsDeleted);

            //primary key
            builder.HasKey(x => x.Id);

            //creates one-to-many relationship with Customer
            //ondelete restrict does not allow deletion of a customer if they have accounts
            builder.HasOne(x => x.Customer).WithMany(x => x.Accounts).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);

            //account number has a fixed length and unique
            builder.Property(x => x.AccountNumber)
                .HasMaxLength(20)
                .IsFixedLength();

            builder.HasIndex(x => x.AccountNumber).IsUnique();



            //checks if AccountNumber is in the correct format and length
            builder.HasCheckConstraint(
                "CK_Account_AccountNumber_Format", 
                "length([AccountNumber]) = 20 AND substr([AccountNumber], 1, 2) = 'TR' AND typeof([AccountNumber]) = 'text'");

            //balance
            builder.Property(x => x.Balance).HasPrecision(18, 2);


            //currency
            // hasconversion is for the enums to be visible as "try" "usd" ... 
            builder.Property(x => x.Currency).HasConversion<String>();

        }
    }
}