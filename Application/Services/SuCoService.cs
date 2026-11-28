using System.Text.Json;
using LibraryManagementSystem.Domain.Strategies;

namespace LibraryManagementSystem.Application.Services;

public sealed record NghiaVuMat(string MaSuCo, string? KhoanPhat, decimal PhatConNo, string? KhoanBoiThuong, decimal BoiThuongConNo);

public sealed class SuCoService(BoDieuPhoi b, HangChoService hangCho, ILichMoCua lich, IChinhSachPhat phat)
{
    public string BaoMat(PhienLamViec p, string yeuCau, string maDong, string minhChung) =>
        b.Lenh(p, yeuCau, nameof(BaoMat), new { maDong, minhChung }, () => ThuThu(p), () =>
        {
            var dong = b.Lay<ChiTietMuon>(maDong);
            YeuCau(dong.TrangThai == TrangThaiMuon.DangMuon, "BAO_MAT", "Dòng không đang mượn.");
            var cuon = b.Lay<CuonSach>(dong.MaCuon);
            var ngay = b.DongHo.HomNay;
            var suCo = new BienBanSuCo(MaMoi(), maDong, LoaiSuCo.Mat, ngay, Chuoi(minhChung, nameof(minhChung)));
            dong.BaoMat(ngay);
            cuon.ChuyenTrangThai(TrangThaiCuonSach.Mat);
            hangCho.TamDungTrongGiaoDich(p, cuon.MaCuon, "Cuốn đã báo mất");
            b.Them(suCo);
            return suCo.MaSuCo;
        });

    public string DuyetDinhGia(PhienLamViec p, string yeuCau, string maCuon, decimal gia, string canCu) =>
        b.Lenh(p, yeuCau, nameof(DuyetDinhGia), new { maCuon, gia, canCu }, () => QuanLy(p), () =>
        {
            var cuon = b.Lay<CuonSach>(maCuon);
            YeuCau(gia > 0 && cuon.GiaBia is not > 0, "DINH_GIA", "Chỉ định giá dương cho cuốn thiếu giá bìa.");
            var duyet = TaoPheDuyet(p, LoaiPheDuyet.DinhGia, nameof(CuonSach), maCuon, canCu);
            cuon.DinhGia(gia, duyet.MaPheDuyet);
            return duyet.MaPheDuyet;
        });

    public string? ChotBoiThuong(PhienLamViec p, string yeuCau, string maSuCo, string canCu,
        bool hongDaCoKhiGiao = false) =>
        b.Lenh<string?>(p, yeuCau, nameof(ChotBoiThuong), new { maSuCo, canCu, hongDaCoKhiGiao }, () => ThuThu(p), () =>
        {
            canCu = Chuoi(canCu, nameof(canCu));
            var suCo = b.Lay<BienBanSuCo>(maSuCo);
            YeuCau(suCo.TrangThai == TrangThaiSuCo.ChoXuLy, "SU_CO", "Sự cố đã được xử lý.");
            var dong = b.Lay<ChiTietMuon>(suCo.MaDongMuon);
            var cuon = b.Lay<CuonSach>(dong.MaCuon);
            YeuCau(!hongDaCoKhiGiao || suCo.Loai == LoaiSuCo.Hong, "SU_CO", "Chỉ loại trừ hư hỏng đã có khi giao.");
            var tyLe = hongDaCoKhiGiao ? 0m : suCo.Loai == LoaiSuCo.Mat || suCo.TinhTrang == TinhTrangSach.HuNang ? 1m : 0.5m;
            if (tyLe == 0)
            {
                suCo.Chot(null, 0, 0, canCu);
                ThanhToanService.CapNhatDongDaNhan(b, dong, b.DongHo.HomNay);
                return null;
            }
            IChinhSachDinhGia chinhSach = cuon.GiaBia is > 0 ? new DinhGiaTheoGiaBia() : new DinhGiaThuVien();
            var gia = chinhSach.GiaTriSach(cuon);
            if (cuon.GiaBia is not > 0)
                KiemTraDuyet(cuon.MaPheDuyetGia!, LoaiPheDuyet.DinhGia, nameof(CuonSach), cuon.MaCuon);
            var docGia = b.Lay<PhieuMuon>(dong.MaPhieu).MaDocGia;
            var khoan = new KhoanThu(MaMoi(), docGia, dong.MaDong, gia * tyLe, loai: LoaiKhoanThu.BoiThuong);
            b.Them(khoan);
            suCo.Chot(khoan.MaKhoan, gia, tyLe, canCu);
            return khoan.MaKhoan;
        });

