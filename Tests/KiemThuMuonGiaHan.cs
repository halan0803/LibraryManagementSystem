using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Infrastructure.Gateways;

namespace LibraryManagementSystem.Tests;

internal static class KiemThuMuonGiaHan
{
    internal static int RunAll()
    {
        var cacCa = new (string Ten, Action Ca)[]
        {
            ("UC03 mượn nhiều cuốn, gửi lặp và kiểm tra hạn mức", MuonNhieuCuon),
            ("UC03 hoàn tác khi cuốn thứ hai không hợp lệ", HoanTac),
            ("UC04 kiểm tra quyền, gia hạn một lần và đọc lại từ JSON", GiaHanMotLan),
            ("UC04 chặn gia hạn khi có hàng chờ", ChanKhiCoHangCho)
        };

        int soLoi = 0;
        foreach (var (ten, ca) in cacCa)
        {
            try
            {
                ca();
                Console.WriteLine($"PASS: {ten}");
            }
            catch (Exception loi)
            {
                soLoi++;
                Console.WriteLine($"FAIL: {ten} — {loi.Message}");
            }
        }

        Console.WriteLine($"Mượn và gia hạn: {cacCa.Length - soLoi}/{cacCa.Length} PASS");
        return soLoi;
    }

    private static void MuonNhieuCuon()
    {
        using var f = new DuLieu();
        string ma = MaMoi();
        var cuon = new[] { f.Chon("C2"), f.Chon("C3") };
        var phieu = f.App.MuonTra.LapPhieuMuon(f.ThuThu, ma, "THE2", cuon);
        Bang(phieu.MaPhieu, f.App.MuonTra.LapPhieuMuon(f.ThuThu, ma, "THE2", cuon).MaPhieu);
        Bang(2, phieu.MaDong.Length);
        Bang(f.App.Lich.CongNgayMoCua(f.Ngay, 14), phieu.HanTra);
        Bang(TrangThaiCuonSach.DangMuon, f.Doc<CuonSach>("C2").TrangThaiLuuThong);
        TuChoi("HAN_MUC", () => f.App.MuonTra.LapPhieuMuon(f.ThuThu, MaMoi(),
            "THE2", [f.Chon("C4"), f.Chon("C5")]));
    }

    private static void HoanTac()
    {
        using var f = new DuLieu();
        TuChoi("DANG_MUON", () => f.App.MuonTra.LapPhieuMuon(f.ThuThu, MaMoi(),
            "THE2", [f.Chon("C2"), f.Chon("C1")]));
        Bang(TrangThaiCuonSach.SanSang, f.Doc<CuonSach>("C2").TrangThaiLuuThong);
        Bang(1, f.DocGiaoDich(tx => tx.Kho<PhieuMuon>().LayTatCa().Count));
    }

    private static void GiaHanMotLan()
    {
        using var f = new DuLieu();
        string dong = f.App.MuonTra.LapPhieuMuon(f.ThuThu, MaMoi(), "THE2",
            [f.Chon("C2")]).MaDong.Single();
        var hanCu = f.Doc<ChiTietMuon>(dong).HanTra;
        TuChoi("CHU_HO_SO", () => f.App.GiaHan.GiaHan(f.DocGia(1), MaMoi(), dong, 5));
        var docGia = f.DocGia(2);
        string ma = MaMoi();
        var hanMoi = f.App.GiaHan.GiaHan(docGia, ma, dong, 5);
        Bang(f.App.Lich.CongNgayMoCua(hanCu, 5), hanMoi);
        Bang(hanMoi, f.App.GiaHan.GiaHan(docGia, ma, dong, 5));
        f.MoLai();
        Bang<DateOnly?>(hanCu, f.Doc<ChiTietMuon>(dong).HanTraTruocGiaHan);
        Bang(hanMoi, f.Doc<ChiTietMuon>(dong).HanTra);
        TuChoi("GIA_HAN", () => f.App.GiaHan.GiaHan(f.ThuThu, MaMoi(), dong, 3));
    }

    private static void ChanKhiCoHangCho()
    {
        using var f = new DuLieu();
        f.App.HangCho.DangKyCho(f.ThuThu, MaMoi(), "DG2", "C1");
        TuChoi("CO_HANG_CHO", () => f.App.GiaHan.GiaHan(f.DocGia(1), MaMoi(), "DM1", 3));
    }

