# Kart Yönetimi Özellikleri - Uygulama Planı

## 📋 Genel Açıklama
Kullanıcının kartlarını görüntüleyebilmesi, yeni kart oluşturabilmesi ve silebilmesi için gerekli özelliklerin projeye eklenmesi.

**Tarih:** 11 Nisan 2026
**Proje:** Payment System

---

## 🎯 Hedefler

1. ✅ Kartları listeleme (Get All Cards)
2. ✅ Belirli bir kartı görüntüleme (Get Card by ID)
3. ✅ Yeni kart oluşturma (Create Card)
4. ✅ Kartı silme (Delete Card)
5. ✅ Hesaba ait tüm kartları listeleme

---

## 🏗️ Katmanlı Mimariye Göre Yapılacak İşler

### **1. Domain Layer** (`payment-system.Domain`)
Mevcut yapı:
- ✅ `Card` Entity
- ✅ `CardStatus` Enum
- ✅ `BaseEntity` (Id, CreatedAt, UpdatedAt vb.)

**Yapılacak:**
- ✅ Zaten tamamlanmış - yeni değişiklik gerekmez

---

### **2. Application Layer** (`payment-system.Application`)

#### **A. Repository Interface** 
**Dosya:** `Repositories/ICardRepository.cs` (YENİ)

```csharp
namespace payment_system.Application.Repositories
{
    public interface ICardRepository
    {
        // READ Operations
        Task<Card?> GetByIdAsync(Guid cardId);
        Task<IEnumerable<Card>> GetAllAsync();
        Task<IEnumerable<Card>> GetAllByAccountIdAsync(Guid accountId);
        Task<Card?> GetByCardNumberAsync(string cardNumber);
        
        // WRITE Operations
        Task<Card> CreateAsync(Card card);
        Task<Card> UpdateAsync(Card card);
        
        // DELETE Operations
        Task<bool> DeleteAsync(Guid cardId);
        
        // CHECK Operations
        Task<bool> ExistsAsync(Guid cardId);
        Task<bool> CardNumberExistsAsync(string cardNumber);
    }
}
```

#### **B. DTOs (Data Transfer Objects)**
**Dosya:** `DTOs/Card/CardDto.cs` (YENİ)
```csharp
public class CardDto
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; } // Masked: "1234 **** **** 6789"
    public string CardName { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid AccountId { get; set; }
    public CardStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**Dosya:** `DTOs/Card/CreateCardRequest.cs` (YENİ)
```csharp
public class CreateCardRequest
{
    public string CardNumber { get; set; } = null!;
    public string CardName { get; set; } = null!;
    public DateTime ExpirationDate { get; set; }
    public string CVC { get; set; } = null!;
    public Guid AccountId { get; set; }
}
```

**Dosya:** `DTOs/Card/UpdateCardRequest.cs` (YENİ)
```csharp
public class UpdateCardRequest
{
    public string CardName { get; set; } = null!;
    public CardStatus Status { get; set; }
}
```

#### **C. Service Interface**
**Dosya:** `Services/Interfaces/ICardService.cs` (YENİ)

```csharp
namespace payment_system.Application.Services.Interfaces
{
    public interface ICardService
    {
        // READ Operations
        Task<Result<CardDto>> GetCardByIdAsync(Guid cardId);
        Task<Result<IEnumerable<CardDto>>> GetAllCardsAsync();
        Task<Result<IEnumerable<CardDto>>> GetCardsByAccountIdAsync(Guid accountId);
        
        // WRITE Operations
        Task<Result<CardDto>> CreateCardAsync(CreateCardRequest request);
        Task<Result<CardDto>> UpdateCardAsync(Guid cardId, UpdateCardRequest request);
        
        // DELETE Operations
        Task<Result<bool>> DeleteCardAsync(Guid cardId);
    }
}
```

#### **D. Service Implementation**
**Dosya:** `Services/Implementations/CardService.cs` (YENİ)

```csharp
namespace payment_system.Application.Services.Implementations
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CardService> _logger;

        public CardService(
            ICardRepository cardRepository,
            IAccountRepository accountRepository,
            IMapper mapper,
            ILogger<CardService> logger)
        {
            _cardRepository = cardRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // Implementation details...
    }
}
```

---

### **3. Infrastructure Layer** (`payment-system.Infrastructure`)

#### **A. Repository Implementation**
**Dosya:** `Repositories/CardRepository.cs` (YENİ)

```csharp
namespace payment_system.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly PaymentSystemDbContext _context;
        private readonly ILogger<CardRepository> _logger;

        public CardRepository(PaymentSystemDbContext context, ILogger<CardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Implementation details...
    }
}
```

#### **B. DbContext Configuration (Eğer gerekirse)**
**Dosya:** `Persistence/Configurations/CardConfiguration.cs` (YENİ)

```csharp
namespace payment_system.Infrastructure.Persistence.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            // Fluent API konfigürasyonu
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CardNumber).IsRequired().HasMaxLength(20);
            builder.Property(c => c.CardName).IsRequired().HasMaxLength(100);
            builder.HasIndex(c => c.CardNumber).IsUnique();
            // vb...
        }
    }
}
```

---

### **4. API Layer** (`payment-system.Api`)

#### **A. Controller**
**Dosya:** `Controllers/Card/CardController.cs` (YENİ)

```csharp
namespace payment_system.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ILogger<CardController> _logger;

        public CardController(ICardService cardService, ILogger<CardController> logger)
        {
            _cardService = cardService;
            _logger = logger;
        }

        // Endpoints...
    }
}
```

**Endpoints:**
- `GET /api/card` - Tüm kartları listele
- `GET /api/card/{id}` - Belirli kartı getir
- `GET /api/card/account/{accountId}` - Hesaba ait kartları listele
- `POST /api/card` - Yeni kart oluştur
- `PUT /api/card/{id}` - Kartı güncelle
- `DELETE /api/card/{id}` - Kartı sil

---

## 📊 Dosya Ağacı (Yapılacak Yeni Dosyalar)

```
payment-system.Application/
├── DTOs/
│   └── Card/
│       ├── CardDto.cs
│       ├── CreateCardRequest.cs
│       └── UpdateCardRequest.cs
├── Repositories/
│   └── ICardRepository.cs
└── Services/
    ├── Interfaces/
    │   └── ICardService.cs
    └── Implementations/
        └── CardService.cs