    public NghiaVuMat ChotPhatMat(PhienLamViec p, string yeuCau, string maSuCo) =>
        b.Lenh(p, yeuCau, nameof(ChotPhatMat), new { maSuCo }, () => ThuThu(p), () =>
        {
            var suCo = b.Lay<BienBanSuCo>(maSuCo);
            var dong = DongMat(suCo);
            var phaiThu = phat.TinhPhat(dong.TinhNgayTre(b.DongHo.HomNay, lich));
            var khoan = KhoanPhat(dong);
            if (khoan is null && phaiThu > 0)
            {
                khoan = new KhoanThu(MaMoi(), b.Lay<PhieuMuon>(dong.MaPhieu).MaDocGia, dong.MaDong, phaiThu);
                b.Them(khoan);
            }
            khoan?.ChotLai(phaiThu);
            var boiThuong = suCo.MaKhoanBoiThuong is null ? null : b.Lay<KhoanThu>(suCo.MaKhoanBoiThuong);
            return new NghiaVuMat(maSuCo, khoan?.MaKhoan, khoan?.SoTienConNo() ?? 0,
                boiThuong?.MaKhoan, boiThuong?.SoTienConNo() ?? 0);
        });

    public string TatToanMat(PhienLamViec p, string yeuCau, string maSuCo) =>
        b.Lenh(p, yeuCau, nameof(TatToanMat), new { maSuCo }, () => ThuThu(p), () =>
        {
            var suCo = b.Lay<BienBanSuCo>(maSuCo);
            var dong = b.Lay<ChiTietMuon>(suCo.MaDongMuon);
            if (suCo.Loai == LoaiSuCo.Mat && !suCo.DaTimLai && suCo.TrangThai == TrangThaiSuCo.HoanTat)
                return dong.MaDong;
            DongMat(suCo);
            YeuCau(ThuTatToanMat(b, lich, phat, dong), "CHUA_TAT_TOAN",
                "Cần hoàn tất bồi thường và tiền phạt đến ngày tất toán.");
            return dong.MaDong;
        });

    public string DuyetBanThayThe(PhienLamViec p, string yeuCau, string maSuCo, string dauSachMoi, string canCu) =>
        b.Lenh(p, yeuCau, nameof(DuyetBanThayThe), new { maSuCo, dauSachMoi, canCu }, () => QuanLy(p), () =>
        {
            var suCo = b.Lay<BienBanSuCo>(maSuCo);
            KiemTraBanThayThe(suCo, dauSachMoi);
            var duyet = TaoPheDuyet(p, LoaiPheDuyet.BanThayThe, nameof(BienBanSuCo), maSuCo, canCu);
            duyet.GanBanThayThe(dauSachMoi);
            return duyet.MaPheDuyet;
        });

    public string DuyetThanhLy(PhienLamViec p, string yeuCau, string maCuon, string canCu) =>
        b.Lenh(p, yeuCau, nameof(DuyetThanhLy), new { maCuon, canCu }, () => QuanLy(p), () =>
        {
            var cuon = b.Lay<CuonSach>(maCuon);
            YeuCau(cuon.TinhTrangVatLy == TinhTrangSach.HuNang && cuon.TrangThaiLuuThong == TrangThaiCuonSach.DangSua,
                "THANH_LY", "Chỉ duyệt thanh lý cuốn hư nặng đã tiếp nhận.");
            return TaoPheDuyet(p, LoaiPheDuyet.ThanhLy, nameof(CuonSach), maCuon, canCu).MaPheDuyet;
        });

    public string ThanhLy(PhienLamViec p, string yeuCau, string maCuon, string maDuyet, string lyDo) =>
        b.Lenh(p, yeuCau, nameof(ThanhLy), new { maCuon, maDuyet, lyDo }, () => ThuThu(p), () =>
        {
            var cuon = b.Lay<CuonSach>(maCuon);
            KiemTraDuyet(maDuyet, LoaiPheDuyet.ThanhLy, nameof(CuonSach), maCuon);
            return GhiThanhLy(p, cuon, maDuyet, lyDo, null);
        });

