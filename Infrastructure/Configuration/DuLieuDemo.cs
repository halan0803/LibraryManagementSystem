using LibraryManagementSystem.Application.Services;

namespace LibraryManagementSystem.Infrastructure.Configuration;

internal static class DuLieuDemo
{
    internal static ThongTinTruong[] Truong { get; } = [
        new("SV01", "Nguyễn Minh An", LoaiDocGia.SinhVien, true),
        new("GV01", "Trần Thu Hà", LoaiDocGia.GiangVien, true),
        new("SV02", "Lê Hoàng Anh", LoaiDocGia.SinhVien, true)];

    internal static void Tao(ITransactionManager db, DateOnly ngay)
    {
        if (db.Kho<TaiKhoan>().LayTatCa().Count != 0) return;
        foreach (var (ten, vaiTro) in new[] { ("thuthu", VaiTro.ThuThu), ("quanly", VaiTro.QuanLy), ("docgia", VaiTro.DocGia) })
        {
            if (vaiTro != VaiTro.DocGia)
                db.Kho<NhanVien>().Luu(new NhanVien { MaNhanVien = ten, HoTen = ten, VaiTro = vaiTro });
            db.Kho<TaiKhoan>().Luu(new TaiKhoan { MaTaiKhoan = ten, TenDangNhap = ten, VaiTro = vaiTro,
                MatKhauBam = NghiepVu.Bam("ThuVien@123"), MaDocGia = vaiTro == VaiTro.DocGia ? "DG01" : null,
                MaNhanVien = vaiTro == VaiTro.DocGia ? null : ten });
        }
        db.Kho<DocGia>().Luu(new SinhVien("DG01", "SV01", Truong[0].HoTen));
        db.Kho<TheThuVien>().Luu(new TheThuVien { MaThe = "THE01", MaDocGia = "DG01", NgayCap = ngay,
            NgayHieuLuc = ngay, NgayHetHan = ngay.AddMonths(6) });
        db.Kho<TacGia>().Luu(new TacGia { MaTacGia = "TG01", HoTen = "Phạm Minh" });
        db.Kho<DauSach>().Luu(new DauSach { MaDauSach = "DS01", TenSach = "Nhập môn lập trình", DanhSachMaTacGia = ["TG01"],
            PhienBan = "1", NgonNgu = "Tiếng Việt", GiaBia = 100000 });
        db.Kho<KeSach>().Luu(new KeSach { MaKe = "K01", ViTri = new("A", "01"), DangSuDung = true });
        foreach (string ma in new[] { "C01", "C02", "C03" })
            db.Kho<CuonSach>().Luu(new CuonSach { MaCuon = ma, MaDauSach = "DS01", GiaBia = 100000, MaKe = "K01" });
    }
}
