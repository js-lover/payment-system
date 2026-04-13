# 🗄️ Kart Yönetimi - Veritabanı Şeması & EF Core Configuration

## 1. Card Entity Migration

```csharp
// Dosya: payment-system.Infrastructure/Migrations/202X0X0X_AddCardFeatures.cs

public partial class AddCardFeatures : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: \"Cards\",
            columns: table => new
            {
                Id = table.Column<Guid>(type: \"TEXT\", nullable: false),
                CardNumber = table.Column<string>(type: \"TEXT\", maxLength: 20, nullable: false),
                CardName = table.Column<string>(type: \"TEXT\", maxLength: 100, nullable: false),
                ExpirationDate = table.Column<DateTime>(type: \"TEXT\", nullable: false),
                CVC = table.Column<string>(type: \"TEXT\", nullable: false), // Encrypted
                AccountId = table.Column<Guid>(type: \"TEXT\", nullable: false),
                Status = table.Column<int>(type: \"INTEGER\", nullable: false),
                CreatedAt = table.Column<DateTime>(type: \"TEXT\", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: \"TEXT\", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey(\"PK_Cards\", x => x.Id);
                table.ForeignKey(
                    name: \"FK_Cards_Accounts_AccountId\",
                    column: x => x.AccountId,
                    principalTable: \"Accounts\",
                    principalColumn: \"Id\",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: \"IX_Cards_AccountId\",
            table: \"Cards\",
            column: \"AccountId\");

        migrationBuilder.CreateIndex(
            name: \"IX_Cards_CardNumber\",
            table: \"Cards\",
            column: \"CardNumber\",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: \"Cards\");
    }
}
```

---

## 2. Entity Framework Core Configuration

```csharp
// Dosya: payment-system.Infrastructure/Persistence/Configurations/CardConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using payment_system.Domain.Entities;
using payment_system.Domain.Enums;

namespace payment_system.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Card entity konfigürasyonu - Fluent API ile yapılandırma
    /// </summary>
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            // Tablo adı
            builder.ToTable(\"Cards\");

            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties - Zorunlu alanlar
            builder.Property(c => c.CardNumber)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName(\"CardNumber\")
                .HasColumnType(\"TEXT\");

            builder.Property(c => c.CardName)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName(\"CardName\")
                .HasColumnType(\"TEXT\");

            builder.Property(c => c.ExpirationDate)
                .IsRequired()
                .HasColumnName(\"ExpirationDate\")
                .HasColumnType(\"TEXT\");

            builder.Property(c => c.CVC)
                .IsRequired()
                .HasMaxLength(255) // Encrypted CVC, daha uzun olabilir
                .HasColumnName(\"CVC\")
                .HasColumnType(\"TEXT\");

            builder.Property(c => c.Status)
                .IsRequired()
                .HasDefaultValue(CardStatus.WaitingForApproval)
                .HasColumnName(\"Status\")
                .HasColumnType(\"INTEGER\");

            // Foreign Key
            builder.HasOne(c => c.Account)
                .WithMany(a => a.Cards)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.CardNumber)
                .IsUnique()
                .HasDatabaseName(\"IX_Cards_CardNumber\");

            builder.HasIndex(c => c.AccountId)
                .HasDatabaseName(\"IX_Cards_AccountId\");

            // CreatedAt / UpdatedAt (BaseEntity'den)
            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnName(\"CreatedAt\")
                .HasColumnType(\"TEXT\");

            builder.Property(c => c.UpdatedAt)
                .HasColumnName(\"UpdatedAt\")
                .HasColumnType(\"TEXT\");
        }
    }
}
```

---

## 3. DbContext Güncellemesi

```csharp
// Dosya: payment-system.Infrastructure/Persistence/Contexts/PaymentSystemDbContext.cs

using Microsoft.EntityFrameworkCore;
using payment_system.Domain.Entities;
using payment_system.Infrastructure.Persistence.Configurations;

namespace payment_system.Infrastructure.Persistence.Contexts
{
    public class PaymentSystemDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public PaymentSystemDbContext(DbContextOptions<PaymentSystemDbContext> options)
            : base(options)
        {
        }

        // Existing DbSets
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
        // NEW
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing configurations
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            
            // NEW - Card Configuration
            modelBuilder.ApplyConfiguration(new CardConfiguration());

            // Seed data (optional)
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Optional: Başlangıç verisi ekle
        }
    }
}
```

