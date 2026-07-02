# FIN-ADVISOR

FIN-ADVISOR adalah aplikasi web sistem pakar untuk merekomendasikan sumber
pembiayaan bagi pelaku UMKM. Aplikasi menggunakan metode forward chaining,
bukan machine learning. Semua keputusan berasal dari rule IF-THEN yang dapat
dilihat dan dikelola melalui admin panel.

## Fitur

- Dashboard dan form konsultasi UMKM.
- Landing page publik, login admin, login UMKM, registrasi UMKM, dan logout.
- Cookie Authentication dan Role-Based Access Control untuk Administrator dan Customer.
- Profil UMKM yang bisa diedit: nama usaha, pemilik, kontak, alamat, jenis
  usaha, omzet, dan tujuan pembiayaan.
- Rule engine terpisah: Fact, Rule, WorkingMemory, InferenceEngine,
  KnowledgeBase, RuleEngine, dan RecommendationService.
- Explanation facility: menampilkan rule aktif, fakta terpenuhi, penjelasan,
  skor kesesuaian, keuntungan, risiko, persyaratan, estimasi proses, checklist
  dokumen, dan saran tambahan.
- Riwayat konsultasi dengan pencarian dan filter.
- Area admin terpisah untuk tambah, ubah, hapus, aktifkan, dan nonaktifkan rule.
- SQLite dengan tabel Users, Consultations, Rules, Recommendations, dan History.
- Dark mode, responsive layout, toast notification, hover effect, print, dan
  export PDF melalui dialog print browser.

## Basis Aturan Default

| Kode | IF | THEN |
| --- | --- | --- |
| R1 | Arus kas tidak stabil dan riwayat kredit buruk | Tunda Pinjaman |
| R2 | Arus kas stabil, agunan memadai, legalitas lengkap | Bank |
| R3 | Arus kas stabil, agunan kecil, dana sedang | Koperasi |
| R4 | Dana kecil, tidak ada agunan, butuh dana cepat | Fintech |
| R5 | Legalitas tidak lengkap dan dana besar | Lengkapi Legalitas |

## Struktur Proyek

```text
src/
  FinAdvisor.Core/
    KnowledgeBase/
    Models/
    RuleEngine/
    Rules/
    Services/
  FinAdvisor.Web/
    Assets/
    Configuration/
    Controllers/
    Helpers/
    Models/
    Pages/
    Services/
    Views/
    wwwroot/
  FinAdvisor.ConsoleApp/
tests/
  FinAdvisor.Tests/
```

## Cara Menjalankan

Membutuhkan .NET 10 SDK.

```bash
dotnet restore
dotnet run --project src/FinAdvisor.Web
```

Buka alamat yang muncul di terminal, biasanya `https://localhost:5001` atau
`http://localhost:5000`.

Database SQLite `finadvisor.db` akan dibuat otomatis saat aplikasi pertama
kali berjalan. Rule default, akun admin awal, dan akun UMKM awal juga akan di-seed otomatis.

## Akun Awal

Admin:

```text
Email    : admin@finadvisor.local
Password : Admin12345!
Route    : /admin/login
```

UMKM:

```text
Email    : umkm@finadvisor.local
Password : Umkm12345!
Route    : /login
```

## Struktur Route

- Public: `/`, `/about`, `/help`, `/login`, `/register`
- Customer UMKM: `/umkm/dashboard`, `/umkm/profile`, `/umkm/consultation`, `/umkm/history`, `/umkm/recommendation`
- Admin: `/admin/login`, `/admin/dashboard`, `/admin/users`, `/admin/rules`, `/admin/knowledge`, `/admin/reports`, `/admin/settings`

## Menjalankan Test

```bash
dotnet test
```

## Mengubah Rule dari Admin

Buka `/admin/rules`. Kondisi rule memakai JSON berikut:

```json
[
  { "FactName": "ArusKas", "ExpectedValue": "Stabil" },
  { "FactName": "Agunan", "ExpectedValue": "Memadai" }
]
```

FactName yang tersedia:

- ArusKas: Stabil, TidakStabil
- Agunan: Memadai, Kecil, TidakAda
- Dana: Kecil, Sedang, Besar
- RiwayatKredit: Baik, Buruk, BelumAda
- Legalitas: Lengkap, TidakLengkap
- ButuhDanaCepat: Ya, Tidak
