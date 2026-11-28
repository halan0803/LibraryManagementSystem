namespace LibraryManagementSystem.Application.Services;

public sealed record TienThu(string MaKhoan, decimal SoTien);

public sealed class ThanhToanService(BoDieuPhoi b, ILichMoCua lich, Domain.Strategies.IChinhSachPhat phat)
{
    public PhieuThu XacNhanThu(PhienLamViec p, string yeuCau, IReadOnlyList<TienThu> phanBo, string canCu)
    {
        var cacKhoan = phanBo.ToArray();
        return b.Lenh(p, yeuCau, nameof(XacNhanThu), new { cacKhoan, canCu },
            () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu), () =>
            {
                Chuoi(canCu, nameof(canCu));
                YeuCau(cacKhoan.Length > 0 && cacKhoan.Select(x => x.MaKhoan).Distinct().Count() == cacKhoan.Length,
                    "KHOAN_THU", "Chọn ít nhất một khoản và không lặp mã.");
                var ngay = b.DongHo.HomNay;
                var maPhieu = MaMoi();
                var chiTiet = new List<ChiTietThu>();
                var dongCanKiemTra = new HashSet<string>();
                string? docGia = null;
                foreach (var muc in cacKhoan)
                {
                    var khoan = b.Lay<KhoanThu>(muc.MaKhoan);
                    docGia ??= khoan.MaDocGia;
                    YeuCau(khoan.MaDocGia == docGia, "CHU_HO_SO", "Một phiếu thu chỉ thuộc một độc giả.");
                    var dong = b.Lay<ChiTietMuon>(khoan.MaDongMuon);
                    YeuCau(khoan.TrangThai != TrangThaiKhoanThu.ChoDoiSoat, "DOI_SOAT", "Phải đối soát trước khi xác nhận thu.");
                    if (khoan.Loai == LoaiKhoanThu.PhatQuaHan && dong.TrangThai == TrangThaiMuon.MatChoXuLy)
                        khoan.ChotLai(phat.TinhPhat(dong.TinhNgayTre(ngay, lich)));
                    khoan.XacNhanThuDu(muc.SoTien);
                    chiTiet.Add(new ChiTietThu { MaDongThu = MaMoi(), MaKhoan = khoan.MaKhoan, SoTien = muc.SoTien, MaPhieuThu = maPhieu });
                    dongCanKiemTra.Add(dong.MaDong);
                }
                var phieu = new PhieuThu(maPhieu, docGia!, b.TaiKhoan.KiemTraQuyen(p)!.MaNhanVien!,
                    ngay, canCu, chiTiet.ToArray(), TrangThaiPhieuThu.DaXacNhan);
                b.Them(phieu);
                foreach (var ma in dongCanKiemTra)
                {
                    var dong = b.Lay<ChiTietMuon>(ma);
                    CapNhatDongDaNhan(b, dong, ngay);
                    SuCoService.ThuTatToanMat(b, lich, phat, dong);
                }
                return phieu;
            });
    }

    public string GhiChoDoiSoat(PhienLamViec p, string yeuCau, string maKhoan, string bangChung) =>
        b.Lenh(p, yeuCau, nameof(GhiChoDoiSoat), new { maKhoan, bangChung },
            () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.ThuThu), () =>
            {
                Chuoi(bangChung, nameof(bangChung));
                b.Lay<KhoanThu>(maKhoan).ChoDoiSoatTienNhan();
                return maKhoan;
            });

    public string DoiSoat(PhienLamViec p, string yeuCau, string maKhoan, string canCu) =>
        b.Lenh(p, yeuCau, nameof(DoiSoat), new { maKhoan, canCu },
            () => b.TaiKhoan.KiemTraQuyen(p, null, false, VaiTro.QuanLy), () =>
            {
                Chuoi(canCu, nameof(canCu));
                var khoan = b.Lay<KhoanThu>(maKhoan);
                khoan.HoanTatDoiSoat();
                var dong = b.Lay<ChiTietMuon>(khoan.MaDongMuon);
                foreach (var suCo in b.TatCa<BienBanSuCo>().Where(x => x.MaKhoanBoiThuong == maKhoan && x.DaTimLai))
                    suCo.HoanTat();
                CapNhatDongDaNhan(b, dong, b.DongHo.HomNay);
                return maKhoan;
            });

    internal static void CapNhatDongDaNhan(BoDieuPhoi b, ChiTietMuon dong, DateOnly ngay)
    {
        if (dong.NgayNhan is null || dong.NgayKetThucNghiaVu is not null)
            return;
        var cacKhoan = b.TatCa<KhoanThu>().Where(x => x.MaDongMuon == dong.MaDong).ToArray();
        foreach (var suCo in b.TatCa<BienBanSuCo>().Where(x => x.MaDongMuon == dong.MaDong &&
            x.Loai == LoaiSuCo.Hong && x.TrangThai == TrangThaiSuCo.DangGiaiQuyet))
        {
            var boiThuong = cacKhoan.Single(x => x.MaKhoan == suCo.MaKhoanBoiThuong);
            if (boiThuong.TrangThai is TrangThaiKhoanThu.DaThuDu or TrangThaiKhoanThu.DaTatToanBangSach)
                suCo.HoanTat();
        }
        if (!cacKhoan.Any(x => x.SoTienConNo() > 0 || x.TrangThai == TrangThaiKhoanThu.ChoDoiSoat) &&
            b.TatCa<BienBanSuCo>().All(x => x.MaDongMuon != dong.MaDong || x.TrangThai == TrangThaiSuCo.HoanTat))
            dong.KetThucNghiaVu(ngay);
    }
}
