# 🎯 Kart Yönetimi - Teknik İş Listesi (To-Do List)

## Phase 1: Infrastructure & Repository Layer
### Priority: HIGH | Başlangıç Noktası

- [ ] **ICardRepository Interface Oluştur**
  - Dosya: `payment-system.Application/Repositories/ICardRepository.cs`
  - CRUD metodları
  - Query metodları
  
- [ ] **CardRepository Implementation Oluştur**
  - Dosya: `payment-system.Infrastructure/Repositories/CardRepository.cs`
  - DbContext kullan
  - Logging ekle
  
- [ ] **CardConfiguration (Fluent API) Oluştur**
  - Dosya: `payment-system.Infrastructure/Persistence/Configurations/CardConfiguration.cs`
  - Entity mapping
  - Unique constraint (CardNumber)
  - Foreign key relation

---

## Phase 2: Application Layer - DTOs & Service Interface
### Priority: HIGH | Foundation

- [ ] **DTO Sınıfları Oluştur**
  - [ ] `CardDto.cs`
    - Masked cardNumber
    - Public properties
    
  - [ ] `CreateCardRequest.cs`
    - Full cardNumber (request'te)
    - CardName
    - ExpirationDate
    - CVC
    - AccountId
    
  - [ ] `UpdateCardRequest.cs`
    - CardName
    - Status
    
  - [ ] `CardListDto.cs` (optional)
    - Lightweight DTO for list responses

- [ ] **ICardService Interface Oluştur**
  - Dosya: `payment-system.Application/Services/Interfaces/ICardService.cs`
  - GetCardByIdAsync()
  - GetAllCardsAsync()
  - GetCardsByAccountIdAsync()
  - CreateCardAsync()
  - UpdateCardAsync()
  - DeleteCardAsync()

---

## Phase 3: Service Implementation
### Priority: HIGH | Core Business Logic

- [ ] **CardService Implementation Oluştur**
  - Dosya: `payment-system.Application/Services/Implementations/CardService.cs`
  
  - [ ] **Create Method**
    - Account existence check
    - CardNumber uniqueness validation
    - Expiration date validation
    - CVC encryption
    - CardNumber masking
    
  - [ ] **Read Methods**
    - GetCardByIdAsync() - include relations
    - GetAllCardsAsync()
    - GetCardsByAccountIdAsync()
    - Masking in DTO mapping
    
  - [ ] **Update Method**
    - Existence check
    - Status update logic
    
  - [ ] **Delete Method**
    - Soft delete or hard delete
    - Check if card has transactions
    
  - [ ] **Helper Methods**
    - MaskCardNumber(string fullNumber)
    - ValidateCardNumber(string cardNumber)
    - EncryptCVC(string cvc)
    - DecryptCVC(string encryptedCvc)

- [ ] **AutoMapper Profiles Oluştur**
  - Card Entity ↔ CardDto mapping
  - Card Entity ↔ CardListDto mapping
  - Masking applied in mapping

---

## Phase 4: Dependency Injection
### Priority: MEDIUM | Integration

- [ ] **ServiceCollectionExtension'a Ekle**
  - Dosya: `payment-system.Api/Extensions/ServiceCollectionExtension.cs`
  - `services.AddScoped<ICardRepository, CardRepository>()`
  - `services.AddScoped<ICardService, CardService>()`
  - AutoMapper profile registration

- [ ] **DbContext Configuration**
  - Eğer CardConfiguration otomatik yüklenmediyse ekle

---

## Phase 5: API Controllers
### Priority: HIGH | User Facing

- [ ] **CardController Oluştur**
  - Dosya: `payment-system.Api/Controllers/Card/CardController.cs`
  
  - [ ] **[GET] /api/card**
    - Tüm kartları listele
    - Pagination (optional)
    - Filtering (optional)
    - Status code: 200, 500
    
  - [ ] **[GET] /api/card/{id}**
    - Single card retrieve
    - Status code: 200, 404, 500
    
  - [ ] **[GET] /api/card/account/{accountId}**
    - Account'a ait kartları listele
    - Status code: 200, 404, 500
    
  - [ ] **[POST] /api/card**
    - Yeni kart oluştur
    - Validation
    - Status code: 201, 400, 401, 404, 500
    
  - [ ] **[PUT] /api/card/{id}**
    - Kartı güncelle
    - Validation
    - Status code: 200, 400, 401, 404, 500
    
  - [ ] **[DELETE] /api/card/{id}**
    - Kartı sil
    - Check permissions
    - Status code: 204, 401, 404, 500

- [ ] **Authorization**
  - [Authorize] attribute ekle
  - User ownership validation
  - Role-based access control (if needed)

---

## Phase 6: Validation & Security
### Priority: HIGH | Non-Functional Requirements

- [ ] **Input Validation**
  - CardNumber format validation
  - ExpirationDate > today
  - CVC format (3-4 digits)
  - CardName not empty
  
- [ ] **Business Logic Validation**
  - AccountId must exist
  - CardNumber must be unique
  - Cannot delete card with pending transactions (optional)
  - Account must be active (optional)

- [ ] **Security Measures**
  - CVC encryption/hashing
  - CardNumber masking
  - Authorization checks
  - Audit logging

- [ ] **FluentValidation Setup (Optional)**
  - `CreateCardRequestValidator`
  - `UpdateCardRequestValidator`

---

## Phase 7: Testing
### Priority: MEDIUM | Quality Assurance

- [ ] **Unit Tests**
  - Dosya: `payment-system.Tests/CardServiceTests.cs`
  - [ ] CreateCardAsync tests
    - Successful creation
    - Duplicate card number
    - Invalid account
    - Invalid expiration date
    
  - [ ] GetCardByIdAsync tests
    - Existing card
    - Non-existent card
    
  - [ ] DeleteCardAsync tests
    - Successful deletion
    - Non-existent card
    - Authorization failure

- [ ] **Integration Tests**
  - End-to-end CRUD operations
  - Database persistence
  - Repository integration

- [ ] **Manual Testing**
  - Postman/Thunder Client requests
  - Edge cases

---

## Phase 8: Documentation & Refinement
### Priority: LOW | Polish

- [ ] **OpenAPI/Swagger Documentation**
  - Endpoint descriptions
  - Request/Response examples
  - Error codes
  
- [ ] **Code Documentation**
  - XML comments on public methods
  - Complex logic explanation
  
- [ ] **Error Handling**
  - Consistent error responses
  - Meaningful error messages
  
- [ ] **Logging**
  - Info level: successful operations
  - Warning level: potential issues
  - Error level: exceptions
  
- [ ] **Performance Optimization**
  - Query optimization
  - Caching (if needed)
  - Pagination implementation

---

## 📊 Dependency Map

```
CardController (API)
    ↓
ICardService (Application)
    ↓
CardService (Application)
    ├─→ ICardRepository (Application)
    │     ↓
    │   CardRepository (Infrastructure)
    │     ↓
    │   DbContext
    │
    └─→ IMapper (AutoMapper)
        ↓
      Card Entity ↔ CardDto
```

---

## 🔄 Data Flow Examples

### Create Card Flow
```
POST /api/card
  └─→ CardController.CreateCardAsync()
      └─→ CardService.CreateCardAsync()
          ├─→ Validate input
          ├─→ Check account exists
          ├─→ Encrypt CVC
          ├─→ Mask card number
          ├─→ CardRepository.CreateAsync()
          └─→ Return CardDto
```

### Get Cards Flow
```
GET /api/card/account/{accountId}
  └─→ CardController.GetCardsByAccountIdAsync()
      └─→ CardService.GetCardsByAccountIdAsync()
          ├─→ CardRepository.GetAllByAccountIdAsync()
          └─→ Map to CardDto[] (with masking)
              └─→ Return List<CardDto>
```

### Delete Card Flow
```
DELETE /api/card/{id}
  └─→ CardController.DeleteCardAsync()
      ├─→ Authorization check
      └─→ CardService.DeleteCardAsync()
          ├─→ Check card exists
          ├─→ Check user permissions
          ├─→ CardRepository.DeleteAsync()
          └─→ Return success
```

---

## 📈 Tahmini İş Yükü

| Phase | Görevler | Saatler | Öncelik |
|-------|---------|---------|---------|
| 1 | Repository Setup | 1-2 | HIGH |
| 2 | DTOs & Interfaces | 1-2 | HIGH |
| 3 | Service Implementation | 3-4 | HIGH |
| 4 | Dependency Injection | 0.5 | MEDIUM |
| 5 | Controllers & Endpoints | 2-3 | HIGH |
| 6 | Validation & Security | 2-3 | HIGH |
| 7 | Testing | 2-3 | MEDIUM |
| 8 | Documentation | 1-2 | LOW |
| **Total** | | **13-19 saat** | |

---

## 🚀 Quick Start Checklist

```bash
# 1. Repository Layer
✓ ICardRepository.cs
✓ CardRepository.cs
✓ CardConfiguration.cs

# 2. Application Layer
✓ CardDto.cs, CreateCardRequest.cs, UpdateCardRequest.cs
✓ ICardService.cs
✓ CardService.cs
✓ AutoMapper profile

# 3. API Layer
✓ CardController.cs
✓ Endpoints (GET, POST, PUT, DELETE)

# 4. Integration
✓ ServiceCollectionExtension.cs update
✓ Test the endpoints

# 5. Polish
✓ Error handling
✓ Logging
✓ Documentation
```

---

## 📞 Notes & Considerations

1. **CVC Handling**: Hiçbir zaman Response'da CVC gösterilmemeli
2. **Card Number Masking**: Database'de full tutulabilir, API response'da masked
3. **Soft vs Hard Delete**: Silinmiş kartlar audit trail için soft delete önerilir
4. **Transactions**: Kart silinirse associated transactions ne olacak? (Policy tanımla)
5. **CardStatus**: Enum'da tanımlı olan statuslar nasıl yönetilecek?
6. **Pagination**: Card listesi büyüyebilirse pagination gerekebilir
7. **Sorting & Filtering**: Future enhancement olarak düşün

---

**Created:** 11 Nisan 2026
**Last Updated:** 11 Nisan 2026
**Status:** READY FOR IMPLEMENTATION ✅
