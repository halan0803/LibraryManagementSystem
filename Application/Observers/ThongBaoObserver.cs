using LibraryManagementSystem.Application.Services;
namespace LibraryManagementSystem.Application.Observers;

public sealed class ThongBaoObserver : IThuVienObserver
{
    private readonly BoDieuPhoi b; private readonly IKenhThongBao kenh; private readonly ILichMoCua lich;
    public string Ten => nameof(ThongBaoObserver);
    public ThongBaoObserver(BoDieuPhoi b, IKenhThongBao kenh, ILichMoCua lich)
    {
        this.b = b;
        this.kenh = kenh;
        this.lich = lich;
    }
    public void CapNhat(SuKienThuVien e)
    {
        if (e.LoaiThongBao is null || e.MaDocGia is null)
            return;
        var tb = b.TatCa<ThongBao>().SingleOrDefault(x => x.MaSuKien == e.MaSuKien && x.NguoiNhan == e.MaDocGia && x.Loai == e.LoaiThongBao);
        if (tb?.TrangThai == TrangThaiThongBao.DaGui)
            return;
        if (tb == null)
        {
            tb = new(MaMoi(), e.MaDocGia, e.NoiDung, e.LoaiThongBao.Value, e.MaSuKien, e.ThoiDiem);
            b.Them(tb);
        }
        try
        {
            kenh.Gui(tb);
        }
        catch { tb.DanhDauLoi(); throw; }
    }
    public IReadOnlyList<ThongBao> DanhSach(PhienLamViec p) => b.Doc(() => { var a = b.TaiKhoan.KiemTraQuyen(p); return (IReadOnlyList<ThongBao>)b.TatCa<ThongBao>().Where(x => a!.VaiTro != VaiTro.DocGia || x.NguoiNhan == a.MaDocGia).ToArray(); });
    public void TaoNhacQuaHan(DateOnly ngay)
    {
        YeuCau(ngay == b.DongHo.HomNay, "NGAY", "Ngày nhắc phải lấy từ nguồn thời gian chung.");
        var p = b.TaiKhoan.HeThong;
        b.Lenh(p, "NhacQuaHan:" + ngay.ToString("yyyy-MM-dd"), "TaoNhacQuaHan", new
        {
            ngay
        }, () => b.TaiKhoan.KiemTraQuyen(p, null, true), () =>
        {
            foreach (var d in b.TatCa<ChiTietMuon>().Where(x => x.NgayNhan is null && x.NgayKetThucNghiaVu is null && lich.DemNgayMoCua(x.HanTra, ngay) >= 1))
            {
                // Một lời nhắc cho một dòng/hạn trả; khởi động muộn vẫn nhắc bù.
                var id = "QuaHan:" + d.MaDong + ":" + d.HanTra.ToString("yyyy-MM-dd");
                if (b.GiaoDich.Kho<SuKienThuVien>().LayTheoMa(id) != null)
                    continue;
                var dg = b.Lay<PhieuMuon>(d.MaPhieu).MaDocGia;
                b.GhiSuKien(new(id, "NhacQuaHan", d.MaDong, dg, b.DongHo.BayGio, null, $"Cuốn {d.MaCuon} quá hạn từ {d.HanTra:dd/MM/yyyy}.", LoaiThongBao.NhacQuaHan));
            }
            return ngay;
        });
    }
    public string GuiLai(PhienLamViec p, string yeuCau, string maThongBao) => b.Lenh(p, yeuCau, "GuiLaiThongBao", new
    {
        maThongBao
    }, () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu, VaiTro.QuanLy), () =>
    {
        var tb = b.Lay<ThongBao>(maThongBao);
        if (tb.TrangThai == TrangThaiThongBao.DaGui)
            return maThongBao;
        tb.YeuCauGuiLai();
        var status = b.GiaoDich.Kho<XuLySuKien>().LayTheoMa(tb.MaSuKien + ":" + Ten);
        if (status != null)
            status.ThanhCong = false;
        return maThongBao;
    });
}
