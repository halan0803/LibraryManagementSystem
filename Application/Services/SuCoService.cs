namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    private void TaoKhoan(ChiTietMuon dong, LoaiKhoanThu loai, decimal tien, string? suCo = null)
    {
        if (tien <= 0) return;
        Them(new KhoanThu { MaKhoan = MaMoi(), MaDocGia = Lay<PhieuMuon>(dong.MaPhieu).MaDocGia,
            MaDongMuon = dong.MaDong, Loai = loai, SoTienChot = tien, NgayChot = Ngay, MaSuCo = suCo });
    }

    internal void NhanTra(string maDong, TinhTrangSach tinhTrang)
    {
        NhanVien();
        var dong = Dong(maDong);
        YeuCau(dong.TrangThai is TrangThaiMuon.DangMuon or TrangThaiMuon.MatChoXuLy, "TRA", "Cuốn đã tiếp nhận.");
        var cuon = Lay<CuonSach>(dong.MaCuon);
        if (dong.TrangThai == TrangThaiMuon.MatChoXuLy)
        {
            foreach (var k in TatCa<KhoanThu>().Where(k => k.MaDongMuon == maDong && k.Loai == LoaiKhoanThu.BoiThuong && k.TrangThai == TrangThaiKhoanThu.ChuaThu))
                Sua(k with { TrangThai = TrangThaiKhoanThu.DaHuy });
            foreach (var sc in TatCa<BienBanSuCo>().Where(s => s.MaDongMuon == maDong && s.Loai == LoaiSuCo.Mat))
                Sua(sc with { TrangThai = TrangThaiSuCo.HoanTat, NgayHoanTat = Ngay });
        }
        TaoKhoan(dong, LoaiKhoanThu.PhatQuaHan, PhatConLai(dong));
        bool hong = tinhTrang >= TinhTrangSach.HuNhe;
        if (hong) LapSuCo(dong, cuon, LoaiSuCo.Hong, tinhTrang, "Kiểm tra khi nhận trả");
        Sua(cuon with { TinhTrangVatLy = tinhTrang, TrangThaiLuuThong = tinhTrang switch {
            TinhTrangSach.HuNhe => TrangThaiCuonSach.DangSua, TinhTrangSach.HuNang => TrangThaiCuonSach.KhongLuuThong,
            _ => TrangThaiCuonSach.SanSang } });
        bool xuLy = hong || TatCa<KhoanThu>().Any(k => k.MaDongMuon == maDong && k.TrangThai == TrangThaiKhoanThu.ChuaThu);
        SuaDong(dong with { NgayNhan = Ngay, NgayChotPhat = Ngay, TinhTrangKhiNhan = tinhTrang.ToString(),
            TrangThai = xuLy ? TrangThaiMuon.DaNhanChoXuLy : TrangThaiMuon.HoanTat });
        if (!hong) XetCho(dong);
    }

    private decimal PhatConLai(ChiTietMuon dong)
    {
        decimal phat = new Domain.Strategies.PhatTheoNgay().TinhPhat(lich.DemNgayMoCua(dong.HanTra, dong.NgayNhan ?? Ngay));
        return phat - TatCa<KhoanThu>().Where(k => k.MaDongMuon == dong.MaDong && k.Loai == LoaiKhoanThu.PhatQuaHan
            && k.TrangThai != TrangThaiKhoanThu.DaHuy).Sum(k => k.SoTienChot);
    }

    internal void BaoMat(string maDong, string lyDo)
    {
        var dong = Dong(maDong);
        PhamVi(Lay<PhieuMuon>(dong.MaPhieu).MaDocGia);
        YeuCau(dong.TrangThai == TrangThaiMuon.DangMuon, "MAT", "Dòng không đang mượn.");
        var cuon = Lay<CuonSach>(dong.MaCuon);
        LapSuCo(dong, cuon, LoaiSuCo.Mat, cuon.TinhTrangVatLy, Chuoi(lyDo, "Minh chứng"));
        SuaDong(dong with { TrangThai = TrangThaiMuon.MatChoXuLy });
        Sua(cuon with { TrangThaiLuuThong = TrangThaiCuonSach.Mat });
    }

    private void LapSuCo(ChiTietMuon dong, CuonSach cuon, LoaiSuCo loai, TinhTrangSach muc, string canCu)
    {
        string id = MaMoi();
        decimal tyLe = loai == LoaiSuCo.Mat || muc == TinhTrangSach.HuNang ? 1m : .5m;
        Domain.Strategies.IChinhSachDinhGia chinhSach = cuon.GiaBia.HasValue
            ? new Domain.Strategies.DinhGiaTheoGiaBia() : new Domain.Strategies.DinhGiaThuVien();
        decimal? gia = cuon.GiaBia.HasValue || cuon.MaPheDuyetGia != null ? chinhSach.GiaTriSach(cuon) : null;
        Them(new BienBanSuCo { MaSuCo = id, MaDongMuon = dong.MaDong, Loai = loai, MucDo = muc, MinhChung = canCu,
            NgayBao = Ngay, MaNguoiLap = Phien.MaTaiKhoan, GiaTriSachChot = gia, TyLeBoiThuong = tyLe,
            TrangThai = gia == null ? TrangThaiSuCo.ChoPheDuyet : TrangThaiSuCo.DangGiaiQuyet });
        if (gia.HasValue) TaoKhoan(dong, LoaiKhoanThu.BoiThuong, gia.Value * tyLe, id);
    }
}

