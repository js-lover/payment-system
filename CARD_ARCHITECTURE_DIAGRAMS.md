# 📐 Kart Yönetimi - Mimari Diyagramlar

## 1. Genel Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                    CLIENT (REST API)                         │
│              (Postman, Mobile App, Web, vb)                  │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│              API LAYER (payment-system.Api)                  │
├─────────────────────────────────────────────────────────────┤
│  CardController                                             │
│  ├── GET    /api/card                → GetAllCardsAsync     │
│  ├── GET    /api/card/{id}           → GetCardByIdAsync     │
│  ├── GET    /api/card/account/{aid}  → GetCardsByAccountId  │
│  ├── POST   /api/card                → CreateCardAsync      │
│  ├── PUT    /api/card/{id}           → UpdateCardAsync      │
│  └── DELETE /api/card/{id}           → DeleteCardAsync      │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│        APPLICATION LAYER (payment-system.Application)        │
├─────────────────────────────────────────────────────────────┤
│  ICardService                                               │
│  └── CardService Implementation                             │
│      ├── Validation                                         │
│      ├── Business Logic                                     │
│      ├── Encryption/Decryption (CVC)                        │
│      ├── Masking (Card Number)                              │
│      └── AutoMapper (DTO Conversion)                        │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│       APPLICATION LAYER (Repositories)                       │
├─────────────────────────────────────────────────────────────┤
│  ICardRepository                                            │
│  ├── GetByIdAsync()                                         │
│  ├── GetAllAsync()                                          │
│  ├── GetAllByAccountIdAsync()                               │
│  ├── CreateAsync()                                          │
│  ├── UpdateAsync()                                          │
│  └── DeleteAsync()                                          │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│     INFRASTRUCTURE LAYER (payment-system.Infrastructure)     │
├─────────────────────────────────────────────────────────────┤
│  CardRepository Implementation                              │
│  └── Integrates with DbContext                              │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ▼
┌─────────────────────────────────────────────────────────────┐
│              PERSISTENCE LAYER                               │
├─────────────────────────────────────────────────────────────┤
│  DbContext                                                  │
│  └── Entity Framework Core                                  │
│      └── SQLite Database (PaymentSystem.db)                 │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│              DOMAIN LAYER (payment-system.Domain)            │
├─────────────────────────────────────────────────────────────┤
│  Card Entity                                                │
│  ├── Id                                                     │
│  ├── CardNumber                                             │
│  ├── CardName                                               │
│  ├── ExpirationDate                                         │
│  ├── CVC (Encrypted)                                        │
│  ├── AccountId (Foreign Key)                                │
│  └── Status (CardStatus Enum)                               │
└─────────────────────────────────────────────────────────────┘
```

---

## 2. Entity Relations Diagram

```
┌─────────────────────┐
│     Customer        │
├─────────────────────┤
│ • Id (PK)          │
│ • Name             │
│ • Email            │
└──────────┬──────────┘
           │ 1
           │
           │ * (Birden çok)
           │
┌──────────▼──────────┐
│      Account        │
├─────────────────────┤
│ • Id (PK)          │
│ • Name             │
│ • Balance          │
│ • CustomerId (FK)  │
└──────────┬──────────┘
           │ 1
           │
           │ * (Birden çok)
           │
┌──────────▼──────────┐
│       Card          │◄─── NEW
├─────────────────────┤
│ • Id (PK)          │
│ • CardNumber ✓     │
│ • CardName         │
│ • ExpirationDate   │
│ • CVC (Encrypted)  │
│ • AccountId (FK)   │
│ • Status           │
└──────────┬──────────┘
           │ 1
           │
           │ * (Birden çok)
           │
