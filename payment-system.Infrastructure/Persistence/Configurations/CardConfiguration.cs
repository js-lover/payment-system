using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using payment_system.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {

            builder.ToTable("Cards", t =>
            {
                t.HasCheckConstraint("CK_Card_CardNumber_Format", "length(CardNumber) = 16");
                t.HasCheckConstraint("CK_Card_ExpirationDate_Format", "length(ExpirationDate) = 5");
            }).HasQueryFilter(x => !x.IsDeleted);

            builder.HasKey(x => x.Id);

            //card is associated with an account
            builder.HasOne(x => x.Account)
                .WithMany(x => x.Cards)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            //card name must be unique and not empty
            builder.Property(x => x.CardName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(x => x.CardName)
                .IsUnique();

            //card number has 16 digits 
            builder.Property(x => x.CardNumber)
                .IsRequired()
                .HasMaxLength(16)
                .IsFixedLength();
            // for uniqueness of card number
            builder.HasIndex(x => x.CardNumber)
                .IsUnique();

            builder.Property(x => x.ExpirationDate)
                .IsRequired()
                .HasMaxLength(5)
                .IsFixedLength();

        }

    }
}