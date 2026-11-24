namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal void GhiNhanChoDoiSoat(string ma, string canCu)
    {
        NhanVien();
        var k = Lay<KhoanThu>(ma);
        YeuCau(k.TrangThai == TrangThaiKhoanThu.ChuaThu, "DOI_SOAT", "Khoản thu không ở trạng thái chưa thu.");
        Chuoi(canCu, "Chứng từ");
        Sua(k with { TrangThai = TrangThaiKhoanThu.ChoDoiSoat });
        LuuCanCu(ma, canCu);
    }

    internal void DoiSoat(string ma, string chungTu, bool daNhan)
    {
        NhanVien(true);
        var k = Lay<KhoanThu>(ma);
        YeuCau(k.TrangThai == TrangThaiKhoanThu.ChoDoiSoat, "DOI_SOAT", "Khoản thu chưa được đánh dấu đối soát.");
        LuuCanCu(ma, Chuoi(chungTu, "Chứng từ"));
        if (!daNhan) { Sua(k with { TrangThai = TrangThaiKhoanThu.ChuaThu }); return; }
        YeuCau(!TatCa<PhieuThu>().Any(p => p.MaGiaoDich == chungTu), "DOI_SOAT", "Chứng từ đã ghi thu.");
        string id = MaMoi();
        var ct = new ChiTietThu { MaDongThu = MaMoi(), MaKhoan = ma, MaPhieuThu = id, SoTien = k.SoTienChot };
        Them(new PhieuThu { MaPhieuThu = id, MaDocGia = k.MaDocGia, MaGiaoDich = chungTu, MaYeuCau = chungTu,
            MaNguoiThu = Phien.MaTaiKhoan, SoTienThucThu = ct.SoTien, NgayThu = luc, DanhSachChiTiet = [ct], TrangThai = TrangThaiPhieuThu.DaXacNhan });
        Sua(k with { TrangThai = TrangThaiKhoanThu.DaThuDu, CachTatToan = CachTatToan.Tien, DanhSachChiTietThu = [ct] });
        var dong = Dong(k.MaDongMuon);
        if (dong.TrangThai == TrangThaiMuon.MatChoXuLy) TaoKhoan(dong, LoaiKhoanThu.PhatQuaHan, PhatConLai(dong));
        KetThucNeuDu(dong);
    }

    private void LuuCanCu(string ma, string canCu) => Them(new NhatKyThaoTac {
        MaNhatKy = MaMoi(), MaTaiKhoanThucHien = Phien.MaTaiKhoan, MaDoiTuong = ma, LoaiDoiTuong = "KhoanThu",
        HanhDong = "Đối soát", LyDo = canCu, ThoiDiem = luc });

    internal void ChoBanMoi(string id, bool tiepTuc)
    {
        var c = Lay<DangKyCho>(id);
        PhamVi(c.MaDocGia);
        YeuCau(c.TrangThai == TrangThaiDangKyCho.TamDung, "CHO", "Đăng ký chưa tạm dừng.");
        if (tiepTuc) Sua(c with { TiepTucChoBanThayThe = true });
        else HuyCho(id);
    }

    private void TamDungCho(string ma, string suKien)
    {
        foreach (var c in TatCa<DangKyCho>().Where(c => c.MaCuon == ma && c.TrangThai <= TrangThaiDangKyCho.DangCoQuyenNhan))
        {
            Sua(c with { TrangThai = TrangThaiDangKyCho.TamDung });
            Bao(c.MaDocGia, suKien, LoaiThongBao.SuCoHangCho, $"Cuốn {ma} gặp sự cố. Bạn có thể tiếp tục chờ bản thay thế hoặc hủy.");
        }
    }
}
