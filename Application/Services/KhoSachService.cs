namespace LibraryManagementSystem.Application.Services;

public sealed class KhoSachService : IQuanLyKhoSach
{
    private readonly BoDieuPhoi b; private readonly HangChoService cho;
    public KhoSachService(BoDieuPhoi b, HangChoService cho)
    {
        this.b = b;
        this.cho = cho;
    }
    public CuonSach? TimCuon(PhienLamViec p, string ma) => b.Doc(() => { b.TaiKhoan.KiemTraQuyen(p); return b.GiaoDich.Kho<CuonSach>().LayTheoMa(ma); });
    private CuonSach KiemTra(PhienLamViec p, string ma, long version)
    {
        YeuCau(b.GiaoDich.DangGiaoDich, "GIAO_DICH", "Phải gọi trong giao dịch mượn/trả.");
        b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu, VaiTro.QuanLy);
        var c = b.Lay<CuonSach>(ma);
        YeuCau(c.PhienBanDuLieu == version, "DU_LIEU_CU", "Cuốn đã thay đổi; tải lại.");
        return c;
    }
    public void DanhDauMuon(PhienLamViec p, string ma, string docGia, long phienBan)
    {
        var c = KiemTra(p, ma, phienBan);
        var dg = b.KiemTraQuyenSuDung(docGia);
        b.KhongNo(docGia);
        YeuCau(b.TheHienHanh(docGia).ConHieuLuc(b.DongHo.HomNay), "THE_HET_HAN", "Thẻ hết hạn.");
        YeuCau(b.SoNghiaVu(docGia) < dg.GioiHanCuon(), "HAN_MUC", "Vượt hạn mức.");
        YeuCau(!b.TatCa<ChiTietMuon>().Any(x => x.MaCuon == ma && x.NgayNhan is null && x.NgayKetThucNghiaVu is null), "DANG_MUON", "Cuốn còn lượt chiếm quyền.");
        cho.KiemTraNhanTrongGiaoDich(ma, docGia);
        c.DanhDauMuon();
    }
    public void DanhDauTra(PhienLamViec p, string ma, string maDong, TinhTrangSach tinhTrang, long phienBan)
    {
        var c = KiemTra(p, ma, phienBan);
        var dong = b.Lay<ChiTietMuon>(maDong);
        YeuCau(dong.MaCuon == ma && dong.NgayNhan != null, "NHAN_TRA", "Cần ghi ngày nhận thực tế của đúng dòng trước khi cập nhật cuốn.");
        c.NhanTra(tinhTrang);
        if (c.DungDuoc())
            cho.XetTrongGiaoDich(p, ma, maDong, "NhanTra:" + maDong);
        else
            cho.TamDungTrongGiaoDich(p, ma, "Sách trả bị hỏng, chưa thể giao");
    }
}