┌──────────▼──────────┐
│    Transaction      │
├─────────────────────┤
│ • Id (PK)          │
│ • Amount           │
│ • Type             │
│ • CardId (FK)      │
│ • Status           │
└─────────────────────┘
```

---

## 3. Data Flow - Create Card

```
┌──────────────┐
│ Client       │
│ POST /card   │
└──────┬───────┘
       │
       │ CreateCardRequest
       │ {
       │   "cardNumber": "1234567890123456",
       │   "cardName": "My Visa",
       │   "expirationDate": "2026-12-31",
       │   "cvc": "123",
       │   "accountId": "guid"
       │ }
       │
       ▼
┌──────────────────────────┐
│   CardController         │
│   CreateCardAsync()      │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardService            │
│   CreateCardAsync()      │
├──────────────────────────┤
│ 1. ValidateInput()       │
│    • Check dates         │
│    • Check formats       │
│                          │
│ 2. CheckAccountExists()  │
│    • Query database      │
│                          │
│ 3. CheckUniqueness()     │
│    • Card number unique? │
│                          │
│ 4. EncryptCVC()          │
│    • Security measure    │
│                          │
│ 5. Repository.Create()   │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardRepository         │
│   CreateAsync()          │
├──────────────────────────┤
│ 1. Create entity         │
│ 2. DbContext.Add()       │
│ 3. SaveChangesAsync()    │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   SQLite Database        │
│   INSERT INTO Cards...   │
└──────┬───────────────────┘
       │
       ▼ (Success)
┌──────────────────────────┐
│   CardService            │
│   MapToDto()             │
├──────────────────────────┤
│ • Mask card number       │
│ • Hide CVC               │
│ • Create response        │
└──────┬───────────────────┘
       │
       ▼ Result<CardDto>
┌──────────────────────────┐
│   CardController         │
│   Return 201 Created     │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   Client Response        │
│   {                      │
│     "id": "guid",        │
│     "cardNumber":        │
│       "1234 **** ****    │
│        3456",            │
│     "cardName": "My V",  │
│     "status": "Active"   │
│   }                      │
└──────────────────────────┘
```

---

## 4. Data Flow - Get Cards by Account

```
┌──────────────────────────┐
│ Client                   │
│ GET /api/card/account/   │
│     {accountId}          │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardController         │
│ GetCardsByAccountId()    │
├──────────────────────────┤
│ • Extract accountId      │
│ • Check authorization    │
│ • Call service           │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardService            │
│ GetCardsByAccountId()    │
├──────────────────────────┤
│ • Validate account id    │
│ • Call repository        │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardRepository         │
│ GetAllByAccountId()      │
├──────────────────────────┤
│ WHERE AccountId = @id    │
│ .ToListAsync()           │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   SQLite Database        │
│   SELECT * FROM Cards    │
│   WHERE AccountId = ?    │
└──────┬───────────────────┘
       │
       ▼ [Card1, Card2, Card3]
┌──────────────────────────┐
│   CardService            │
│   MapToDtoList()         │
├──────────────────────────┤
│ For each card:           │
│ • Mask card number       │
│ • Remove CVC             │
│ • Create CardDto         │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardController         │
│   Return 200 OK          │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   Client Response        │
│   [                      │
│     {                    │
│       "id": "...",       │
│       "cardNumber":      │
│         "1234 ****",     │
│       "cardName": "V1"   │
│     },                   │
│     {                    │
│       "id": "...",       │
│       "cardNumber":      │
│         "5678 ****",     │
│       "cardName": "MC"   │
│     }                    │
│   ]                      │
└──────────────────────────┘
```

---

## 5. Data Flow - Delete Card

```
┌──────────────────────────┐
│ Client                   │
│ DELETE /api/card/{id}    │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardController         │
│   DeleteCardAsync()      │
├──────────────────────────┤
│ • Extract card id        │
│ • Check authorization    │
│ • Verify ownership       │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardService            │
│   DeleteCardAsync()      │
├──────────────────────────┤
│ • Check card exists      │
│ • Check transactions     │
│ • (Optional) Audit log   │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   CardRepository         │
│   DeleteAsync()          │
├──────────────────────────┤
│ 1. var card =            │
│    GetByIdAsync(id)      │
│ 2. DbContext.Remove()    │
│ 3. SaveChangesAsync()    │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   SQLite Database        │
│   DELETE FROM Cards      │
│   WHERE Id = ?           │
└──────┬───────────────────┘
       │
       ▼ (Success)
