namespace LibraryManagementSystem.Application.Services;

public sealed record KetQuaNghiaVu(string MaDocGia, int SoCuonChuaHoanTat, decimal TienConNo, int SuCoChuaXong, bool ChoDoiSoat)
{
    public bool DaHoanTat => SoCuonChuaHoanTat == 0 && TienConNo == 0 && SuCoChuaXong == 0 && !ChoDoiSoat;
}
public sealed class HoSoService
{
    private readonly BoDieuPhoi b; private readonly HangChoService cho;
    public HoSoService(BoDieuPhoi b, HangChoService cho)
    {
        this.b = b;
        this.cho = cho;
    }
    private void Quyen(PhienLamViec p) => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.QuanLy);
    private KetQuaNghiaVu NghiaVu(string ma)
    {
        b.Lay<DocGia>(ma);
        var phieu = b.TatCa<PhieuMuon>().Where(x => x.MaDocGia == ma).Select(x => x.MaPhieu).ToHashSet();
        var dong = b.TatCa<ChiTietMuon>().Where(x => phieu.Contains(x.MaPhieu)).Select(x => x.MaDong).ToHashSet();
        var khoan = b.TatCa<KhoanThu>().Where(x => x.MaDocGia == ma).ToArray();
        return new(ma, b.SoNghiaVu(ma), khoan.Sum(x => x.SoTienConNo()), b.TatCa<BienBanSuCo>().Count(x => dong.Contains(x.MaDongMuon) && x.TrangThai != TrangThaiSuCo.HoanTat), khoan.Any(x => x.TrangThai == TrangThaiKhoanThu.ChoDoiSoat));
    }
    public KetQuaNghiaVu XemNghiaVu(PhienLamViec p, string ma) => b.Doc(() => { b.TaiKhoan.KiemTraQuyen(p, ma); return NghiaVu(ma); });
    public IReadOnlyList<DocGia> DanhSach(PhienLamViec p) => b.Doc(() => { b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu, VaiTro.QuanLy); return b.TatCa<DocGia>(); });
    public KetQuaNghiaVu XacNhanNghiaVu(PhienLamViec p, string yeuCau, string ma, string lyDo) => b.Lenh(p, yeuCau, "XacNhanNghiaVu", new { ma, lyDo }, () => Quyen(p), () => { Chuoi(lyDo, nameof(lyDo)); var k = NghiaVu(ma); YeuCau(k.DaHoanTat, "CON_NGHIA_VU", "Còn sách, công nợ, sự cố hoặc khoản chờ đối soát; chưa thể xác nhận."); return k; });
    public string DongHoSo(PhienLamViec p, string yeuCau, string ma, string lyDo) => b.Lenh(p, yeuCau, "DongHoSo", new
    {
        ma,
        lyDo
    }, () => Quyen(p), () =>
    {
        var k = NghiaVu(ma);
        YeuCau(k.DaHoanTat, "CON_NGHIA_VU", "Chưa hoàn tất toàn bộ nghĩa vụ.");
        var dg = b.Lay<DocGia>(ma);
        dg.DongHoSo(lyDo);
        b.GiaoDich.Kho<DocGia>().CapNhat(dg);

        foreach (var the in b.TatCa<TheThuVien>()
            .Where(x => x.MaDocGia == ma && x.TrangThai == TrangThaiThe.HienHanh))
        {
            the.HuyThe(lyDo);
            b.GiaoDich.Kho<TheThuVien>().CapNhat(the);
        }

        b.TaiKhoan.VoHieuHoaTrongGiaoDich(ma);
        cho.KetThucKhiMatQuyenTrongGiaoDich(p, ma, lyDo);

        return ma;
    });
    public string HanCheKhiBuocRoiTruong(PhienLamViec p, string yeuCau, string ma, string canCu) => b.Lenh(p, yeuCau, "HanCheKhiBuocRoiTruong", new
    {
        ma,
        canCu
    }, () => Quyen(p), () =>
    {
        var dg = b.Lay<DocGia>(ma);
        dg.DungQuyen(canCu);
        dg.GhiNgayRoiTruong(b.DongHo.HomNay);
        b.GiaoDich.Kho<DocGia>().CapNhat(dg);

        cho.KetThucKhiMatQuyenTrongGiaoDich(p, ma, canCu);

        return ma;
    });
}