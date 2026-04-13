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

            builder.ToTable("Cards").HasQueryFilter(x => !x.IsDeleted);

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

            //checks if card number has 16 digits and its all numbers
            builder.HasCheckConstraint("CK_Card_CardNumber_Format", "length([CardNumber]) = 16 AND [CardNumber] NOT GLOB '*[^0-9]*'");

            builder.Property(x => x.ExpirationDate)
                .IsRequired()
                .HasMaxLength(5)
                .IsFixedLength();

            //checks if expiration date is in MM/YY format
            builder.HasCheckConstraint("CK_Card_ExpirationDate_Format", "length([ExpirationDate]) = 5 AND [ExpirationDate] LIKE '__/__' AND [ExpirationDate] NOT LIKE '%[^0-9/]%'");

            //checks if CVC is 3 digits
            builder.Property(x => x.CVC)
                .IsRequired()
                .HasMaxLength(3)
                .IsFixedLength();

            builder.HasCheckConstraint("CK_Card_CVC_Format", "length([CVC]) = 3 AND [CVC] NOT GLOB '*[^0-9]*'");

        }

    }
}