payment-system.Infrastructure/
├── Persistence/
│   └── Configurations/
│       └── CardConfiguration.cs
└── Repositories/
    └── CardRepository.cs

payment-system.Api/
└── Controllers/
    └── Card/
        ├── CardController.cs
        └── CardController.Command.cs (CQRS pattern için)
```

---

## 🔐 Güvenlik Konuları

1. **CVC Şifreleme**: CVC hiçbir zaman düz metin olarak DB'ye kaydedilmemeli
   - Encryption service kullanılmalı
   - Request'te alınmalı ancak Response'da gönderilmemeli

2. **Kart Numarası Maskeleme**: 
   - Response'da sadece "1234 **** **** 6789" formatında gösterilmeli
   - Full kart numarası sadece iç işlemlerde kullanılmalı

3. **Yetkilendirme**:
   - Kullanıcı sadece kendi hesabının kartlarını görebilmeli
   - Kart silme/güncelleme için owner olması gerekli

4. **Validation**:
   - Kart numarası formatı kontrol edilmeli (Luhn algoritması)
   - Expiration date geçmiş olmamış olmalı
   - CVC format kontrolü

---

## 🧪 Test Stratejisi

### Unit Tests (`payment-system.Tests`)
1. CardService tests
2. CardRepository tests
3. Validation tests

### Integration Tests
1. End-to-end card CRUD operasyonları
2. Account-Card ilişkisi testleri
3. Authorization testleri

---

## 📝 Uygulama Adımları

### **Faz 1: Foundation (1. Adım)**
- [ ] `ICardRepository` arayüzü oluştur
- [ ] `CardRepository` implementasyonu yap
- [ ] `CardConfiguration` (Fluent API) konfigürasyonu

### **Faz 2: Application Layer (2. Adım)**
- [ ] DTOs oluştur (`CardDto`, `CreateCardRequest`, `UpdateCardRequest`)
- [ ] `ICardService` arayüzü oluştur
- [ ] `CardService` implementasyonu yap
- [ ] Dependency Injection'a ekle (`ServiceCollectionExtension.cs`)

### **Faz 3: API Layer (3. Adım)**
- [ ] `CardController` oluştur
- [ ] REST endpoints'i implement et
- [ ] OpenAPI/Swagger dokumentasyonu ekle

### **Faz 4: Testing (4. Adım)**
- [ ] Unit tests yaz
- [ ] Integration tests yaz
- [ ] Manual testing yap

### **Faz 5: Refinement (5. Adım)**
- [ ] Error handling
- [ ] Logging
- [ ] Performance optimization
- [ ] Security review

---

## 🛠️ Teknoloji Stack

- **Framework:** .NET 10.0
- **Database:** SQLite (PaymentSystem.db)
- **ORM:** Entity Framework Core
- **Mapping:** AutoMapper
- **Validation:** FluentValidation (opsiyonel)
- **Logging:** Built-in Microsoft.Extensions.Logging

---

## 📚 İlgili Referanslar

- Proje Structure: Layered Architecture (Clean Architecture)
- Pattern: Repository Pattern + Service Pattern
- Design: CQRS (Command Query Responsibility Segregation) - optional
- Error Handling: Result<T> Generic Pattern

---

## ✅ Başarı Kriterleri

1. ✅ Kullanıcı kendi hesabının tüm kartlarını görebilmeli
2. ✅ Yeni kart ekleyebilmeli (geçerli validasyon ile)
3. ✅ Kartını silebilmeli
4. ✅ CVC ve diğer hassas bilgiler korunmalı
5. ✅ Kart numarası maskelenmiş olmalı
6. ✅ Authorization kontrolü olmalı
7. ✅ Hata yönetimi uygun olmalı
8. ✅ Testler yazılmış olmalı

---

## 📞 Notlar

- CardStatus enum'u zaten tanımlanmış: `None, WaitingForApproval, Active, Blocked, Expired`
- Card entity'si Account'a foreign key ile bağlı
- Transaction entity'si Card'a bağlı
- Existing patterns (Customer, Account, Transaction) takip edilmeli
