# 🔐 Endpoint Yetkilendirme Sınıflandırması - Görsel Özet

## 📊 Endpoint Dağılımı

```
┌─────────────────────────────────────────────────────────────────┐
│                    TOPLAM 28 ENDPOINT                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  🔓 PUBLIC          ██ (2)         7%                           │
│  🟢 ADMIN-ONLY      ███ (3)       11%                           │
│  ⚠️  RISKY           ████████████████████ (23)       82%         │
│                                                                 │
│     ├─ 🔴 YÜKSEK RİSK   ████████ (18)     64%                   │
│     └─ 🟠 ORTA RİSK      ████ (5)         18%                   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎯 Controller Bazında Durum

### Auth Controller ✅
```
📊 Durum: GÜVENLI

[AllowAnonymous] - POST /login        [✅ Doğru]
[AllowAnonymous] - POST /register     [✅ Doğru]

Açıklama: Giriş ve kayıt public olmalı. Email/Password ile korunmuş.
```

### Admin Controller ✅
```
📊 Durum: GÜVENLI

[Admin] - GET  /admin              [✅ Doğru]
[Admin] - POST /admin              [✅ Doğru]
[Admin] - GET  /admin/dashboard    [✅ Doğru]

Açıklama: Tüm endpoints Admin-only. Uygun kullanım.
```

### Account Controller 🔴 YÜKSEK RİSK
```
📊 Durum: GÜVENLI DEĞİL

┌─ Query Endpoints ─────────────────────────────────────────┐
│ [Authorize] - GET  /account                [🔴 SORUN]      │
│              ├─ Risk: Tüm hesapları görebilir             │
│              └─ Çözüm: Admin-only yap                    │
│                                                           │
│ [Authorize] - GET  /account/{id}/details   [🔴 SORUN]      │
│              ├─ Risk: Ownership kontrolü yok              │
│              └─ Çözüm: Ownership kontrol ekle             │
│                                                           │
│ [Authorize] - GET  /account/{id}/balance   [🔴 SORUN]      │
│              ├─ Risk: Ownership kontrolü yok              │
│              └─ Çözüm: Ownership kontrol ekle             │
│                                                           │
│ [Authorize] - GET  /account/customer/{id}  [🔴 SORUN]      │
│              ├─ Risk: Ownership kontrolü yok              │
│              └─ Çözüm: Ownership kontrol ekle             │
│                                                           │
│ [Authorize] - GET  /account/balance-range  [🔴 SORUN]      │
│              ├─ Risk: Tüm hesapların bakiyesi açık       │
│              └─ Çözüm: Admin-only yap                    │
└─────────────────────────────────────────────────────────────┘

┌─ Command Endpoints ───────────────────────────────────────┐
│ [Admin,Customer] - POST /account           [⚠️  UYARI]      │
│                   ├─ Rol: Doğru                           │
│                   └─ Dikkat: Ownership kontrolü gerekli   │
│                                                           │
│ [Authorize] - PUT  /account/{id}           [🟠 ORTA]        │
│              ├─ Risk: Ownership kontrolü yok              │
│              └─ Çözüm: Ownership kontrol ekle             │
│                                                           │
│ [Authorize] - DELETE /account/{id}         [🟠 ORTA]        │
│              ├─ Risk: Ownership kontrolü yok              │
│              └─ Çözüm: Ownership kontrol ekle             │
└─────────────────────────────────────────────────────────────┘

Genel Durum: 🔴 GÜVENLI DEĞİL
Sorun Sayısı: 8/8 endpoint'in sorunu var
```

### Card Controller 🔴 YÜKSEK RİSK
```
📊 Durum: GÜVENLI DEĞİL

[Authorize] - GET  /card                    [🔴 SORUN]
             ├─ Risk: Tüm kartları görebilir
             └─ Çözüm: Admin-only yap

[Authorize] - GET  /card/{id}               [🔴 SORUN]
             ├─ Risk: Ownership kontrolü yok
             └─ Çözüm: Ownership kontrol ekle

