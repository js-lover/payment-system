using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using payment_system.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using payment_system.Domain.Enums;


namespace payment_system.Infrastructure.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {

        //base constructor method to initialize the DbContext with options
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }


        // Define your DbSet properties for each entity here
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        //used ApplyConfigurationsFromAssembly to register all entity configurations
        //instead of manually configuring each entity 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);



            // 1. Sabit ID'ler (İlişkileri kurabilmek için şart)
            var customerId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d");
            var accountId = Guid.Parse("b2c3d4e5-f6a7-4b5c-9d8e-1f2a3b4c5d6e");
            var saleId = Guid.Parse("c3d4e5f6-a7b8-4c5d-8e9f-2a3b4c5d6e7f");
            var refundId = Guid.Parse("d4e5f6a7-b8c9-4d5e-9f0a-3b4c5d6e7f8a");

            // 2. Örnek Müşteri (Customer)
            modelBuilder.Entity<Customer>().HasData(new Customer
            {
                Id = customerId,
                Name = "Floyd",
                Surname = "Pro",
                Email = "floyd@example.com",
                NationalId = "12345678901",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                PasswordHash = "seed_data_temporary_hash", // Temporary hash for seeding 
                IsDeleted = false
            });

            // 3. Örnek Hesap (Account)
            modelBuilder.Entity<Account>().HasData(new Account
            {
                Id = accountId,
                CustomerId = customerId,
                AccountNumber = "TR001234567890123456789012",
                Balance = 10000m,
                Currency = Currency.TRY,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            });

            // 4. ANA İŞLEM: Satış (Parent Transaction)
            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                Id = saleId,
                AccountId = accountId,
                Amount = 500.00m,
                TransactionType = TransactionType.Sale,
                Status = TransactionStatus.Success,
                TransactionDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Description = "Market Alışverişi",
                ReferenceTransactionId = null, // Bu ana işlemdir
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            });

            // 5. BAĞLI İŞLEM: İade (Child Transaction - Self Referencing Test)
            modelBuilder.Entity<Transaction>().HasData(new Transaction
            {
                Id = refundId,
                AccountId = accountId,
                Amount = 100.00m,
                TransactionType = TransactionType.Refund,
                Status = TransactionStatus.Success,
                TransactionDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Description = "Ürün İadesi",
                ReferenceTransactionId = saleId, // İŞTE BURASI! Satış işlemine bağladık.
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            });
        }

    }
}