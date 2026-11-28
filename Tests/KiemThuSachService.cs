using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Exceptions;

namespace LibraryManagementSystem.Tests;

public static class KiemThuSachService
{
    public static int RunAll()
    {
        int soCaDat = 0;
        int soCaLoi = 0;

        void Chay(string ten, Action<DuLieuKiemThu> kiemThu)
        {
            try
            {
                kiemThu(new DuLieuKiemThu());
                soCaDat++;
                Console.WriteLine($"PASS: {ten}");
            }
            catch (Exception ex)
            {
                soCaLoi++;
                Console.WriteLine($"FAIL: {ten} — {ex.Message}");
            }
        }

        Chay("Đổi tên tác giả và lưu vào đúng bản ghi", f =>
        {
            f.Sach.LuuTacGia(f.Phien, "them-tac-gia", "TG01", "Nguyễn An");

            f.Sach.LuuTacGia(
                f.Phien, "doi-ten-tac-gia", "TG01", "Nguyễn Văn An");

            var tacGia = f.Kho.DaLuu<TacGia>().Single();

            XacNhan(tacGia.MaTacGia == "TG01", "Mã tác giả phải giữ nguyên.");
            XacNhan(tacGia.HoTen == "Nguyễn Văn An",
                "Tên mới phải được lưu vào kho.");
        });

        Chay("Thêm đầu sách với danh mục hợp lệ", f =>
        {
            f.TaoDauSach();

            var dauSach = f.Kho.DaLuu<DauSach>().Single();

            XacNhan(dauSach.MaDauSach == "DS01",
                "Kho phải lưu đầu sách vừa thêm.");
            XacNhan(dauSach.MaTheLoai == "TL01",
                "Đầu sách phải thuộc đúng thể loại.");
            XacNhan(dauSach.MaNXB == "NXB01",
                "Đầu sách phải thuộc đúng nhà xuất bản.");
        });

        Chay("Từ chối nhập cuốn khi đầu sách không tồn tại", f =>
        {
            MongDoiLoi("KHONG_TIM_THAY", () =>
            {
                f.Sach.NhapCuon(
                    f.Phien, "nhap-sai", "CS01",
                    "DS-KHONG-CO", null, TinhTrangSach.Tot);
            });

            XacNhan(f.Kho.DaLuu<CuonSach>().Count == 0,
                "Yêu cầu thất bại không được lưu cuốn sách.");
        });

        Chay("Nhập hai cuốn khác mã cho cùng một đầu sách", f =>
        {
            f.TaoDauSach();
            f.NhapCuon("CS01");
            f.NhapCuon("CS02");

            var cacCuon = f.Kho.DaLuu<CuonSach>();

            XacNhan(cacCuon.Count == 2,
                "Kho phải có đúng hai cuốn.");
            XacNhan(cacCuon.All(cuon => cuon.MaDauSach == "DS01"),
                "Cả hai cuốn phải thuộc đầu sách DS01.");
            XacNhan(cacCuon.Select(cuon => cuon.MaCuon)
                    .ToHashSet()
                    .SetEquals(new[] { "CS01", "CS02" }),
                "Hai cuốn phải có đúng hai mã đã nhập.");
        });

        Chay("Chuyển cuốn sang kệ mới và lưu thay đổi", f =>
        {
            var cuon = f.TaoCuonTrenKe();

            f.Sach.ChuyenKe(
                f.Phien, "chuyen-ke", cuon.MaCuon, "KE02",
                "Ô 2", cuon.PhienBanDuLieu);

            var daLuu = f.DocCuon(cuon.MaCuon);

            XacNhan(daLuu.MaKe == "KE02",
                "Kho phải lưu mã kệ mới.");
            XacNhan(daLuu.PhienBanDuLieu > cuon.PhienBanDuLieu,
                "Phiên bản của cuốn phải tăng sau khi chuyển kệ.");
        });

        Chay("Từ chối chuyển kệ với phiên bản cũ", f =>
        {
            var cuon = f.TaoCuonTrenKe();
            long phienBanCu = cuon.PhienBanDuLieu;

            f.Sach.ChuyenKe(
                f.Phien, "chuyen-lan-dau", cuon.MaCuon,
                "KE02", "Ô 2", phienBanCu);

            var sauLanDau = f.DocCuon(cuon.MaCuon);

            XacNhan(sauLanDau.MaKe == "KE02"
                    && sauLanDau.PhienBanDuLieu > phienBanCu,
                "Cần lưu lần chuyển đầu tiên để tạo dữ liệu có phiên bản mới.");

            MongDoiLoi("DU_LIEU_CU", () =>
            {
                f.Sach.ChuyenKe(
                    f.Phien, "chuyen-voi-ban-cu", cuon.MaCuon,
                    "KE01", "Ô 1", phienBanCu);
            });

            var daLuu = f.DocCuon(cuon.MaCuon);

            XacNhan(daLuu.MaKe == "KE02"
                    && daLuu.PhienBanDuLieu == sauLanDau.PhienBanDuLieu,
                "Yêu cầu dùng phiên bản cũ không được đổi kệ hoặc phiên bản.");
        });

        Chay("Từ chối ngừng sử dụng kệ còn sách", f =>
        {
            var cuon = f.TaoCuonTrenKe();

            MongDoiLoi("KE_CON_SACH", () =>
            {
                f.Sach.NgungDungKe(f.Phien, "ngung-ke", "KE01");
            });

            var ke = f.Kho.DaLuu<KeSach>().Single(x => x.MaKe == "KE01");

            XacNhan(ke.DangSuDung, "Kệ phải tiếp tục hoạt động.");
            XacNhan(f.DocCuon(cuon.MaCuon).MaKe == "KE01",
                "Cuốn sách phải giữ nguyên kệ đang xếp.");
        });

        Console.WriteLine($"Kiểm thử SachService: {soCaDat} PASS, {soCaLoi} FAIL");

        return soCaLoi;
    }

