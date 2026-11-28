using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Domain.Services;

namespace LibraryManagementSystem.Tests;

public static class KiemThuHangChoService
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

        Chay("Đăng ký chờ lưu đúng độc giả và cuốn sách", f =>
        {
            var dangKy = f.DangKy("DG01", "CS01", "dang-ky-01");
            var daLuu = f.DocDangKy(dangKy.MaDangKy);

            XacNhan(
                daLuu.MaDocGia == "DG01"
                && daLuu.MaCuon == "CS01"
                && daLuu.TrangThai == TrangThaiDangKyCho.DangCho,
                "Phải lưu đúng đăng ký ở trạng thái đang chờ.");

            XacNhan(f.Kho.DaLuu<DotGiuCho>().Count == 0,
                "Đăng ký chờ chưa được cấp quyền nhận sách.");
        });

        Chay("Mỗi độc giả chỉ có một đăng ký hiệu lực", f =>
        {
            f.DangKy("DG01", "CS01", "dang-ky-01");

            MongDoiLoi("DA_CHO", () =>
                f.DangKy("DG01", "CS02", "dang-ky-02"));

            XacNhan(f.Kho.DaLuu<DangKyCho>().Count == 1,
                "Đăng ký cho cuốn khác cũng phải bị từ chối.");
        });

        Chay("Cuốn sẵn sàng phải được mượn trực tiếp", f =>
        {
            MongDoiLoi("MUON_TRUC_TIEP", () =>
                f.DangKy("DG01", "CS03", "cho-cuon-san-sang"));

            XacNhan(f.Kho.DaLuu<DangKyCho>().Count == 0,
                "Không được lưu đăng ký cho cuốn đang sẵn sàng.");
        });

        Chay("Hủy đăng ký rồi được đăng ký cho cuốn khác", f =>
        {
            var dangKy = f.DangKy("DG01", "CS01", "dang-ky-01");

            f.HangCho.HuyDangKy(
                f.Phien, "huy-01", dangKy.MaDangKy, "Không còn nhu cầu");

            XacNhan(
                f.DocDangKy(dangKy.MaDangKy).TrangThai
                    == TrangThaiDangKyCho.DaHuy,
                "Phải lưu trạng thái đã hủy.");

            var dangKyMoi = f.DangKy("DG01", "CS02", "dang-ky-02");

            XacNhan(f.DocDangKy(dangKyMoi.MaDangKy).ConHieuLuc(),
                "Độc giả phải đăng ký lại được sau khi hủy.");
        });

        Chay("Xét đúng thứ tự và chỉ cấp một quyền nhận", f =>
        {
            var dau = f.DangKy("DG01", "CS01", "dang-ky-01");
            var hai = f.DangKy("DG02", "CS01", "dang-ky-02");
            var ba = f.DangKy("DG03", "CS01", "dang-ky-03");

            var dot = f.NhanTraVaXet();

            XacNhan(dot.DanhSachLuot.Count == 2,
                "Một đợt chỉ chọn tối đa hai lượt.");

            XacNhan(
                dot.DanhSachLuot[0].MaDangKy == dau.MaDangKy
                && dot.DanhSachLuot[1].MaDangKy == hai.MaDangKy,
                "Hai lượt phải theo đúng thứ tự đăng ký.");

            XacNhan(
                dot.DanhSachLuot[0].TrangThai == TrangThaiLuotGiuCho.DangCoQuyen
                && dot.DanhSachLuot[1].HanNhan is null
                && dot.DanhSachLuot.Count(
                    x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen) == 1,
                "Chỉ lượt đầu có quyền nhận và hạn nhận.");

            XacNhan(
                f.DocDangKy(ba.MaDangKy).TrangThai == TrangThaiDangKyCho.DangCho,
                "Người thứ ba vẫn ở hàng chờ.");
        });

        Chay("Cấp lượt thứ Bảy, hạn nhận là 17 giờ thứ Ba", f =>
        {
            f.DangKy("DG01", "CS01", "dang-ky-01");

            var dot = f.NhanTraVaXet();
            var hanMongDoi = new DateTimeOffset(
                2026, 12, 1, 17, 0, 0, TimeSpan.FromHours(7));

            XacNhan(dot.DanhSachLuot[0].HanNhan == hanMongDoi,
                "Phải bỏ qua Chủ nhật khi tính hai ngày mở cửa.");
        });

        Chay("Đúng giờ hết hạn thì chuyển quyền sang lượt hai", f =>
        {
            f.DangKy("DG01", "CS01", "dang-ky-01");
            var dangKyHai = f.DangKy("DG02", "CS01", "dang-ky-02");
            var dot = f.NhanTraVaXet();

            f.DongHo.BayGio = dot.DanhSachLuot[0].HanNhan
                ?? throw new InvalidOperationException("Lượt đầu chưa có hạn nhận.");

            f.HangCho.XuLyHetLuot(f.Phien, "xu-ly-het-luot");

            var daLuu = f.DocDot(dot.MaDot);

            XacNhan(
                daLuu.DanhSachLuot[0].TrangThai == TrangThaiLuotGiuCho.HetLuot,
                "Lượt đầu phải hết quyền tại đúng giờ hết hạn.");

            XacNhan(
                daLuu.DanhSachLuot[1].MaDangKy == dangKyHai.MaDangKy
                && daLuu.DanhSachLuot[1].TrangThai == TrangThaiLuotGiuCho.DangCoQuyen
                && f.DocDangKy(dangKyHai.MaDangKy).TrangThai
                    == TrangThaiDangKyCho.DangCoQuyenNhan,
                "Phải lưu quyền nhận cho người thứ hai.");

            var hanLuotHai = new DateTimeOffset(
                2026, 12, 3, 17, 0, 0, TimeSpan.FromHours(7));

            XacNhan(daLuu.DanhSachLuot[1].HanNhan == hanLuotHai,
                "Lượt hai phải được tính hạn từ ngày cấp lượt của mình.");
        });

        Console.WriteLine(
            $"Kiểm thử HangChoService: {soCaDat} PASS, {soCaLoi} FAIL");

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
        public PhienLamViec Phien { get; }
        public HangChoService HangCho { get; }

        public DuLieuKiemThu()
        {
            const string matKhau = "MatKhauTest01";
            var lich = new LichMoCua();
            var ngayMuon = new DateOnly(2026, 11, 20);

            Ghi(tx =>
            {
                tx.Kho<NhanVien>().Luu(
                    new NhanVien("NV01", "Thủ thư kiểm thử", VaiTro.ThuThu));

                tx.Kho<TaiKhoan>().Luu(
                    new TaiKhoan(
                        "TK01", "thuthu", matKhau, VaiTro.ThuThu,
                        maNhanVien: "NV01"));

                for (int i = 1; i <= 4; i++)
                {
                    string maDocGia = $"DG{i:00}";

                    tx.Kho<DocGia>().Luu(
                        new SinhVien(maDocGia, $"SV{i:00}", $"Sinh viên {i}"));

                    tx.Kho<TheThuVien>().Luu(
                        new TheThuVien($"THE{i:00}", maDocGia, ngayMuon));
                }

                var cuonMot = new CuonSach("CS01", "DS01", 50_000m);
                var cuonHai = new CuonSach("CS02", "DS01", 50_000m);

                cuonMot.DanhDauMuon();
                cuonHai.DanhDauMuon();

                tx.Kho<CuonSach>().Luu(cuonMot);
                tx.Kho<CuonSach>().Luu(cuonHai);
                tx.Kho<CuonSach>().Luu(
                    new CuonSach("CS03", "DS01", 50_000m));

                tx.Kho<PhieuMuon>().Luu(
                    new PhieuMuon(
                        "PM01", "DG04", ngayMuon, "TK01", "muon-nguon"));

                tx.Kho<ChiTietMuon>().Luu(
                    new ChiTietMuon(
                        "DM01", "PM01", "CS01", ngayMuon,
                        lich.CongNgayMoCua(ngayMuon, 14), LoaiDocGia.SinhVien));

                tx.Kho<ChiTietMuon>().Luu(
                    new ChiTietMuon(
                        "DM02", "PM01", "CS02", ngayMuon,
                        lich.CongNgayMoCua(ngayMuon, 14), LoaiDocGia.SinhVien));
            });

            var taiKhoan = new TaiKhoanService(Kho);
            Phien = taiKhoan.DangNhap("thuthu", matKhau);

            var dieuPhoi = new BoDieuPhoi(Kho, DongHo, taiKhoan);
            HangCho = new HangChoService(dieuPhoi, lich);
        }

        public DangKyCho DangKy(string maDocGia, string maCuon, string maYeuCau)
        {
            return HangCho.DangKyCho(Phien, maYeuCau, maDocGia, maCuon);
        }

        public DangKyCho DocDangKy(string maDangKy)
        {
            return Kho.DaLuu<DangKyCho>()
                .Single(x => x.MaDangKy == maDangKy);
        }

        public DotGiuCho DocDot(string maDot)
        {
            return Kho.DaLuu<DotGiuCho>().Single(x => x.MaDot == maDot);
        }

        public DotGiuCho NhanTraVaXet()
        {
            // Chuẩn bị kết quả nhận trả để kiểm thử riêng bước xét hàng chờ.
            Ghi(tx =>
            {
                var khoDong = tx.Kho<ChiTietMuon>();
                var dong = khoDong.LayTheoMa("DM01")!;
                dong.NhanTra(DongHo.HomNay, "Tốt");
                khoDong.CapNhat(dong);

                var khoCuon = tx.Kho<CuonSach>();
                var cuon = khoCuon.LayTheoMa("CS01")!;
                cuon.NhanTra(TinhTrangSach.Tot);
                khoCuon.CapNhat(cuon);
            });

            var dot = HangCho.XetDotGiuCho(
                Phien, "xet-01", "CS01", "DM01", "tra-CS01-DM01");

            return DocDot(dot?.MaDot
                ?? throw new InvalidOperationException("Phải tạo một đợt giữ chỗ."));
        }

        private void Ghi(Action<ITransactionManager> ghiDuLieu)
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
        public DateTimeOffset BayGio { get; set; } =
            new(2026, 11, 28, 10, 0, 0, TimeSpan.FromHours(7));

        public DateOnly HomNay => DateOnly.FromDateTime(BayGio.DateTime);
    }
}