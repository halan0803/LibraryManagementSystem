using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Services;

namespace LibraryManagementSystem.Tests;

public static class KiemThuHoSoService
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

        Chay("Còn sách thì không được đóng hồ sơ", f =>
        {
            f.TaoLuotMuon();

            var nghiaVu = f.HoSo.XemNghiaVu(f.QuanLy, "DG01");
            XacNhan(nghiaVu.SoCuonChuaHoanTat == 1 && !nghiaVu.DaHoanTat,
                "Hồ sơ phải còn một cuốn chưa hoàn tất.");

            MongDoiLoi("CON_NGHIA_VU", () =>
                f.HoSo.DongHoSo(f.QuanLy, "dong-con-sach", "DG01", "Rời trường"));

            XacNhan(f.DocHoSo().TinhTrangQuyen == TrangThaiDocGia.HoatDong,
                "Yêu cầu thất bại phải giữ nguyên trạng thái hồ sơ.");
        });

        Chay("Còn nợ thì không được xác nhận hoàn tất nghĩa vụ", f =>
        {
            f.Ghi(tx => tx.Kho<KhoanThu>().Luu(
                new KhoanThu("KT01", "DG01", "DM01", 5_000m)));

            var nghiaVu = f.HoSo.XemNghiaVu(f.QuanLy, "DG01");
            XacNhan(nghiaVu.TienConNo == 5_000m && !nghiaVu.DaHoanTat,
                "Phải tính đủ khoản nợ của độc giả.");

            MongDoiLoi("CON_NGHIA_VU", () =>
                f.HoSo.XacNhanNghiaVu(
                    f.QuanLy, "xac-nhan-con-no", "DG01", "Rời trường"));
        });

        Chay("Xác nhận hết nghĩa vụ chưa tự đóng hồ sơ", f =>
        {
            var ketQua = f.HoSo.XacNhanNghiaVu(
                f.QuanLy, "xac-nhan", "DG01", "Đã đối chiếu nghĩa vụ");

            XacNhan(ketQua.DaHoanTat, "Hồ sơ phải đủ điều kiện xác nhận.");
            XacNhan(f.DocHoSo().TinhTrangQuyen == TrangThaiDocGia.HoatDong,
                "Hồ sơ vẫn hoạt động sau khi xác nhận nghĩa vụ.");
        });

        Chay("Đóng hồ sơ lưu trạng thái, hủy thẻ và vô hiệu hóa tài khoản", f =>
        {
            f.HoSo.DongHoSo(f.QuanLy, "dong-hop-le", "DG01", "Tốt nghiệp");

            XacNhan(f.DocHoSo().TinhTrangQuyen == TrangThaiDocGia.DaDong,
                "Kho phải lưu trạng thái đã đóng.");

            var the = f.Kho.DaLuu<TheThuVien>().Single(x => x.MaThe == "THE01");
            XacNhan(the.TrangThai == TrangThaiThe.DaHuy,
                "Thẻ hiện hành phải được hủy và lưu lại.");

            var taiKhoan = f.Kho.DaLuu<TaiKhoan>()
                .Single(x => x.MaTaiKhoan == "TK-DG01");
            XacNhan(taiKhoan.TrangThai == TrangThaiTaiKhoan.VoHieuHoa,
                "Tài khoản độc giả phải được vô hiệu hóa và lưu lại.");
        });

        Chay("Dừng quyền lưu ngày rời trường và giữ nghĩa vụ mượn", f =>
        {
            var hanTraCu = f.TaoLuotMuon();

            f.HoSo.HanCheKhiBuocRoiTruong(
                f.QuanLy, "dung-quyen", "DG01", "Quyết định số 01");

            var docGia = f.DocHoSo();
            XacNhan(docGia.TinhTrangQuyen == TrangThaiDocGia.DungQuyen,
                "Kho phải lưu trạng thái dừng quyền.");
            XacNhan(docGia.NgayRoiTruong == f.DongHo.HomNay,
                "Phải lưu đúng ngày rời trường.");

            var dong = f.Kho.DaLuu<ChiTietMuon>().Single(x => x.MaDong == "DM01");
            XacNhan(dong.HanTra == hanTraCu,
                "Dừng quyền không được thay đổi hạn trả.");
            XacNhan(f.HoSo.XemNghiaVu(f.QuanLy, "DG01").SoCuonChuaHoanTat == 1,
                "Nghĩa vụ với cuốn đang mượn phải được giữ lại.");
        });

        Console.WriteLine(
            $"Kiểm thử HoSoService: {soCaDat} PASS, {soCaLoi} FAIL");

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
        public DongHoKiemThu DongHo { get; } = new();
        public PhienLamViec QuanLy { get; }
        public HoSoService HoSo { get; }

        public DuLieuKiemThu()
        {
            const string matKhau = "MatKhauTest01";

            Ghi(tx =>
            {
                tx.Kho<NhanVien>().Luu(
                    new NhanVien("NVQL01", "Quản lý kiểm thử", VaiTro.QuanLy));

                tx.Kho<TaiKhoan>().Luu(
                    new TaiKhoan("TKQL01", "quanly", matKhau, VaiTro.QuanLy,
                        maNhanVien: "NVQL01"));

                tx.Kho<DocGia>().Luu(
                    new SinhVien("DG01", "SV01", "Nguyễn An"));

                tx.Kho<TaiKhoan>().Luu(
                    new TaiKhoan("TK-DG01", "docgia01", matKhau, VaiTro.DocGia,
                        maDocGia: "DG01"));

                tx.Kho<TheThuVien>().Luu(
                    new TheThuVien("THE01", "DG01", DongHo.HomNay.AddMonths(-1)));
            });

            var taiKhoan = new TaiKhoanService(Kho);
            QuanLy = taiKhoan.DangNhap("quanly", matKhau);

            var dieuPhoi = new BoDieuPhoi(Kho, DongHo, taiKhoan);
            var hangCho = new HangChoService(dieuPhoi, new LichMoCua());
            HoSo = new HoSoService(dieuPhoi, hangCho);
        }

        public DocGia DocHoSo()
        {
            return Kho.DaLuu<DocGia>().Single(x => x.MaDocGia == "DG01");
        }

        public DateOnly TaoLuotMuon()
        {
            var ngayMuon = DongHo.HomNay.AddDays(-2);
            var hanTra = DongHo.HomNay.AddDays(10);

            Ghi(tx =>
            {
                tx.Kho<PhieuMuon>().Luu(
                    new PhieuMuon("PM01", "DG01", ngayMuon, "TKQL01", "muon-01"));

                tx.Kho<ChiTietMuon>().Luu(
                    new ChiTietMuon(
                        "DM01", "PM01", "CS01", ngayMuon, hanTra, LoaiDocGia.SinhVien));
            });

            return hanTra;
        }

        public void Ghi(Action<ITransactionManager> ghiDuLieu)
        {
            Kho.BatDau();

            try
            {
                ghiDuLieu(Kho);
                Kho.Commit();
            }
            catch
            {
                Kho.Rollback();
                throw;
            }
        }
    }

    private sealed class DongHoKiemThu : IDongHo
    {
        public DateTimeOffset BayGio { get; } =
            new(2026, 11, 28, 10, 0, 0, TimeSpan.FromHours(7));

        public DateOnly HomNay => DateOnly.FromDateTime(BayGio.DateTime);
    }
}