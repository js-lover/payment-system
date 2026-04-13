# Endpoint Yetkilendirme Sınıflandırması

**Son Güncelleme:** 11 Nisan 2026  
**Durum:** Analiz Tamamlandı

---

## 📋 Özet

Projede toplam **23 endpoint** tanımlanmıştır. Bunların yetkilendirme durumları aşağıdaki gibidir:

| Kategori | Sayı | Yüzde |
|----------|------|-------|
| **Public (AllowAnonymous)** | 2 | 8.7% |
| **Sadece Admin** | 3 | 13% |
| **Sadece Customer** | 0 | 0% |
| **Admin + Customer** | 10 | 43.5% |
| **Sadece Authenticated [Authorize]** | 8 | 34.8% |
| **TOPLAM** | **23** | **100%** |

---

## 🔓 PUBLIC ENDPOINTS (2)
**Kimse tarafından erişilebilir - [AllowAnonymous]**

### Auth Controller
1. **POST** `/api/auth/login`
   - Açıklama: Kullanıcı giriş işlemi
   - Rol: Herkese açık
   - Güvenlik: Email ve şifre doğrulaması gerekli
   - ✅ Doğru

2. **POST** `/api/auth/register`
   - Açıklama: Yeni kullanıcı kaydı (Customer veya Admin)
   - Rol: Herkese açık
   - Güvenlik: Email doğrulaması ve password strength kontrolü gerekli
   - ✅ Doğru

---

## 👤 ADMIN-ONLY ENDPOINTS (3)
**Sadece Admin rolüne sahip kullanıcılar erişebilir - [Authorize(Roles = "Admin")]**

### Admin Controller
1. **GET** `/api/admin`
   - Açıklama: Tüm admin kullanıcıları listele
   - Rol: Admin
   - Güvenlik: ✅ Korunmuş
   - ⚠️ Dikkat: Admin bilgilerinin açığa çıkması risk oluşturabilir

2. **POST** `/api/admin`
   - Açıklama: Yeni admin kullanıcı oluştur
   - Rol: Admin
   - Güvenlik: ✅ Korunmuş
   - ⚠️ Dikkat: Admin oluşturma yetkisinin sınırlanması önerilir

3. **GET** `/api/admin/dashboard`
   - Açıklama: Admin dashboard istatistikleri (henüz implementasyon yapılmadı)
   - Rol: Admin
   - Güvenlik: ✅ Korunmuş
   - ⏳ TODO: Dashboard istatistikleri tamamlanmalı

---

## 👥 ADMIN + CUSTOMER ENDPOINTS (10)
**Hem Admin hem de Customer rolündeki kullanıcılar erişebilir - [Authorize(Roles = "Admin,Customer")]**

### Account Controller
1. **POST** `/api/account`
   - Açıklama: Yeni hesap oluştur
   - Rol: Admin + Customer
   - Güvenlik: ✅ Korunmuş
   - 📝 Not: Kullanıcılar sadece kendi hesaplarını oluşturabilmeli

2. **PUT** `/api/account/{accountId}`
   - Açıklama: Hesap güncelle
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Korunmuş ama Rol kontrolü eksik
   - 📝 Not: Ownership kontrolü gerekli (kendi hesabı mı kontrol edilmeli?)

3. **DELETE** `/api/account/{accountId}`
   - Açıklama: Hesap sil
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Korunmuş ama Rol kontrolü eksik
   - 📝 Not: Ownership kontrolü gerekli

### Card Controller
4. **GET** `/api/card`
   - Açıklama: Tüm kartları listele
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Tüm kartları görebilir (admin kontrolü yok, customers sadece kendi kartlarını görmeli)

5. **GET** `/api/card/{id}`
   - Açıklama: Belirli bir kartı getir
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok (herhangi biri herhangi kartı görebilir)

6. **GET** `/api/card/account/{accountId}`
   - Açıklama: Account'a ait tüm kartları getir
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok

