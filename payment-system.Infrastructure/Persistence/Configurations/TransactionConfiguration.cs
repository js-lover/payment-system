using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using payment_system.Domain.Entities;

namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions").HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);


            //transaction is associated with an account
            builder.HasOne(x => x.Account)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            //transaction is associated with a card
            builder.HasOne(x => x.Card)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Restrict);


            // Amount configuration
            builder.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            // Currency configuration
            builder.Property(x => x.Currency)
                .IsRequired();

            // TransactionDate configuration
            builder.Property(x => x.TransactionDate)
                .IsRequired();

            // IsDeleted configuration
            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);



            // Description configuration
            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // TransactionType configuration
            builder.Property(x => x.TransactionType)
                .IsRequired();

            // Status configuration
            builder.Property(x => x.Status)
                .IsRequired();


            //self referencing
            builder.HasOne(x => x.ReferenceTransaction)
                .WithMany(x => x.ChildTransactions)
                .HasForeignKey(x => x.ReferenceTransactionId);

            // End self referencing configuration
            builder.HasOne(x => x.Card)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.CardId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}