[Authorize] - GET  /card/account/{id}       [🔴 SORUN]
             ├─ Risk: Ownership kontrolü yok
             └─ Çözüm: Ownership kontrol ekle

[Authorize] - POST /card                    [🔴 SORUN]
             ├─ Risk: Başkasının account'ına kart oluşturabilir
             └─ Çözüm: Ownership kontrol ekle

[Authorize] - PUT  /card/{id}               [🔴 SORUN]
             ├─ Risk: Ownership kontrolü yok
             └─ Çözüm: Ownership kontrol ekle

Genel Durum: 🔴 GÜVENLI DEĞİL
Sorun Sayısı: 6/6 endpoint'in sorunu var
```

### Customer Controller 🟠 ORTA RİSK
```
📊 Durum: GÜVENLI DEĞİL

[Authorize] - GET  /customer                [🔴 SORUN]
             ├─ Risk: Tüm customers'ları görebilir
             └─ Çözüm: Admin-only yap

[Authorize] - GET  /customer/{id}           [🟠 ORTA]
             ├─ Risk: Ownership kontrolü yok
             └─ Çözüm: Ownership kontrol ekle

[Authorize] - POST /customer                [⚠️  UYARI]
             ├─ Rol: Belirlenmemiş
             └─ Dikkat: Public veya Customer-only olmalı

[Authorize] - DELETE /customer/{id}         [🟠 ORTA]
             ├─ Risk: Ownership kontrolü yok
             └─ Çözüm: Ownership kontrol ekle

Genel Durum: 🟠 GÜVENLI DEĞİL
Sorun Sayısı: 4/4 endpoint'in sorunu var
```

### Transaction Controller 🔴 YÜKSEK RİSK (ACİL!)
```
📊 Durum: GÜVENLI DEĞİL - PUBLIC ENDPOINTS!

⚠️⚠️⚠️ UYARI ⚠️⚠️⚠️ - HİÇBİR [Authorize] YOKTUR!

[NO AUTH] - GET  /transaction               [🔴🔴🔴 SORUN]
          ├─ Risk: TÜM TRANSACTIONLAR HALKA AÇIK!
          ├─ Risk: Kimse tüm transactionları görebilir
          └─ Çözüm: [Authorize] EKLE!

[NO AUTH] - GET  /transaction/account/{id}  [🔴🔴🔴 SORUN]
          ├─ Risk: TÜM HESAPLARIN İŞLEMLERİ AÇIK!
          ├─ Risk: Ownership kontrolü yok
          └─ Çözüm: [Authorize] + Ownership kontrol

[NO AUTH] - GET  /transaction/date-range    [🔴🔴🔴 SORUN]
          ├─ Risk: TÜM TRANSACTIONLAR HALKA AÇIK!
          └─ Çözüm: [Authorize] EKLE!

[NO AUTH] - GET  /transaction/type/{type}   [🔴🔴🔴 SORUN]
          ├─ Risk: TÜM TRANSACTIONLAR HALKA AÇIK!
          └─ Çözüm: [Authorize] EKLE!

[Authorize] - POST /transaction             [✅ OK]

Genel Durum: 🔴 GÜVENLI DEĞİL - ACİL DÜZELTME GEREKLİ
Sorun Sayısı: 4/5 endpoint PUBLIC!
```

---

## 🚨 Öncelikli Sorunlar

### 🔴 ACİL (1-2 saat içinde çözülmeli)
```
1. Transaction Controller - TÜM GET endpoints PUBLIC!
   Riski: Finansal işlemler herkese açık
   Etki: Yüksek
   Çözüm: 4 satır kod - [Authorize] ekle

2. Account Controller - Tüm hesapları listele
   Riski: Tüm hesap bilgileri açık
   Etki: Yüksek
   Çözüm: Admin-only yap
```

### 🟠 YÜKSEK (1 gün içinde çözülmeli)
```
3. Card Controller - Ownership kontrolü eksik
   Riski: Başkasının kartını görebilir/silebilir
   Etki: Yüksek
   Çözüm: 6 endpoint'e ownership kontrolü ekle