┌──────────────────────────┐
│   CardController         │
│   Return 204 NoContent   │
│   (or 200 with message)  │
└──────┬───────────────────┘
       │
       ▼
┌──────────────────────────┐
│   Client Response        │
│   204 No Content         │
└──────────────────────────┘
```

---

## 6. Class Diagram

```
┌─────────────────────────────┐
│       ICardRepository       │◄── Interface
├─────────────────────────────┤
│ + GetByIdAsync()            │
│ + GetAllAsync()             │
│ + GetAllByAccountIdAsync()  │
│ + CreateAsync()             │
│ + UpdateAsync()             │
│ + DeleteAsync()             │
└────────────▲────────────────┘
             │
             │ Implements
             │
┌────────────┴────────────────┐
│    CardRepository           │ ◄── Implementation (Infrastructure)
├─────────────────────────────┤
│ - _context: DbContext       │
│ - _logger: ILogger          │
├─────────────────────────────┤
│ + GetByIdAsync()            │
│ + GetAllAsync()             │
│ + GetAllByAccountIdAsync()  │
│ + CreateAsync()             │
│ + UpdateAsync()             │
│ + DeleteAsync()             │
└─────────────────────────────┘


┌─────────────────────────────┐
│      ICardService           │◄── Interface
├─────────────────────────────┤
│ + GetCardByIdAsync()        │
│ + GetAllCardsAsync()        │
│ + GetCardsByAccountIdAsync()│
│ + CreateCardAsync()         │
│ + UpdateCardAsync()         │
│ + DeleteCardAsync()         │
└────────────▲────────────────┘
             │
             │ Implements
             │
┌────────────┴────────────────┐
│     CardService             │ ◄── Implementation (Application)
├─────────────────────────────┤
│ - _repository: ICardRepo    │
│ - _accRepo: IAccountRepo    │
│ - _mapper: IMapper          │
│ - _logger: ILogger          │
├─────────────────────────────┤
│ + GetCardByIdAsync()        │
│ + GetAllCardsAsync()        │
│ + GetCardsByAccountIdAsync()│
│ + CreateCardAsync()         │
│ + UpdateCardAsync()         │
│ + DeleteCardAsync()         │
│ - MaskCardNumber()          │
│ - ValidateCard()            │
│ - EncryptCVC()              │
└─────────────────────────────┘


┌─────────────────────────────┐
│    CardController           │ ◄── API Layer
├─────────────────────────────┤
│ - _service: ICardService    │
│ - _logger: ILogger          │
├─────────────────────────────┤
│ + GetAllCards()             │
│ + GetCardById()             │
│ + GetCardsByAccountId()     │
│ + CreateCard()              │
│ + UpdateCard()              │
│ + DeleteCard()              │
└─────────────────────────────┘


