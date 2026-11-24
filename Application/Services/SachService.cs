namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal void NhapSach(string maDau, string ten, string tacGia, string phienBan, string ngonNgu, string maCuon, decimal gia, string ke)
    {
        NhanVien();
        Chuoi(maCuon, "Mã cuốn"); Chuoi(maDau, "Mã đầu sách"); Chuoi(ke, "Kệ");
        YeuCau(gia >= 0, "GIA", "Giá phải không âm.");
        if (Kho<DauSach>().LayTheoMa(maDau) == null)
        {
            var tg = TatCa<TacGia>().FirstOrDefault(t => t.HoTen == tacGia);
            if (tg == null) { tg = new TacGia { MaTacGia = MaMoi(), HoTen = Chuoi(tacGia, "Tác giả") }; Them(tg); }
            Them(new DauSach { MaDauSach = maDau, TenSach = Chuoi(ten, "Tên sách"), PhienBan = Chuoi(phienBan, "Phiên bản"),
                NgonNgu = Chuoi(ngonNgu, "Ngôn ngữ"), DanhSachMaTacGia = [tg.MaTacGia], GiaBia = gia });
        }
        if (Kho<KeSach>().LayTheoMa(ke) == null) Them(new KeSach { MaKe = ke, DangSuDung = true, ViTri = new("Thư viện", ke) });
        Them(new CuonSach { MaCuon = maCuon, MaDauSach = maDau, GiaBia = gia, MaKe = ke });
    }

    private string Duyet(LoaiPheDuyet loai, string doiTuong, string ma, string canCu)
    {
        NhanVien(true);
        string id = MaMoi();
        Them(new PheDuyet { MaPheDuyet = id, MaNguoiDuyet = Phien.MaTaiKhoan, NgayDuyet = luc,
            LoaiPheDuyet = loai, LoaiDoiTuong = doiTuong, MaDoiTuong = ma, CanCu = Chuoi(canCu, "Căn cứ"), QuyetDinh = QuyetDinhPheDuyet.ChapThuan });
        return id;
    }

    internal void NhanBanThay(string maSC, string maMoi, string dauSach, string ke, string canCu)
    {
        var sc = Lay<BienBanSuCo>(maSC);
        var dong = Dong(sc.MaDongMuon);
        var cu = Lay<CuonSach>(dong.MaCuon);
        YeuCau(sc.TrangThai != TrangThaiSuCo.HoanTat && (sc.Loai == LoaiSuCo.Mat || sc.MucDo == TinhTrangSach.HuNang)
            && dauSach == cu.MaDauSach && maMoi != cu.MaCuon && Kho<CuonSach>().LayTheoMa(maMoi) == null,
            "THAY_THE", "Cần bản cùng ấn bản, mã mới và sự cố đủ điều kiện.");
        var khoan = TatCa<KhoanThu>().SingleOrDefault(k => k.MaSuCo == maSC && k.Loai == LoaiKhoanThu.BoiThuong);
        YeuCau(khoan?.TrangThai == TrangThaiKhoanThu.ChuaThu, "THAY_THE", "Bồi thường đã xử lý hoặc chưa chốt.");
        Lay<KeSach>(ke);
        string duyet = Duyet(LoaiPheDuyet.BanThayThe, "BienBanSuCo", maSC, canCu);
        string bienNhan = MaMoi();
        Them(new BienNhanThayThe { MaBienNhan = bienNhan, MaCuonCu = cu.MaCuon, MaCuonMoi = maMoi, MaSuCo = maSC,
            MaNguoiDuyet = Phien.MaTaiKhoan, MaNguoiNhan = Phien.MaTaiKhoan, MaPheDuyet = duyet, NgayNhan = Ngay });
        Them(new CuonSach { MaCuon = maMoi, MaDauSach = dauSach, GiaBia = cu.GiaBia, MaKe = ke });
        Sua(khoan! with { TrangThai = TrangThaiKhoanThu.DaTatToanBangSach, MaBienNhan = bienNhan, CachTatToan = CachTatToan.BanThayThe });
        if (sc.Loai == LoaiSuCo.Mat) TaoKhoan(dong, LoaiKhoanThu.PhatQuaHan, PhatConLai(dong));
        KetThucNeuDu(dong);
    }

    internal void ThanhLy(string ma, string lyDo)
    {
        var cu = Lay<CuonSach>(ma);
        YeuCau(cu.TinhTrangVatLy == TinhTrangSach.HuNang && cu.TrangThaiLuuThong == TrangThaiCuonSach.KhongLuuThong,
            "THANH_LY", "Chỉ thanh lý cuốn hư nặng đã ngừng lưu thông.");
        string duyet = Duyet(LoaiPheDuyet.ThanhLy, "CuonSach", ma, lyDo);
        Them(new BienBanThanhLy { MaThanhLy = MaMoi(), MaCuon = ma, MaPheDuyet = duyet,
            MaNguoiDuyet = Phien.MaTaiKhoan, LyDo = lyDo, NgayThanhLy = Ngay });
        Sua(cu with { TrangThaiLuuThong = TrangThaiCuonSach.DaThanhLy, MaKe = null });
    }
}