4. Account Controller - Ownership kontrolü eksik
   Riski: Başkasının hesabını güncelleyebilir/silebilir
   Etki: Yüksek
   Çözüm: 3 endpoint'e ownership kontrolü ekle
```

### 🟡 ORTA (1 hafta içinde çözülmeli)
```
5. Customer Controller - Ownership kontrolü eksik
   Riski: Başkasını silebilir
   Etki: Orta
   Çözüm: 2 endpoint'e ownership kontrolü ekle
```

---

## 💡 Önerilen Çözüm Örneği

### Ownership Helper Method (Bir kez yazıp tüm yerde kullan)

```csharp
// Extension method oluştur
public static class AuthorizationExtensions
{
    public static bool IsOwnerOrAdmin(
        this ClaimsPrincipal user, 
        Guid resourceOwnerId)
    {
        var currentUserId = user.FindFirst("UserId")?.Value;
        return resourceOwnerId.ToString() == currentUserId 
               || user.IsInRole("Admin");
    }
}

// Endpoint'te kullan
[HttpGet("{id}")]
[Authorize]
public async Task<ActionResult<CardDto>> GetCard(Guid id)
{
    var result = await _cardService.GetCardByIdAsync(id);
    if (!result.IsSuccess)
        return StatusCode(result.StatusCode ?? 500, result.Message);
    
    // Bir satır!
    if (!User.IsOwnerOrAdmin(result.Data.Account.Customer.UserId))
        return Forbid();
    
    return Ok(result.Data);
}
```

---

## 📈 Düzeltme Sonrası Hedef

```
┌─────────────────────────────────────────────────────────────────┐
│                 DÜZELTME SONRASI HEDEFİ                        │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  🔓 PUBLIC          ██ (2)         7%   - Aynı                  │
│  🟢 ADMIN-ONLY      ███████ (7)   25%   - ARTTI ↑               │
│  🔐 GUARDED         ████████████ (19)  68%   - Ownership ile    │
│                                                                 │
│     ├─ Admin-only      7                                        │
│     ├─ Admin+Customer  5      Ownership kontrolü ile            │
│     └─ Authenticated   7      Ownership kontrolü ile            │
│                                                                 │
│  Güvenlik Puanı: 18% → 93%  ⬆️⬆️⬆️                              │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## ✅ Checklist

### Faz 1: Transaction Controller (ACIL)
- [ ] TransactionController.Query.cs açıyorum
- [ ] Tüm 4 GET endpoint'e `[Authorize]` ekle
- [ ] Test et: curl ile kontrol
- [ ] Commit: "fix: Secure transaction endpoints with [Authorize]"

### Faz 2: List Endpoints (Admin-only)
- [ ] AccountController.Query.cs - `GET /account` → Admin-only
- [ ] AccountController.Query.cs - `GET /balance-range` → Admin-only
- [ ] CardController.cs - `GET /card` → Admin-only
- [ ] CustomerController.Query.cs - `GET /customer` → Admin-only
- [ ] Test et
- [ ] Commit: "fix: Restrict list endpoints to admin users"

### Faz 3: Ownership Kontrol Helper
- [ ] Extension method oluştur: `IsOwnerOrAdmin()`
- [ ] Tüm endpoint'lerde ownership kontrolü ekle
- [ ] Test et
- [ ] Commit: "feat: Add ownership authorization checks"

### Faz 4: Tests
- [ ] Unit testler yazma
- [ ] Integration testler
- [ ] Commit: "test: Add authorization tests"

---

## 📞 Sorular?

- **Soru:** Customer kendi hesabını silmeli mi?
  **Cevap:** Hayır, sadece Admin silmeli (soft delete kullan)

- **Soru:** Admin başka admin görebilir mi?
  **Cevap:** Evet, admin tüm admins'leri görebilir (log tutmak için)

- **Soru:** Transaction sahibi başkasının transaction'ını görebilir mi?
  **Cevap:** Hayır, sadece kendi transaction'larını (ve Admin tümü)

- **Soru:** Card sahibi başkasının kartını görebilir mi?
  **Cevap:** Hayır, sadece kendi kartlarını