7. **POST** `/api/card`
   - Açıklama: Yeni kart oluştur
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Başkasının account'ına kart oluşturabilir

8. **PUT** `/api/card/{id}`
   - Açıklama: Kartı güncelle
   - Rol: Authenticated (hem Admin hem Customer)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok

---

## 🔐 AUTHENTICATED-ONLY ENDPOINTS (8)
**Kimliği doğrulanmış herhangi bir kullanıcı erişebilir - [Authorize]**

### Account Controller
1. **GET** `/api/account`
   - Açıklama: Tüm hesapları listele
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Tüm hesapları görebilir (admin kontrol ederken customers sadece kendi hesaplarını görmeli)

2. **GET** `/api/account/{accountId}/details`
   - Açıklama: Hesap detaylarını getir
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok

3. **GET** `/api/account/{accountId}/balance`
   - Açıklama: Hesap bakiyesi getir
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok

4. **GET** `/api/account/customer/{customerId}`
   - Açıklama: Customer'ın hesabını getir
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership kontrolü yok

5. **GET** `/api/account/balance-range`
   - Açıklama: Bakiye aralığına göre hesap ara
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Admin özelliği, customers erişmemeli

### Customer Controller
6. **GET** `/api/customer/{id}`
   - Açıklama: Customer detayları getir
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Korunmuş ama Rol kontrolü eksik
   - 🔴 SORUN: Ownership kontrolü yok

7. **GET** `/api/customer`
   - Açıklama: Tüm customers listele
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Admin özelliği, customers erişmemeli

8. **DELETE** `/api/customer/{id}`
   - Açıklama: Customer sil
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Potansiyel Risk!
   - 🔴 SORUN: Ownership ve Admin kontrolü yok

### Customer Controller
9. **POST** `/api/customer`
   - Açıklama: Yeni customer oluştur
   - Rol: Authenticated (tüm roller)
   - Güvenlik: ⚠️ Korunmuş ama Rol kontrolü eksik
   - 📝 Not: Public olabilir (registration aşaması)

### Transaction Controller
10. **POST** `/api/transaction`
    - Açıklama: Yeni transaction oluştur
    - Rol: Authenticated (tüm roller)
    - Güvenlik: ✅ Doğru
    - 📝 Not: Authenticated kullanıcılar transaction yapabilir

### Transaction Controller (Query Endpoints)
11. **GET** `/api/transaction`
    - Açıklama: Tüm transactions listele
    - Rol: Authenticated (tüm roller) - YOK!
    - Güvenlik: ⚠️ Potansiyel Risk!
    - 🔴 SORUN: Hiçbir [Authorize] var yok, Public! Tüm transactionları kimse görebilir

12. **GET** `/api/transaction/account/{accountId}`
    - Açıklama: Hesabın transactionlarını listele
    - Rol: Authenticated (tüm roller) - YOK!
    - Güvenlik: ⚠️ Potansiyel Risk!
    - 🔴 SORUN: Hiçbir [Authorize] var yok, Public! Ownership kontrolü yok

13. **GET** `/api/transaction/date-range`
    - Açıklama: Tarih aralığına göre transaction ara
    - Rol: Authenticated (tüm roller) - YOK!
    - Güvenlik: ⚠️ Potansiyel Risk!
    - 🔴 SORUN: Hiçbir [Authorize] var yok, Public!

14. **GET** `/api/transaction/type/{transactionType}`
    - Açıklama: Transaction tipine göre ara
    - Rol: Authenticated (tüm roller) - YOK!
    - Güvenlik: ⚠️ Potansiyel Risk!
    - 🔴 SORUN: Hiçbir [Authorize] var yok, Public!

---

## 🎯 Önerilen Düzenlemeler

### 🔴 YÜKSEK PRİORİTE (Güvenlik Riski)

#### 1. Transaction Controller - Public olan tüm GET endpoints
**Sorun:** Hiçbir `[Authorize]` yoktur, tamamen public durumdadır.
**Çözüm:**
```csharp
// Şu an: Yok
// Olması gereken: [Authorize]

[HttpGet]
[Authorize]  // ← EKLE
public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAll()
```