    public string NhanBanThayThe(PhienLamViec p, string yeuCau, string maSuCo, string maDuyet,
        string maCuonMoi, string dauSachMoi, string maKe, TinhTrangSach tinhTrang, string? duyetThanhLy = null) =>
        b.Lenh(p, yeuCau, nameof(NhanBanThayThe), new
        {
            maSuCo, maDuyet, maCuonMoi, dauSachMoi, maKe, tinhTrang, duyetThanhLy
        }, () => ThuThu(p), () =>
        {
            var suCo = b.Lay<BienBanSuCo>(maSuCo);
            var cu = KiemTraBanThayThe(suCo, dauSachMoi);
            var duyet = KiemTraDuyet(maDuyet, LoaiPheDuyet.BanThayThe, nameof(BienBanSuCo), maSuCo);
            YeuCau(duyet.MaDauSachThayThe == dauSachMoi, "BAN_MOI", "Đầu sách khác phương án đã duyệt.");
            YeuCau(b.Lay<KeSach>(maKe).DangSuDung && tinhTrang is TinhTrangSach.Tot or TinhTrangSach.HaoMon,
                "BAN_MOI", "Bản mới phải dùng được và có kệ hợp lệ.");
            var khoan = b.Lay<KhoanThu>(suCo.MaKhoanBoiThuong!);
            khoan.TatToanBangSach();
            if (suCo.Loai == LoaiSuCo.Hong && cu.TrangThaiLuuThong != TrangThaiCuonSach.DaThanhLy)
            {
                var ma = Chuoi(duyetThanhLy, nameof(duyetThanhLy));
                KiemTraDuyet(ma, LoaiPheDuyet.ThanhLy, nameof(CuonSach), cu.MaCuon);
                GhiThanhLy(p, cu, ma, "Nhận bản thay thế cho cuốn hư nặng", maSuCo);
            }
            var moi = new CuonSach(maCuonMoi, dauSachMoi, b.Lay<DauSach>(dauSachMoi).GiaBia, tinhTrang);
            moi.ChuyenKe(maKe, null);
            b.Them(moi);
            var bienNhan = new BienNhanThayThe(MaMoi(), cu.MaCuon, moi.MaCuon, maSuCo, maDuyet,
                b.TaiKhoan.KiemTraQuyen(p)!.MaNhanVien!, b.DongHo.HomNay);
            b.Them(bienNhan);
            hangCho.ChuyenBanTrongGiaoDich(p, cu.MaCuon, moi.MaCuon, maSuCo);
            var dong = b.Lay<ChiTietMuon>(suCo.MaDongMuon);
            ThanhToanService.CapNhatDongDaNhan(b, dong, b.DongHo.HomNay);
            ThuTatToanMat(b, lich, phat, dong);
            return bienNhan.MaBienNhan;
        });

    internal static bool ThuTatToanMat(BoDieuPhoi b, ILichMoCua lich, IChinhSachPhat phat, ChiTietMuon dong)
    {
        if (dong.TrangThai != TrangThaiMuon.MatChoXuLy)
            return false;
        var suCo = b.TatCa<BienBanSuCo>().Single(x => x.MaDongMuon == dong.MaDong && x.Loai == LoaiSuCo.Mat && !x.DaTimLai);
        var boiThuong = suCo.MaKhoanBoiThuong is null ? null : b.Lay<KhoanThu>(suCo.MaKhoanBoiThuong);
        if (boiThuong?.TrangThai is not (TrangThaiKhoanThu.DaThuDu or TrangThaiKhoanThu.DaTatToanBangSach))
            return false;
        var ngay = b.DongHo.HomNay;
        var khoan = b.TatCa<KhoanThu>().SingleOrDefault(x => x.MaDongMuon == dong.MaDong && x.Loai == LoaiKhoanThu.PhatQuaHan);
        var phaiThu = phat.TinhPhat(dong.TinhNgayTre(ngay, lich));
        if ((khoan?.SoTienDaThu ?? 0) != phaiThu || khoan?.TrangThai == TrangThaiKhoanThu.ChoDoiSoat)
            return false;
        khoan?.ChotLai(phaiThu);
        dong.ChotNgayTatToanMat(ngay);
        dong.KetThucNghiaVu(ngay);
        GhiSoHuuSauTatToanMat(b, dong.MaCuon);
        suCo.HoanTat();
        return true;
    }

