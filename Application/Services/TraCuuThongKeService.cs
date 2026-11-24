namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal object TraSach(string tuKhoa) =>
        (from cuon in TatCa<CuonSach>()
         let dau = Lay<DauSach>(cuon.MaDauSach)
         let tacGia = string.Join(", ", dau.DanhSachMaTacGia.Select(id => Lay<TacGia>(id).HoTen))
         where (cuon.MaCuon + " " + dau.TenSach + " " + tacGia).Contains(tuKhoa, StringComparison.CurrentCultureIgnoreCase)
         select new { cuon.MaCuon, dau.TenSach, TacGia = tacGia, dau.PhienBan, cuon.MaKe, cuon.TrangThaiLuuThong }).ToList();

    internal object HoSo(string ma)
    {
        PhamVi(ma);
        var dg = Lay<DocGia>(ma);
        return new { dg.HoTen, dg.MaDocGia, dg.TinhTrangQuyen, The = TheHienHanh(ma), Muon = DongCua(ma).ToList(),
            CongNo = TatCa<KhoanThu>().Where(k => k.MaDocGia == ma),
            HangCho = TatCa<DangKyCho>().Where(k => k.MaDocGia == ma),
            ThongBao = TatCa<ThongBao>().Where(t => t.NguoiNhan == ma) };
    }

    internal object ThongKe(DateOnly? tu = null, DateOnly? den = null)
    {
        NhanVien();
        YeuCau(tu == null || den == null || tu <= den, "THOI_GIAN", "Khoảng ngày không hợp lệ.");
        bool TrongKy(DateOnly ngay) => (tu == null || ngay >= tu) && (den == null || ngay <= den);
        var dong = TatCa<PhieuMuon>().SelectMany(p => p.DanhSachDongMuon)
            .Where(d => d.TrangThai is TrangThaiMuon.DangMuon or TrangThaiMuon.MatChoXuLy).ToList();
        return new { DangMuon = dong.Count, QuaHan = dong.Count(d => d.HanTra < Ngay),
            ChuaThu = TatCa<KhoanThu>().Where(k => k.TrangThai == TrangThaiKhoanThu.ChuaThu).Sum(k => k.SoTienChot),
            ThucThu = TatCa<PhieuThu>().Where(p => p.TrangThai == TrangThaiPhieuThu.DaXacNhan && TrongKy(DateOnly.FromDateTime(p.NgayThu.ToOffset(TimeSpan.FromHours(7)).DateTime))).Sum(p => p.SoTienThucThu),
            PhatTamTinh = dong.Sum(d => Math.Max(0, PhatConLai(d))), SuCo = TatCa<BienBanSuCo>() };
    }
}