**Etkilenen Endpointler:**
- GET `/api/transaction`
- GET `/api/transaction/account/{accountId}`
- GET `/api/transaction/date-range`
- GET `/api/transaction/type/{transactionType}`

#### 2. Card Controller - Ownership Kontrolü Eksik
**Sorun:** Customers tüm kartları görebilir, başkasının kartına erişebilir.
**Çözüm:** Her endpoint'te ownership kontrolü ekle:
```csharp
// Örnek: GetCard endpoint'i
[HttpGet("{id}")]
[Authorize]
public async Task<ActionResult<CardDto>> GetCard(Guid id)
{
    var result = await _cardService.GetCardByIdAsync(id);
    if (!result.IsSuccess)
        return StatusCode(result.StatusCode ?? 500, result.Message);
    
    // Ownership kontrolü ekle
    var currentUserId = User.FindFirst("UserId")?.Value;
    if (result.Data?.Account?.Customer?.UserId.ToString() != currentUserId && !User.IsInRole("Admin"))
        return Forbid();
    
    return Ok(result.Data);
}
```

**Etkilenen Endpointler:**
- GET `/api/card` - Tüm kartlar listesi (Admin-only olmalı)
- GET `/api/card/{id}` - Ownership kontrolü gerekli
- GET `/api/card/account/{accountId}` - Ownership kontrolü gerekli
- POST `/api/card` - Ownership kontrolü gerekli
- PUT `/api/card/{id}` - Ownership kontrolü gerekli

#### 3. Account Controller - Büyük Veri Sızması Riski
**Sorun:** `GET /api/account` tüm hesapları listeler, `GET /api/account/balance-range` tüm hesapların bakiyesini gösterir.
**Çözüm:** 
- Admin-only yap veya
- Customers sadece kendi hesaplarını görebilsin

```csharp
[HttpGet]
[Authorize(Roles = "Admin")]  // ← DEĞIŞTIR
public async Task<ActionResult<IEnumerable<AccountDetailsDto>>> GetAll()
```

#### 4. Customer Controller - Tüm Customers Listesi
**Sorun:** `GET /api/customer` tüm customers'ı listeler.
**Çözüm:**
```csharp
[HttpGet]
[Authorize(Roles = "Admin")]  // ← DEĞIŞTIR
public async Task<IActionResult> GetAllCustomersAsync()
```

---

### 🟠 ORTA PRİORİTE (Ownership Kontrolü)

#### 5. Account Controller Update/Delete
**Sorun:** Herhangi bir kullanıcı herhangi bir hesabı güncelleyebilir/silebilir.
**Çözüm:**
```csharp
[HttpPut("{accountId}")]
[Authorize]
public async Task<ActionResult<AccountDto>> Update(Guid accountId, [FromBody] UpdateAccountRequest request)
{
    // Ownership kontrolü ekle
    var currentUserId = User.FindFirst("UserId")?.Value;
    var account = await _accountService.GetAccountAsync(accountId);
    
    if (account.Data?.Customer?.UserId.ToString() != currentUserId && !User.IsInRole("Admin"))
        return Forbid();
    
    var result = await _accountService.UpdateAccountAsync(accountId, request);
    if (result.IsSuccess)
        return Ok(result.Data);

    return BadRequest(new { message = result.Message });
}
```

#### 6. Customer Controller Delete
**Sorun:** Herhangi bir kullanıcı herhangi bir customer'ı silebilir.
**Çözüm:**
```csharp
[HttpDelete("{id}")]
[Authorize]
public async Task<IActionResult> DeleteCustomer(Guid id)
{
    // Ownership kontrolü ekle veya Admin-only yap
    var currentUserId = User.FindFirst("UserId")?.Value;
    if (id.ToString() != currentUserId && !User.IsInRole("Admin"))
        return Forbid();
    
    var result = await _customerService.DeleteCustomerAsync(id);
    if (result.IsSuccess)
        return StatusCode(StatusCodes.Status200OK, "Customer deleted successfully.");

    return NotFound(new { message = result.Message });
}
```