    private static void Bang<T>(T mongDoi, T thucTe)
    {
        if (!EqualityComparer<T>.Default.Equals(mongDoi, thucTe))
            throw new InvalidOperationException($"Mong đợi {mongDoi}, thực tế {thucTe}.");
    }

    private static void TuChoi(string maLoi, Action thaoTac)
    {
        try
        {
            thaoTac();
        }
        catch (NghiepVuException loi) when (loi.MaLoi == maLoi)
        {
            return;
        }

        throw new InvalidOperationException($"Cần từ chối với mã lỗi {maLoi}.");
    }

    private sealed class DuLieu : IDisposable
    {
        private readonly string tep = Path.Combine(AppContext.BaseDirectory, $"kiem-thu-muon-{MaMoi()}.json");
        private readonly IDongHo dongHo = new DongHoThu();
        private readonly IDonViTruongGateway nhaTruong = new DonViTruongGiaLap([
            new("SV1", "Sinh viên 1", LoaiDocGia.SinhVien, true),
            new("SV2", "Sinh viên 2", LoaiDocGia.SinhVien, true)
        ]);

        internal NghiepVuMuon App { get; private set; }
        internal PhienLamViec ThuThu => App.TaiKhoan.DangNhap("thuthu", "Demo123!");
        internal DateOnly Ngay => dongHo.HomNay;

        internal DuLieu()
        {
            App = Mo();
            var tx = App.GiaoDich;
            tx.BatDau();
            try
            {
                tx.Kho<NhanVien>().Luu(new("NV1", "Thủ thư", VaiTro.ThuThu));
                tx.Kho<TaiKhoan>().Luu(new("TT", "thuthu", "Demo123!", VaiTro.ThuThu,
                    maNhanVien: "NV1"));
                for (int i = 1; i <= 2; i++)
                {
                    tx.Kho<DocGia>().Luu(new SinhVien($"DG{i}", $"SV{i}", $"Sinh viên {i}"));
                    tx.Kho<TheThuVien>().Luu(new($"THE{i}", $"DG{i}", Ngay));
                    tx.Kho<TaiKhoan>().Luu(new($"TK{i}", $"docgia{i}", "Demo123!", VaiTro.DocGia,
                        maDocGia: $"DG{i}"));
                }

                tx.Kho<TacGia>().Luu(new("TG1", "Tác giả"));
                tx.Kho<TheLoai>().Luu(new("TL1", "Khoa học"));
                tx.Kho<DauSach>().Luu(new("DS1", "Sách mẫu", "1", "Tiếng Việt", ["TG1"], "TL1"));
                for (int i = 1; i <= 5; i++)
                {
                    var cuon = new CuonSach($"C{i}", "DS1", 100000);
                    if (i == 1)
                        cuon.DanhDauMuon();
                    tx.Kho<CuonSach>().Luu(cuon);
                }

                tx.Kho<PhieuMuon>().Luu(new("PM1", "DG1", Ngay, "NV1", "DU_LIEU_MAU"));
                tx.Kho<ChiTietMuon>().Luu(new("DM1", "PM1", "C1", Ngay,
                    App.Lich.CongNgayMoCua(Ngay, 14), LoaiDocGia.SinhVien));
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                Dispose();
                throw;
            }
        }

        private NghiepVuMuon Mo() =>
            new(tep, new Infrastructure.Configuration.CauHinhThuVien(), dongHo, nhaTruong);

        internal void MoLai()
        {
            App.Dispose();
            App = Mo();
        }

        internal PhienLamViec DocGia(int so) => App.TaiKhoan.DangNhap($"docgia{so}", "Demo123!");
        internal CuonMuon Chon(string ma) => new(ma, Doc<CuonSach>(ma).PhienBanDuLieu);
        internal T Doc<T>(string ma) where T : class =>
            DocGiaoDich(tx => BatBuoc(tx.Kho<T>().LayTheoMa(ma), ma));

        internal T DocGiaoDich<T>(Func<ITransactionManager, T> doc)
        {
            var tx = App.GiaoDich;
            tx.BatDau();
            try { return doc(tx); }
            finally { tx.Rollback(); }
        }

        public void Dispose()
        {
            App.Dispose();
            File.Delete(tep);
            File.Delete(tep + ".bak");
            File.Delete(tep + ".lock");
        }
    }

    private sealed class DongHoThu : IDongHo
    {
        public DateTimeOffset BayGio { get; } = new(2026, 11, 23, 10, 0, 0, TimeSpan.FromHours(7));
    }
}