    private static void GhiSoHuuSauTatToanMat(BoDieuPhoi b, string maCuon)
    {
        var cuon = b.Lay<CuonSach>(maCuon);
        YeuCau(cuon.TrangThaiLuuThong == TrangThaiCuonSach.Mat && cuon.QuyenSoHuu == QuyenSoHuuSach.ThuVien,
            "BAO_MAT", "Chỉ chuyển sở hữu cuốn đã tất toán mất.");

        // Lớp CuonSach cho phép nạp các thuộc tính này qua hợp đồng JSON.
        var duLieu = JsonSerializer.SerializeToNode(cuon)!.AsObject();
        duLieu[nameof(CuonSach.QuyenSoHuu)] = (int)QuyenSoHuuSach.NguoiDoc;
        duLieu[nameof(CuonSach.MaKe)] = null;
        duLieu[nameof(CuonSach.ViTriChiTiet)] = null;
        duLieu[nameof(CuonSach.PhienBanDuLieu)] = cuon.PhienBanDuLieu + 1;
        var cuonDaTatToan = duLieu.Deserialize<CuonSach>()!;
        b.GiaoDich.Kho<CuonSach>().CapNhat(cuonDaTatToan);
    }

    private KhoanThu? KhoanPhat(ChiTietMuon dong) => b.TatCa<KhoanThu>()
        .SingleOrDefault(x => x.MaDongMuon == dong.MaDong && x.Loai == LoaiKhoanThu.PhatQuaHan);

    private ChiTietMuon DongMat(BienBanSuCo suCo)
    {
        var dong = b.Lay<ChiTietMuon>(suCo.MaDongMuon);
        YeuCau(suCo.Loai == LoaiSuCo.Mat && !suCo.DaTimLai && dong.TrangThai == TrangThaiMuon.MatChoXuLy,
            "SU_CO_MAT", "Nghĩa vụ mất không còn hiệu lực.");
        return dong;
    }

    private CuonSach KiemTraBanThayThe(BienBanSuCo suCo, string maDauSach)
    {
        YeuCau((suCo.Loai == LoaiSuCo.Mat || suCo.TinhTrang == TinhTrangSach.HuNang) &&
            suCo.MaKhoanBoiThuong is not null && suCo.TrangThai == TrangThaiSuCo.DangGiaiQuyet,
            "BAN_MOI", "Chỉ mất hoặc hư nặng còn phải bồi thường được đổi bản.");
        var cu = b.Lay<CuonSach>(b.Lay<ChiTietMuon>(suCo.MaDongMuon).MaCuon);
        var dauCu = b.Lay<DauSach>(cu.MaDauSach);
        var dauMoi = b.Lay<DauSach>(maDauSach);
        YeuCau(string.Equals(dauCu.TenSach, dauMoi.TenSach, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(dauCu.NgonNgu, dauMoi.NgonNgu, StringComparison.OrdinalIgnoreCase) &&
            dauCu.DanhSachMaTacGia.ToHashSet().SetEquals(dauMoi.DanhSachMaTacGia),
            "BAN_MOI", "Tên sách, ngôn ngữ và tác giả phải tương đương. Quản lý xác nhận phiên bản trong căn cứ duyệt.");
        return cu;
    }

    private string GhiThanhLy(PhienLamViec p, CuonSach cuon, string maDuyet, string lyDo, string? maSuCo)
    {
        cuon.ThanhLy();
        hangCho.TamDungTrongGiaoDich(p, cuon.MaCuon, lyDo);
        var bienBan = new BienBanThanhLy(MaMoi(), cuon.MaCuon, lyDo, b.DongHo.HomNay,
            b.TaiKhoan.KiemTraQuyen(p)!.MaNhanVien!, maDuyet, maSuCo);
        b.Them(bienBan);
        return bienBan.MaThanhLy;
    }

    private PheDuyet TaoPheDuyet(PhienLamViec p, LoaiPheDuyet loai, string loaiDoiTuong, string doiTuong, string canCu)
    {
        var duyet = new PheDuyet(MaMoi(), b.TaiKhoan.KiemTraQuyen(p)!.MaNhanVien!, canCu,
            loai, loaiDoiTuong, doiTuong, b.DongHo.BayGio);
        duyet.ChapThuan();
        b.Them(duyet);
        return duyet;
    }

    private PheDuyet KiemTraDuyet(string ma, LoaiPheDuyet loai, string loaiDoiTuong, string doiTuong)
    {
        var duyet = b.Lay<PheDuyet>(ma);
        YeuCau(duyet.LoaiPheDuyet == loai && duyet.LoaiDoiTuong == loaiDoiTuong &&
            duyet.MaDoiTuong == doiTuong && duyet.QuyetDinh == QuyetDinhPheDuyet.ChapThuan,
            "PHE_DUYET", "Thiếu phê duyệt đúng đối tượng.");
        return duyet;
    }

    private void ThuThu(PhienLamViec p) => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu);
    private void QuanLy(PhienLamViec p) => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.QuanLy);
}