---

### 🟡 DÜŞÜK PRİORİTE (Best Practices)

#### 7. Customer Post Endpoint
**Durum:** Şu an Authenticated (tüm rollerden) gerekli.
**Soru:** Bu customer oluşturma mı yoksa customer profili güncelleme mi?
**Önerilen:** Public olabilir (registration sırasında) veya Customer-only olmalı.

```csharp
[HttpPost]
[AllowAnonymous]  // ← DÜŞÜN: Registration sırasında public olmalı mı?
// veya
[Authorize(Roles = "Customer")]  // Customers kendi profil oluşturabilsin
```

---

## 📊 Düzeltilmiş Sınıflandırma (Önerilen)

| Kategori | Şu An | Önerilen | Not |
|----------|-------|----------|-----|
| Public | 2 | 2 | Auth endpoints doğru |
| Admin-Only | 3 | 6+ | Bazı endpoints Admin-only olmalı |
| Customer-Only | 0 | 1-2 | Bazı endpoints Customer-only olabilir |
| Admin + Customer | 10 | 5-8 | Ownership kontrolü ile |
| Authenticated (Tüm Roller) | 8 | 0 | Azaltılmalı |

---

## ✅ Aksiyon Planı

### 1. Adım: Transaction Controller Güvenlik (ACIL)
- [ ] Tüm GET endpoints'e `[Authorize]` ekle
- [ ] Ownership kontrolü ekle

### 2. Adım: Card Controller Güvenlik
- [ ] `GET /api/card` - Admin-only yap
- [ ] Diğer endpointlere ownership kontrolü ekle

### 3. Adım: Account Controller Güvenlik
- [ ] `GET /api/account` - Admin-only yap
- [ ] `GET /api/account/balance-range` - Admin-only yap
- [ ] Update/Delete endpointlere ownership kontrolü ekle

### 4. Adım: Customer Controller Güvenlik
- [ ] `GET /api/customer` - Admin-only yap
- [ ] Delete endpointine ownership kontrolü ekle

### 5. Adım: Helper Method Oluştur
```csharp
// ServiceCollectionExtension.cs veya yeni bir Authorization helper
public static bool IsOwnerOrAdmin(this ClaimsPrincipal user, Guid resourceUserId)
{
    var currentUserId = user.FindFirst("UserId")?.Value;
    return resourceUserId.ToString() == currentUserId || user.IsInRole("Admin");
}
```

---

## 📌 Notlar

- **[Authorize]:** Sadece authenticated (JWT token ile) kullanıcılar erişebilir
- **[Authorize(Roles = "Admin")]:** Sadece Admin rolü erişebilir
- **[Authorize(Roles = "Admin,Customer")]:** Admin veya Customer rolü erişebilir
- **[AllowAnonymous]:** Herkes erişebilir (token gerekli değil)
- **Ownership:** Kullanıcının kendi verilerine mi yoksa başkasının verilerine mi eriştiğinin kontrolü

---

## 🔗 Referans Dosyalar

- `/payment-system.Api/Controllers/Account/AccountController.Query.cs`
- `/payment-system.Api/Controllers/Account/AccountController.Command.cs`
- `/payment-system.Api/Controllers/Admin/AdminController.cs`
- `/payment-system.Api/Controllers/Auth/AuthController.cs`
- `/payment-system.Api/Controllers/Card/CardController.cs`
- `/payment-system.Api/Controllers/Customer/CustomerController.Query.cs`
- `/payment-system.Api/Controllers/Customer/CustomerController.Command.cs`
- `/payment-system.Api/Controllers/Transaction/TransactionController.Query.cs`
- `/payment-system.Api/Controllers/Transaction/TransactionController.Command.cs`