---

## 4. Database Schema - SQL View

```sql
-- ╔════════════════════════════════════════════════════════════════╗
-- ║                     CARDS TABLE SCHEMA                         ║
-- ╚════════════════════════════════════════════════════════════════╝

CREATE TABLE IF NOT EXISTS \"Cards\" (
    \"Id\" TEXT NOT NULL PRIMARY KEY,
    \"CardNumber\" TEXT NOT NULL UNIQUE,
    \"CardName\" TEXT NOT NULL,
    \"ExpirationDate\" TEXT NOT NULL,
    \"CVC\" TEXT NOT NULL,                    -- Encrypted
    \"AccountId\" TEXT NOT NULL,
    \"Status\" INTEGER NOT NULL DEFAULT 1,   -- CardStatus enum (1=WaitingForApproval)
    \"CreatedAt\" TEXT NOT NULL,
    \"UpdatedAt\" TEXT,
    FOREIGN KEY (\"AccountId\") REFERENCES \"Accounts\"(\"Id\") ON DELETE CASCADE
);

-- Indexes
CREATE UNIQUE INDEX \"IX_Cards_CardNumber\" ON \"Cards\"(\"CardNumber\");
CREATE INDEX \"IX_Cards_AccountId\" ON \"Cards\"(\"AccountId\");
CREATE INDEX \"IX_Cards_Status\" ON \"Cards\"(\"Status\");

-- ╔════════════════════════════════════════════════════════════════╗
-- ║                    RELATIONSHIP VIEW                           ║
-- ╚════════════════════════════════════════════════════════════════╝

Customers (1) ────────── (M) Accounts ────────── (M) Cards
                                                      │
                                                      └─── (M) Transactions
```

---

## 5. Example Queries

