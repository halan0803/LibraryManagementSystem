namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal string ThuTien(string ma, decimal tien, string maGiaoDich)
    {
        NhanVien();
        Chuoi(maGiaoDich, "Mã giao dịch");
        var cu = TatCa<PhieuThu>().SingleOrDefault(p => p.MaGiaoDich == maGiaoDich);
        if (cu != null)
        {
            YeuCau(cu.MaDocGia == ma && cu.SoTienThucThu == tien, "TRUNG", "Mã giao dịch bị trùng.");
            return cu.MaPhieuThu;
        }
        foreach (var dong in DongCua(ma).Where(d => d.TrangThai == TrangThaiMuon.MatChoXuLy))
            TaoKhoan(dong, LoaiKhoanThu.PhatQuaHan, PhatConLai(dong));
        YeuCau(!TatCa<KhoanThu>().Any(k => k.MaDocGia == ma && k.TrangThai == TrangThaiKhoanThu.ChoDoiSoat), "DOI_SOAT", "Cần đối soát trước khi thu lại.");
        var no = TatCa<KhoanThu>().Where(k => k.MaDocGia == ma && k.TrangThai == TrangThaiKhoanThu.ChuaThu).ToArray();
        YeuCau(tien > 0 && tien == no.Sum(k => k.SoTienChot), "THU", "Số tiền phải bằng tổng nghĩa vụ chưa thu.");
        string id = MaMoi();
        var ds = new List<ChiTietThu>();
        foreach (var k in no)
        {
            var ct = new ChiTietThu { MaDongThu = MaMoi(), MaKhoan = k.MaKhoan, SoTien = k.SoTienChot, MaPhieuThu = id };
            ds.Add(ct);
            Sua(k with { TrangThai = TrangThaiKhoanThu.DaThuDu, CachTatToan = CachTatToan.Tien, DanhSachChiTietThu = [ct] });
        }
        Them(new PhieuThu { MaPhieuThu = id, NgayThu = luc, MaNguoiThu = Phien.MaTaiKhoan, MaDocGia = ma,
            MaGiaoDich = maGiaoDich, MaYeuCau = maGiaoDich, SoTienThucThu = tien,
            TrangThai = TrangThaiPhieuThu.DaXacNhan, DanhSachChiTiet = ds });
        foreach (string maDong in no.Select(k => k.MaDongMuon).Distinct()) KetThucNeuDu(Dong(maDong));
        return id;
    }

    private void KetThucNeuDu(ChiTietMuon dong)
    {
        if (TatCa<KhoanThu>().Any(k => k.MaDongMuon == dong.MaDong && k.TrangThai is TrangThaiKhoanThu.ChuaThu or TrangThaiKhoanThu.ChoDoiSoat)
            || TatCa<BienBanSuCo>().Any(s => s.MaDongMuon == dong.MaDong && s.TrangThai == TrangThaiSuCo.ChoPheDuyet)) return;
        foreach (var s in TatCa<BienBanSuCo>().Where(s => s.MaDongMuon == dong.MaDong && s.TrangThai != TrangThaiSuCo.HoanTat))
            Sua(s with { TrangThai = TrangThaiSuCo.HoanTat, NgayHoanTat = Ngay });
        SuaDong(dong with { TrangThai = TrangThaiMuon.HoanTat, NgayChotPhat = dong.NgayNhan ?? Ngay, NgayKetThucNghiaVu = Ngay });
    }
}