┌─────────────────────────────┐
│      Card Entity            │ ◄── Domain Layer
├─────────────────────────────┤
│ + Id: Guid                  │
│ + CardNumber: string        │
│ + CardName: string          │
│ + ExpirationDate: DateTime  │
│ + CVC: string (Encrypted)   │
│ + AccountId: Guid (FK)      │
│ + Status: CardStatus        │
│ + Account: Account (Nav)    │
│ + Transactions: List        │
└─────────────────────────────┘
```

---

## 7. API Endpoints Quick Reference

```
┌─────────────────────────────────────────────────────────────┐
│                     CARD API ENDPOINTS                       │
├─────────┬─────────────────────┬──────────────────────────────┤
│ METHOD  │ ENDPOINT            │ DESCRIPTION                  │
├─────────┼─────────────────────┼──────────────────────────────┤
│ GET     │ /api/card           │ Get all cards                │
│         │                     │ Response: 200, 500           │
│         │                     │ Return: List<CardDto>        │
├─────────┼─────────────────────┼──────────────────────────────┤
│ GET     │ /api/card/{id}      │ Get single card by ID        │
│         │                     │ Response: 200, 404, 500      │
│         │                     │ Return: CardDto              │
├─────────┼─────────────────────┼──────────────────────────────┤
│ GET     │ /api/card/          │ Get account's cards          │
│         │ account/{accountId} │ Response: 200, 401, 404, 500 │
│         │                     │ Return: List<CardDto>        │
├─────────┼─────────────────────┼──────────────────────────────┤
│ POST    │ /api/card           │ Create new card              │
│         │                     │ Request: CreateCardRequest   │
│         │                     │ Response: 201, 400, 401, 500 │
│         │                     │ Return: CardDto              │
├─────────┼─────────────────────┼──────────────────────────────┤
│ PUT     │ /api/card/{id}      │ Update card                  │
│         │                     │ Request: UpdateCardRequest   │
│         │                     │ Response: 200, 400, 401, 404 │
│         │                     │ Return: CardDto              │
├─────────┼─────────────────────┼──────────────────────────────┤
│ DELETE  │ /api/card/{id}      │ Delete card                  │
│         │                     │ Response: 204, 401, 404, 500 │
│         │                     │ Return: (empty)              │
└─────────┴─────────────────────┴──────────────────────────────┘
```

---

## 8. Security & Validation Flow

```
┌─────────────────────────────────────────────────────────────┐
│              INPUT VALIDATION CHAIN                          │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│ CreateCardRequest                                           │
│   ↓                                                          │
│ [1] Input Validation                                        │
│   ├─ CardNumber: Required, Valid Format (Luhn)             │
│   ├─ CardName: Required, Max 100 chars                      │
│   ├─ ExpirationDate: Required, Future date                  │
│   └─ CVC: Required, 3-4 digits                              │
│   ↓                                                          │
│ [2] Business Logic Validation                               │
│   ├─ Account exists?                                        │
│   ├─ Card number unique?                                    │
│   ├─ Account active?                                        │
│   └─ ExpirationDate > DateTime.Now                          │
│   ↓                                                          │
│ [3] Security Processing                                     │
│   ├─ Encrypt CVC → encrypted_cvc                            │
│   ├─ Mask card number → MaskCardNumber()                    │
│   └─ Sanitize inputs                                        │
│   ↓                                                          │
│ [4] Authorization Check                                     │
│   ├─ User authenticated?                                    │
│   ├─ User owns this account?                                │
│   └─ Has permission?                                        │
│   ↓                                                          │
│ [5] Database Operation                                      │
│   └─ CREATE/UPDATE/DELETE                                   │
│   ↓                                                          │
│ [6] Response Mapping                                        │
│   ├─ Convert to CardDto                                     │
│   ├─ Apply masking                                          │
│   └─ Hide sensitive data                                    │
│   ↓                                                          │
│ [✓] Success Response                                        │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

---

## 9. Technology Stack

```
┌──────────────────────────────────────────────────────────────┐
│              TECHNOLOGY STACK                                │
├────────────────────┬─────────────────────────────────────────┤
│ Layer              │ Technology                              │
├────────────────────┼─────────────────────────────────────────┤
│ Framework          │ .NET 10.0 (C#)                          │
│ API                │ ASP.NET Core REST API                   │
│ Database           │ SQLite (PaymentSystem.db)               │
│ ORM                │ Entity Framework Core 10                │
│ Mapping            │ AutoMapper                              │
│ Logging            │ Microsoft.Extensions.Logging            │
│ DI Container       │ Built-in Dependency Injection           │
│ Validation         │ Data Annotations / FluentValidation     │
│ Security           │ JWT / Built-in Authorization            │
│ Testing            │ xUnit / NUnit                           │
│ API Documentation  │ Swagger/OpenAPI                         │
└────────────────────┴─────────────────────────────────────────┘
```

---

**Created:** 11 Nisan 2026
**Status:** Ready for Review ✅