```sql
-- ═══════════════════════════════════════════════════════════════
-- READ Operations
-- ═══════════════════════════════════════════════════════════════

-- 1. Tüm kartları getir
SELECT * FROM \"Cards\" 
ORDER BY \"CreatedAt\" DESC;

-- 2. Belirli bir kartı getir
SELECT * FROM \"Cards\" 
WHERE \"Id\" = @cardId;

-- 3. Account'a ait tüm kartları getir
SELECT * FROM \"Cards\" 
WHERE \"AccountId\" = @accountId 
ORDER BY \"CreatedAt\" DESC;

-- 4. Active kartları getir
SELECT * FROM \"Cards\" 
WHERE \"Status\" = 2  -- CardStatus.Active
ORDER BY \"CreatedAt\" DESC;

-- 5. Müşterinin tüm kartlarını getir (Account üzerinden)
SELECT c.* FROM \"Cards\" c
INNER JOIN \"Accounts\" a ON c.\"AccountId\" = a.\"Id\"
INNER JOIN \"Customers\" cu ON a.\"CustomerId\" = cu.\"Id\"
WHERE cu.\"Id\" = @customerId
ORDER BY c.\"CreatedAt\" DESC;

-- 6. Son 30 gün içinde oluşturulan kartlar
SELECT * FROM \"Cards\" 
WHERE \"CreatedAt\" >= datetime('now', '-30 days')
ORDER BY \"CreatedAt\" DESC;

-- 7. Süresi dolmak üzere olan kartlar (30 gün içinde)
SELECT * FROM \"Cards\" 
WHERE \"ExpirationDate\" < datetime('now', '+30 days')
AND \"ExpirationDate\" > datetime('now')
AND \"Status\" = 2  -- Active
ORDER BY \"ExpirationDate\" ASC;

-- ═══════════════════════════════════════════════════════════════
-- WRITE Operations
-- ═══════════════════════════════════════════════════════════════

-- 8. Yeni kart ekle
INSERT INTO \"Cards\" 
(\"Id\", \"CardNumber\", \"CardName\", \"ExpirationDate\", \"CVC\", \"AccountId\", \"Status\", \"CreatedAt\")
VALUES 
(@id, @cardNumber, @cardName, @expirationDate, @cvc, @accountId, 1, @createdAt);

-- 9. Kartı güncelle (Status değiştir)
UPDATE \"Cards\" 
SET \"Status\" = @newStatus, \"UpdatedAt\" = @updatedAt
WHERE \"Id\" = @cardId;

-- 10. Kartı güncelle (Adını değiştir)
UPDATE \"Cards\" 
SET \"CardName\" = @cardName, \"UpdatedAt\" = @updatedAt
WHERE \"Id\" = @cardId;

-- ═══════════════════════════════════════════════════════════════
-- DELETE Operations
-- ═══════════════════════════════════════════════════════════════

-- 11. Kartı sil (Hard Delete)
DELETE FROM \"Cards\" 
WHERE \"Id\" = @cardId;

-- 12. Account'ın tüm kartlarını sil
DELETE FROM \"Cards\" 
WHERE \"AccountId\" = @accountId;

-- 13. Süresi dolmuş tüm kartları sil (hard delete)
DELETE FROM \"Cards\" 
WHERE \"ExpirationDate\" < datetime('now');

-- ═══════════════════════════════════════════════════════════════
-- ANALYTIC Queries
-- ═══════════════════════════════════════════════════════════════

-- 14. Account başına kart sayısı
SELECT 
    a.\"Id\",
    a.\"Name\",
    COUNT(c.\"Id\") AS \"CardCount\"
FROM \"Accounts\" a
LEFT JOIN \"Cards\" c ON a.\"Id\" = c.\"AccountId\"
GROUP BY a.\"Id\", a.\"Name\"
ORDER BY \"CardCount\" DESC;

-- 15. Status dağılımı
SELECT 
    \"Status\",
    COUNT(*) AS \"Count\"
FROM \"Cards\"
GROUP BY \"Status\"
ORDER BY \"Status\";

-- 16. Müşteri başına toplam kart sayısı
SELECT 
    cu.\"Id\",
    cu.\"Name\",
    COUNT(c.\"Id\") AS \"TotalCards\"
FROM \"Customers\" cu
INNER JOIN \"Accounts\" a ON cu.\"Id\" = a.\"CustomerId\"
LEFT JOIN \"Cards\" c ON a.\"Id\" = c.\"AccountId\"
GROUP BY cu.\"Id\", cu.\"Name\"
ORDER BY \"TotalCards\" DESC;

-- 17. Son 7 gün içinde oluşturulan kartlar - sayı ve tarih
SELECT 
    DATE(\"CreatedAt\") AS \"Date\",
    COUNT(*) AS \"CardCount\"
FROM \"Cards\"
WHERE \"CreatedAt\" >= datetime('now', '-7 days')
GROUP BY DATE(\"CreatedAt\")
ORDER BY \"Date\" DESC;
```

---

## 6. Data Model - JSON Examples

### Card Entity (Database Level)
```json
{
  "Id": "550e8400-e29b-41d4-a716-446655440000",
  "CardNumber": "1234567890123456",
  "CardName": "My Visa Card",
  "ExpirationDate": "2027-12-31T00:00:00Z",
  "CVC": "encrypted_cvc_hash_here",
  "AccountId": "6f36cf61-927b-429d-8ac1-d13e8c6d42f2",
  "Status": 2,
  "CreatedAt": "2026-04-11T10:30:00Z",
  "UpdatedAt": "2026-04-11T15:45:00Z"
}
```

### CardDto (API Response Level)
```json
{
  "Id": "550e8400-e29b-41d4-a716-446655440000",
  "CardNumber": "1234 **** **** 3456",
  "CardName": "My Visa Card",
  "ExpirationDate": "2027-12-31T00:00:00Z",
  "AccountId": "6f36cf61-927b-429d-8ac1-d13e8c6d42f2",
  "Status": "Active",
  "CreatedAt": "2026-04-11T10:30:00Z",
  "UpdatedAt": "2026-04-11T15:45:00Z"
}
```