    private static void XacNhan(bool dieuKien, string thongBao)
    {
        if (!dieuKien)
        {
            throw new InvalidOperationException(thongBao);
        }
    }

    private static void MongDoiLoi(string maLoi, Action thaoTac)
    {
        try
        {
            thaoTac();
        }
        catch (NghiepVuException ex)
        {
            XacNhan(ex.MaLoi == maLoi,
                $"Mong lỗi {maLoi}, nhận được {ex.MaLoi}.");

            return;
        }

        throw new InvalidOperationException(
            $"Mong lỗi {maLoi} nhưng thao tác đã thành công.");
    }

    private sealed class DuLieuKiemThu
    {
        public KiemThuTheService.KhoKiemThu Kho { get; } = new();
        public PhienLamViec Phien { get; }
        public SachService Sach { get; }

        public DuLieuKiemThu()
        {
            const string matKhau = "MatKhauTest01";

            Kho.BatDau();

            Kho.Kho<NhanVien>().Luu(
                new NhanVien("NV01", "Thủ thư kiểm thử", VaiTro.ThuThu));

            Kho.Kho<TaiKhoan>().Luu(
                new TaiKhoan(
                    "TK01", "thuthu", matKhau, VaiTro.ThuThu,
                    maNhanVien: "NV01"));

            Kho.Commit();

            var taiKhoan = new TaiKhoanService(Kho);
            Phien = taiKhoan.DangNhap("thuthu", matKhau);

            var dieuPhoi = new BoDieuPhoi(Kho, new DongHoKiemThu(), taiKhoan);
            Sach = new SachService(dieuPhoi);
        }

        public void TaoDauSach()
        {
            Sach.LuuTacGia(Phien, "tao-tg", "TG01", "Nguyễn An");
            Sach.LuuTheLoai(Phien, "tao-tl", "TL01", "Tin học");
            Sach.LuuNhaXuatBan(Phien, "tao-nxb", "NXB01", "Nhà xuất bản mẫu");

            Sach.ThemDauSach(
                Phien,
                "tao-dau-sach",
                "DS01",
                "Lập trình hướng đối tượng",
                "Bản 1",
                "Tiếng Việt",
                new[] { "TG01" },
                "TL01",
                "NXB01");
        }

        public CuonSach NhapCuon(string maCuon, string? maKe = null)
        {
            return Sach.NhapCuon(
                Phien,
                $"nhap-{maCuon}",
                maCuon,
                "DS01",
                50_000m,
                TinhTrangSach.Tot,
                maKe,
                maKe is null ? null : "Ô 1");
        }

        public CuonSach TaoCuonTrenKe()
        {
            TaoDauSach();

            Sach.LuuKe(Phien, "tao-ke-01", "KE01", "A", "1", "01");
            Sach.LuuKe(Phien, "tao-ke-02", "KE02", "A", "1", "02");

            var cuon = NhapCuon("CS01", "KE01");

            return DocCuon(cuon.MaCuon);
        }

        public CuonSach DocCuon(string maCuon)
        {
            return Kho.DaLuu<CuonSach>().Single(cuon => cuon.MaCuon == maCuon);
        }
    }

    private sealed class DongHoKiemThu : IDongHo
    {
        public DateTimeOffset BayGio { get; } =
            new(2026, 11, 28, 9, 0, 0, TimeSpan.FromHours(7));

        public DateOnly HomNay => DateOnly.FromDateTime(BayGio.DateTime);
    }
}