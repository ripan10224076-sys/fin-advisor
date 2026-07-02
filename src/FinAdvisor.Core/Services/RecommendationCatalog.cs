using FinAdvisor.Core.Models;

namespace FinAdvisor.Core.Services;

public sealed record RecommendationDetails(
    string Explanation,
    int Score,
    IReadOnlyList<string> Benefits,
    IReadOnlyList<string> Risks,
    IReadOnlyList<string> Requirements,
    string EstimatedProcess,
    IReadOnlyList<string> DocumentChecklist,
    IReadOnlyList<string> Suggestions);

public static class RecommendationCatalog
{
    public static RecommendationDetails Get(RekomendasiJenis jenis) => jenis switch
    {
        RekomendasiJenis.Bank => new(
            "Karena arus kas usaha stabil, legalitas lengkap, dan agunan memadai, sistem merekomendasikan pembiayaan melalui Bank.",
            92,
            new[] { "Bunga relatif rendah", "Plafon pembiayaan lebih besar", "Tenor pembayaran lebih panjang" },
            new[] { "Verifikasi dokumen lebih ketat", "Proses dapat lebih lama" },
            new[] { "Legalitas usaha lengkap", "Agunan memadai", "Catatan arus kas rapi" },
            "7-14 hari kerja",
            new[] { "KTP pemilik", "NIB/NPWP", "Rekening koran", "Laporan penjualan", "Dokumen agunan" },
            new[] { "Rapikan pembukuan 3-6 bulan terakhir", "Bandingkan bunga dan biaya administrasi antar bank" }),
        RekomendasiJenis.Koperasi => new(
            "Karena arus kas stabil, agunan terbatas, dan kebutuhan dana berada pada kategori sedang, sistem merekomendasikan Koperasi.",
            84,
            new[] { "Syarat relatif fleksibel", "Pendampingan anggota lebih dekat", "Cocok untuk pembiayaan menengah" },
            new[] { "Plafon biasanya lebih terbatas dari bank", "Ada kewajiban keanggotaan atau simpanan" },
            new[] { "Menjadi anggota koperasi", "Memiliki catatan usaha", "Menyetujui ketentuan simpanan" },
            "3-7 hari kerja",
            new[] { "KTP pemilik", "Kartu keluarga", "Catatan omzet", "Formulir anggota", "Bukti alamat usaha" },
            new[] { "Pilih koperasi resmi dan aktif", "Pastikan jadwal angsuran sesuai siklus penjualan" }),
        RekomendasiJenis.Fintech => new(
            "Karena kebutuhan dana kecil, tidak ada agunan, dan dana dibutuhkan cepat, sistem merekomendasikan Fintech lending yang legal dan diawasi OJK.",
            78,
            new[] { "Pengajuan cepat", "Tidak selalu membutuhkan agunan", "Cocok untuk kebutuhan kecil dan mendesak" },
            new[] { "Bunga dan denda bisa lebih tinggi", "Risiko memakai platform ilegal jika tidak hati-hati" },
            new[] { "Identitas pemilik valid", "Nomor rekening aktif", "Memilih penyelenggara resmi OJK" },
            "1-3 hari kerja",
            new[] { "KTP pemilik", "Foto usaha", "Rekening bank", "Nomor kontak aktif" },
            new[] { "Cek legalitas fintech di OJK", "Pinjam sesuai kemampuan bayar dan hindari gali lubang tutup lubang" }),
        RekomendasiJenis.TundaPinjaman => new(
            "Karena arus kas belum stabil dan riwayat kredit buruk, sistem merekomendasikan untuk menunda pinjaman baru.",
            88,
            new[] { "Mengurangi risiko gagal bayar", "Memberi waktu memperbaiki arus kas", "Mencegah beban utang bertambah" },
            new[] { "Kebutuhan modal belum langsung terpenuhi", "Perlu disiplin memperbaiki pembukuan" },
            new[] { "Evaluasi pemasukan dan pengeluaran", "Susun rencana pemulihan kredit", "Kurangi biaya yang tidak prioritas" },
            "30-90 hari untuk perbaikan awal",
            new[] { "Catatan utang berjalan", "Rekap penjualan", "Rekap biaya operasional" },
            new[] { "Negosiasikan ulang kewajiban lama jika memungkinkan", "Bangun dana darurat usaha sebelum pinjaman baru" }),
        RekomendasiJenis.LengkapiLegalitas => new(
            "Karena kebutuhan dana besar tetapi legalitas belum lengkap, sistem merekomendasikan melengkapi legalitas usaha terlebih dahulu.",
            86,
            new[] { "Membuka akses ke bank dan program pemerintah", "Meningkatkan kredibilitas usaha", "Memudahkan kerja sama bisnis" },
            new[] { "Ada waktu dan proses administrasi", "Pengajuan pembiayaan besar tertunda sementara" },
            new[] { "Mengurus NIB", "Menyiapkan NPWP bila diperlukan", "Merapikan identitas dan alamat usaha" },
            "1-7 hari untuk legalitas dasar, tergantung kelengkapan data",
            new[] { "KTP pemilik", "Email aktif", "Nomor telepon", "Data alamat usaha", "Data bidang usaha" },
            new[] { "Urus NIB melalui kanal resmi", "Setelah legalitas siap, ulangi analisis untuk melihat opsi bank" }),
        _ => new(
            "Data belum cukup untuk menentukan rekomendasi utama.",
            45,
            Array.Empty<string>(),
            Array.Empty<string>(),
            Array.Empty<string>(),
            "Perlu verifikasi lanjutan",
            Array.Empty<string>(),
            Array.Empty<string>())
    };
}
