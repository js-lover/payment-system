# Endpoint Yetkilendirme Hızlı Referans Tablosu

## 🔴 PUBLIC ENDPOINTS (Kimse tarafından erişilebilir)

| # | HTTP | Endpoint | Açıklama | Güvenlik |
|---|------|----------|----------|----------|
| 1 | POST | `/api/auth/login` | Giriş yap | ✅ Email/Password |
| 2 | POST | `/api/auth/register` | Kayıt ol | ✅ Email/Password |

---

## 🟢 ADMIN-ONLY ENDPOINTS

| # | HTTP | Endpoint | Açıklama | Güvenlik |
|---|------|----------|----------|----------|
| 3 | GET | `/api/admin` | Admin listesi | ✅ [Authorize(Roles = "Admin")] |
| 4 | POST | `/api/admin` | Admin oluştur | ✅ [Authorize(Roles = "Admin")] |
| 5 | GET | `/api/admin/dashboard` | Dashboard istatistikleri | ✅ [Authorize(Roles = "Admin")] |

---

## 🟡 RISK ALTINDA OLAN ENDPOINTS (Düzeltilmesi Gerekli)

### 🔴 YÜKSEK RİSK - Transaction Controller (Public!)

| # | HTTP | Endpoint | Şu Hal | Olması Gereken | Risk |
|---|------|----------|--------|---|------|
| 6 | GET | `/api/transaction` | ❌ Public | ✅ [Authorize] | Tüm transactionlar kimse görebilir |
| 7 | GET | `/api/transaction/account/{id}` | ❌ Public | ✅ [Authorize] + Ownership | Başkasının hesabı görebilir |
| 8 | GET | `/api/transaction/date-range` | ❌ Public | ✅ [Authorize] | Tüm transactionlar kimse görebilir |
| 9 | GET | `/api/transaction/type/{type}` | ❌ Public | ✅ [Authorize] | Tüm transactionlar kimse görebilir |
| 10 | POST | `/api/transaction` | ✅ [Authorize] | ✅ [Authorize] | OK |

### 🔴 YÜKSEK RİSK - Account Controller (Tüm Veriler Açık)

| # | HTTP | Endpoint | Şu Hal | Olması Gereken | Risk |
|---|------|----------|--------|---|------|
| 11 | GET | `/api/account` | ⚠️ [Authorize] | ✅ [Authorize(Roles = "Admin")] | Tüm hesaplar listesi |
| 12 | GET | `/api/account/{id}/details` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının hesabı görebilir |
| 13 | GET | `/api/account/{id}/balance` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının bakiyesi görebilir |
| 14 | GET | `/api/account/customer/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının hesabı görebilir |
| 15 | GET | `/api/account/balance-range` | ⚠️ [Authorize] | ✅ [Authorize(Roles = "Admin")] | Tüm hesapların bakiyesi açık |
| 16 | POST | `/api/account` | ⚠️ [Authorize(Roles = "Admin,Customer")] | ✅ [Authorize(Roles = "Admin,Customer")] + Ownership | OK ama ownership kontrol ekle |
| 17 | PUT | `/api/account/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının hesabı güncelleyebilir |
| 18 | DELETE | `/api/account/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının hesabı silebilir |

### 🔴 YÜKSEK RİSK - Card Controller (Ownership Eksik)

| # | HTTP | Endpoint | Şu Hal | Olması Gereken | Risk |
|---|------|----------|--------|---|------|
| 19 | GET | `/api/card` | ⚠️ [Authorize] | ✅ [Authorize(Roles = "Admin")] | Tüm kartlar listesi |
| 20 | GET | `/api/card/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının kartı görebilir |
| 21 | GET | `/api/card/account/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının kartı görebilir |
| 22 | POST | `/api/card` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının account'ına kart oluşturabilir |
| 23 | PUT | `/api/card/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının kartı güncelleyebilir |
| 24 | DELETE | `/api/card/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının kartı silebilir |

### 🟠 ORTA RİSK - Customer Controller

| # | HTTP | Endpoint | Şu Hal | Olması Gereken | Risk |
|---|------|----------|--------|---|------|
| 25 | GET | `/api/customer` | ⚠️ [Authorize] | ✅ [Authorize(Roles = "Admin")] | Tüm customers listesi |
| 26 | GET | `/api/customer/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasının profili görebilir |
| 27 | POST | `/api/customer` | ⚠️ [Authorize] | ✅ [AllowAnonymous] veya [Authorize(Roles = "Customer")] | Belirlenmemiş |
| 28 | DELETE | `/api/customer/{id}` | ⚠️ [Authorize] | ✅ [Authorize] + Ownership | Başkasını silebilir |

---

## 📊 Özet İstatistik

```
Toplam Endpoint: 28

✅ Doğru: 5
   - 2 Public (Auth endpoints)
   - 3 Admin-only (Admin controller)

⚠️ Risk: 23
   - 🔴 Yüksek Risk (Public veya Ownership eksik): 18
   - 🟠 Orta Risk: 5

Güvenlik Puanı: 18% (5/28)
```

---

## 🔧 Düzeltme Şablonları

### Şablon 1: Admin-Only Endpoint
```csharp
[HttpGet]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<IEnumerable<T>>> GetAll()
{
    // ...
}
```

### Şablon 2: Ownership Kontrolü ile Authenticated Endpoint
```csharp
[HttpGet("{id}")]
[Authorize]
public async Task<ActionResult<T>> GetById(Guid id)
{
    var currentUserId = User.FindFirst("UserId")?.Value;
    var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
    
    var result = await _service.GetByIdAsync(id);
    if (!result.IsSuccess)
        return NotFound();
    
    // Admin herkesi görebilir, Customer sadece kendini görebilir
    if (currentUserRole != "Admin" && result.Data.UserId.ToString() != currentUserId)
        return Forbid();
    
    return Ok(result.Data);
}
```

### Şablon 3: Admin veya Owner Endpoint
```csharp
[HttpPut("{id}")]
[Authorize]
public async Task<ActionResult<T>> Update(Guid id, [FromBody] UpdateRequest request)
{
    var currentUserId = User.FindFirst("UserId")?.Value;
    var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
    
    var result = await _service.GetByIdAsync(id);
    if (!result.IsSuccess)
        return NotFound();
    
    // Sadece Admin veya Owner güncelleyebilir
    if (currentUserRole != "Admin" && result.Data.UserId.ToString() != currentUserId)
        return Forbid();
    
    var updateResult = await _service.UpdateAsync(id, request);
    return Ok(updateResult.Data);
}
```

---

## 🚀 Uygulama Adımları

### Faz 1: ACİL (Transaction Controller - PUBLIC!)
- [ ] TransactionController.Query.cs - Tüm GET'lere `[Authorize]` ekle
- [ ] Ownership kontrolü ekle

### Faz 2: Yüksek Öncelik (Veri Sızması Riski)
- [ ] Account Controller - List endpoint'i Admin-only yap
- [ ] Card Controller - List endpoint'i Admin-only yap
- [ ] Customer Controller - List endpoint'i Admin-only yap

### Faz 3: Ownership Kontrolü
- [ ] Tüm GET, PUT, DELETE endpointlerine ownership kontrolü ekle
- [ ] Helper method oluştur: `IsOwnerOrAdmin()`

### Faz 4: Test
- [ ] Unit testler yazma
- [ ] Integration testler yazma
- [ ] Swagger/API docs güncelleme