### CreateCardRequest (API Request Level)
```json
{
  "cardNumber": "1234567890123456",
  "cardName": "My Visa Card",
  "expirationDate": "2027-12-31T00:00:00Z",
  "cvc": "123",
  "accountId": "6f36cf61-927b-429d-8ac1-d13e8c6d42f2"
}
```

---

## 7. Constraints & Validations

```csharp
// Database Level Constraints
┌─────────────────────────────────────────┐
│          Column Constraints             │
├──────────────────┬──────────────────────┤
│ Column           │ Constraint           │
├──────────────────┼──────────────────────┤
│ Id               │ PK, NOT NULL, UNIQUE │
│ CardNumber       │ NOT NULL, UNIQUE     │
│ CardName         │ NOT NULL, MaxLen:100 │
│ ExpirationDate   │ NOT NULL             │
│ CVC              │ NOT NULL, Encrypted  │
│ AccountId        │ NOT NULL, FK         │
│ Status           │ NOT NULL, Default: 1 │
│ CreatedAt        │ NOT NULL             │
│ UpdatedAt        │ NULLABLE             │
└──────────────────┴──────────────────────┘

// Application Level Validations
┌─────────────────────────────────────────┐
│       Input Validations                 │
├──────────────────┬──────────────────────┤
│ Field            │ Validation           │
├──────────────────┼──────────────────────┤
│ CardNumber       │ Format, Luhn algo    │
│                  │ Unique in DB         │
│                  │ Max 20 chars         │
│ CardName         │ Required             │
│                  │ Max 100 chars        │
│ ExpirationDate   │ Future date only     │
│                  │ Valid format         │
│ CVC              │ 3-4 digits           │
│                  │ Numeric only         │
│ AccountId        │ Must exist in DB     │
│                  │ Valid GUID format    │
└──────────────────┴──────────────────────┘
```

---

## 8. Migration Commands

```bash
# ═══════════════════════════════════════════════════════════════
# EF Core Migration Commands
# ═══════════════════════════════════════════════════════════════

# 1. Migration oluştur
dotnet ef migrations add AddCardFeatures \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 2. Migration güncelleme (code-first)
dotnet ef migrations add UpdateCardConfiguration \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 3. Veritabanına uygula
dotnet ef database update \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 4. Belirli bir migration'a geri dön
dotnet ef database update {PreviousMigrationName} \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 5. Son migration'ı geri al
dotnet ef migrations remove \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 6. Migration'lar listele
dotnet ef migrations list \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api

# 7. SQL script oluştur (apply etmeden)
dotnet ef migrations script \
  --project payment-system.Infrastructure \
  --startup-project payment-system.Api \
  --output migrations.sql
```

---

## 9. Performance Considerations

```sql
-- Index Optimization

-- ✅ Good: Frequently queried columns
CREATE INDEX \"IX_Cards_AccountId\" ON \"Cards\"(\"AccountId\");
CREATE UNIQUE INDEX \"IX_Cards_CardNumber\" ON \"Cards\"(\"CardNumber\");
CREATE INDEX \"IX_Cards_Status\" ON \"Cards\"(\"Status\");

-- ✅ Good: Composite index for common queries
CREATE INDEX \"IX_Cards_AccountId_Status\" 
ON \"Cards\"(\"AccountId\", \"Status\");

-- Execution Plans
-- 1. GetAllByAccountId - uses IX_Cards_AccountId
-- 2. GetByCardNumber - uses IX_Cards_CardNumber (UNIQUE)
-- 3. GetByStatus - uses IX_Cards_Status
```

---

## 10. Backup & Recovery

```sql
-- Backup Card Data
PRAGMA database_list;
-- Tüm Cards tablosunu export et

-- Recovery
-- SQLite backup dosyasından restore et

-- Test Veri Cleanup
DELETE FROM \"Cards\" WHERE \"CreatedAt\" < datetime('now', '-90 days');
```

---

**Created:** 11 Nisan 2026
**Last Updated:** 11 Nisan 2026
**Status:** Ready for Database Initialization ✅
