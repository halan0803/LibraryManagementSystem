using System.Text.Json;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Enums;
using LibraryManagementSystem.Domain.Exceptions;

namespace LibraryManagementSystem.Tests;

public static class KiemThuTheService
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

        Chay("Cấp mới và lưu thẻ hiện hành", f =>
        {
            var the = f.CapThe();
            var daLuu = f.DocThe(the.MaThe);

            XacNhan(daLuu.TrangThai == TrangThaiThe.HienHanh,
                "Thẻ vừa cấp phải là thẻ hiện hành.");
            XacNhan(daLuu.NgayHetHan == new DateOnly(2026, 7, 1),
                "Thẻ cấp ngày 01/01 phải hết hạn ngày 01/07.");
            XacNhan(f.Kho.DaLuu<DocGia>().Single().MaDocGia == daLuu.MaDocGia,
                "Thẻ phải thuộc đúng hồ sơ độc giả đã lưu.");
        });

        Chay("Tên không khớp dữ liệu trường thì không cấp thẻ", f =>
        {
            MongDoiLoi("XAC_MINH", () => f.Service.CapThe(
                f.Phien, "cap-sai", "SV01", LoaiDocGia.SinhVien, "Tên khác"));

            XacNhan(f.Kho.DaLuu<TheThuVien>().Count == 0
                && f.Kho.DaLuu<DocGia>().Count == 0,
                "Yêu cầu bị từ chối không được lưu thẻ hoặc hồ sơ.");
        });

        Chay("Cấp lại khóa thẻ cũ và giữ nguyên hạn", f =>
        {
            var theCu = f.CapThe();

            var theMoi = f.Service.CapLaiThe(
                f.Phien, "cap-lai", theCu.MaDocGia, theCu.MaThe, "Mất thẻ");

            var cuDaLuu = f.DocThe(theCu.MaThe);
            var moiDaLuu = f.DocThe(theMoi.MaThe);

            XacNhan(cuDaLuu.TrangThai == TrangThaiThe.DaThayThe,
                "Phải lưu trạng thái đã thay thế của thẻ cũ.");
            XacNhan(moiDaLuu.MaThe != cuDaLuu.MaThe
                && moiDaLuu.MaTheTruoc == cuDaLuu.MaThe
                && moiDaLuu.MaDocGia == cuDaLuu.MaDocGia
                && moiDaLuu.NgayHetHan == cuDaLuu.NgayHetHan,
                "Thẻ mới phải liên kết đúng thẻ cũ và giữ hạn cũ.");
            XacNhan(f.Kho.DaLuu<TheThuVien>()
                .Count(t => t.TrangThai == TrangThaiThe.HienHanh) == 1,
                "Chỉ được có một thẻ hiện hành.");
        });

        Chay("Gia hạn hợp lệ lưu hạn mới tính từ hạn cũ", f =>
        {
            var the = f.CapThe();
            f.DongHo.DatNgay(2026, 6, 15);

            f.Service.GiaHanThe(f.Phien, "gia-han", the.MaThe);

            XacNhan(f.DocThe(the.MaThe).NgayHetHan == new DateOnly(2027, 1, 1),
                "Phải lưu hạn mới là 01/01/2027.");
        });

        Chay("Còn nợ thì từ chối gia hạn và giữ hạn cũ", f =>
        {
            var the = f.CapThe();
            f.DongHo.DatNgay(2026, 6, 15);

            f.Kho.BatDau();
            f.Kho.Kho<KhoanThu>().Luu(
                new KhoanThu("KT01", the.MaDocGia, "DM01", 5_000m));
            f.Kho.Commit();

            MongDoiLoi("CON_NO", () =>
                f.Service.GiaHanThe(f.Phien, "gia-han-co-no", the.MaThe));

            XacNhan(f.DocThe(the.MaThe).NgayHetHan == the.NgayHetHan,
                "Khi còn nợ, hạn đã lưu phải giữ nguyên.");
        });

        Chay("Khôi phục thẻ hết hạn tính từ ngày xử lý", f =>
        {
            var the = f.CapThe();
            f.DongHo.DatNgay(2026, 7, 10);

            f.Service.KhoiPhucThe(f.Phien, "khoi-phuc", the.MaThe);

            var daLuu = f.DocThe(the.MaThe);
            XacNhan(daLuu.NgayHieuLuc == new DateOnly(2026, 7, 10)
                && daLuu.NgayHetHan == new DateOnly(2027, 1, 10),
                "Phải lưu ngày hiệu lực và hạn mới theo ngày khôi phục.");
        });

        Chay("Gửi lại cùng mã yêu cầu không gia hạn lần hai", f =>
        {
            var the = f.CapThe();
            f.DongHo.DatNgay(2026, 6, 15);

            var lanDau = f.Service.GiaHanThe(f.Phien, "gui-lap", the.MaThe);
            var lanHai = f.Service.GiaHanThe(f.Phien, "gui-lap", the.MaThe);

            XacNhan(lanDau.NgayHetHan == new DateOnly(2027, 1, 1)
                && lanHai.NgayHetHan == lanDau.NgayHetHan
                && f.DocThe(the.MaThe).NgayHetHan == lanDau.NgayHetHan,
                "Yêu cầu lặp phải trả lại kết quả đã lưu.");
        });

        Console.WriteLine($"Kiểm thử TheService: {soCaDat} PASS, {soCaLoi} FAIL");
        return soCaLoi;
    }

    private static void XacNhan(bool dieuKien, string thongBao)
    {
        if (!dieuKien)
            throw new InvalidOperationException(thongBao);
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

        throw new InvalidOperationException($"Mong lỗi {maLoi} nhưng lệnh thành công.");
    }

    private sealed class DuLieuKiemThu
    {
        public KhoKiemThu Kho { get; } = new();
        public DongHoKiemThu DongHo { get; } = new();
        public PhienLamViec Phien { get; }
        public TheService Service { get; }

        public DuLieuKiemThu()
        {
            DongHo.DatNgay(2026, 1, 1);

            Kho.BatDau();
            Kho.Kho<NhanVien>().Luu(new NhanVien("NV01", "Thủ thư", VaiTro.ThuThu));
            Kho.Kho<TaiKhoan>().Luu(new TaiKhoan(
                "TK01", "thuthu", "MatKhauTest01", VaiTro.ThuThu,
                maNhanVien: "NV01"));
            Kho.Commit();

            var taiKhoan = new TaiKhoanService(Kho);
            Phien = taiKhoan.DangNhap("thuthu", "MatKhauTest01");
            Service = new TheService(
                new BoDieuPhoi(Kho, DongHo, taiKhoan), new TruongKiemThu());
        }

        public TheThuVien CapThe() => Service.CapThe(
            Phien, "cap-moi", "SV01", LoaiDocGia.SinhVien, "Nguyễn An");

        public TheThuVien DocThe(string maThe) =>
            Kho.DaLuu<TheThuVien>().Single(t => t.MaThe == maThe);
    }

    private sealed class DongHoKiemThu : IDongHo
    {
        public DateTimeOffset BayGio { get; private set; }
        public DateOnly HomNay => DateOnly.FromDateTime(BayGio.DateTime);

        public void DatNgay(int nam, int thang, int ngay)
        {
            BayGio = new DateTimeOffset(nam, thang, ngay, 9, 0, 0, TimeSpan.FromHours(7));
        }
    }

    private sealed class TruongKiemThu : IDonViTruongGateway
    {
        public ThongTinTruong? TinhTrang(string maTruong) =>
            maTruong == "SV01"
                ? new ThongTinTruong("SV01", "Nguyễn An", LoaiDocGia.SinhVien, true)
                : null;

        public bool DuDoiTuong(string maTruong, LoaiDocGia loai) =>
            TinhTrang(maTruong) is { ConThuocTruong: true } thongTin
            && thongTin.LoaiDocGia == loai;
    }

    private sealed class KhoKiemThu : ITransactionManager
    {
        private sealed record BanGhi(Type Kieu, string Json);

        private Dictionary<(Type, string), BanGhi> daLuu = new();
        private Dictionary<(Type, string), BanGhi>? dangSua;

        public bool DangGiaoDich => dangSua is not null;

        public void BatDau()
        {
            XacNhan(!DangGiaoDich, "Kho thử không hỗ trợ giao dịch lồng nhau.");
            dangSua = new(daLuu);
        }

        public void Commit()
        {
            daLuu = DuLieuGiaoDich();
            dangSua = null;
        }

        public void Rollback() => dangSua = null;

        public IRepository<T> Kho<T>() where T : class =>
            new RepositoryKiemThu<T>(DuLieuGiaoDich());

        public IReadOnlyList<T> DaLuu<T>() where T : class =>
            daLuu.Where(x => x.Key.Item1 == typeof(T))
                .Select(x => GiaiMa<T>(x.Value))
                .ToArray();

        private Dictionary<(Type, string), BanGhi> DuLieuGiaoDich() =>
            dangSua ?? throw new InvalidOperationException("Chưa mở giao dịch.");

        private static T GiaiMa<T>(BanGhi banGhi) where T : class =>
            (T)(JsonSerializer.Deserialize(banGhi.Json, banGhi.Kieu)
                ?? throw new InvalidOperationException("Không đọc được bản ghi kiểm thử."));

        private sealed class RepositoryKiemThu<T>(
            Dictionary<(Type, string), BanGhi> duLieu) : IRepository<T>
            where T : class
        {
            public T? LayTheoMa(string ma) =>
                duLieu.TryGetValue((typeof(T), ma), out var banGhi)
                    ? GiaiMa<T>(banGhi)
                    : null;

            public IReadOnlyList<T> LayTatCa() =>
                duLieu.Where(x => x.Key.Item1 == typeof(T))
                    .Select(x => GiaiMa<T>(x.Value))
                    .ToArray();

            public void Luu(T doiTuong)
            {
                XacNhan(duLieu.TryAdd(Khoa(doiTuong), TaoBanGhi(doiTuong)),
                    "Trùng khóa trong kho kiểm thử.");
            }

            public void CapNhat(T doiTuong)
            {
                var khoa = Khoa(doiTuong);
                XacNhan(duLieu.ContainsKey(khoa), "Không có bản ghi để cập nhật.");
                duLieu[khoa] = TaoBanGhi(doiTuong);
            }

            private static BanGhi TaoBanGhi(T doiTuong)
            {
                Type kieu = doiTuong.GetType();
                return new BanGhi(kieu, JsonSerializer.Serialize(doiTuong, kieu));
            }

            private static (Type, string) Khoa(T doiTuong)
            {
                string ma = doiTuong switch
                {
                    TheThuVien the => the.MaThe,
                    DocGia docGia => docGia.MaDocGia,
                    TaiKhoan taiKhoan => taiKhoan.MaTaiKhoan,
                    NhanVien nhanVien => nhanVien.MaNhanVien,
                    KhoanThu khoanThu => khoanThu.MaKhoan,
                    YeuCauDaXuLy yeuCau => yeuCau.MaYeuCau,
                    _ => throw new InvalidOperationException(
                        $"Kho kiểm thử chưa hỗ trợ {typeof(T).Name}.")
                };

                return (typeof(T), ma);
            }
        }
    }
}