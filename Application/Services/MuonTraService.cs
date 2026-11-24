namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    private IEnumerable<ChiTietMuon> DongCua(string ma) => TatCa<PhieuMuon>()
        .Where(p => p.MaDocGia == ma).SelectMany(p => p.DanhSachDongMuon);

    private bool ConQuyen(string ma) => Lay<DocGia>(ma).DuocSuDung()
        && TheHienHanh(ma) is { } the && the.NgayHetHan >= Ngay && !CoNo(ma);

    private bool ConCho(string ma, int them) => DongCua(ma).Count(d =>
        d.TrangThai is TrangThaiMuon.DangMuon or TrangThaiMuon.MatChoXuLy) + them <= Lay<DocGia>(ma).GioiHanCuon();

    private ChiTietMuon Dong(string ma) => TatCa<PhieuMuon>().SelectMany(p => p.DanhSachDongMuon)
        .SingleOrDefault(d => d.MaDong == ma) ?? throw new InvalidOperationException("Không tìm thấy dòng mượn.");

    private void SuaDong(ChiTietMuon dong)
    {
        var phieu = Lay<PhieuMuon>(dong.MaPhieu);
        var ds = phieu.DanhSachDongMuon.Select(d => d.MaDong == dong.MaDong ? dong : d).ToArray();
        Sua(phieu with { DanhSachDongMuon = ds, TrangThai = ds.All(d => d.TrangThai == TrangThaiMuon.HoanTat)
            ? TrangThaiPhieuMuon.HoanTat : TrangThaiPhieuMuon.DangXuLy });
    }

    internal string Muon(string ma, string[] cuon, string yeuCau)
    {
        NhanVien();
        Chuoi(yeuCau, "Mã yêu cầu");
        var cu = TatCa<PhieuMuon>().SingleOrDefault(p => p.MaYeuCau == yeuCau);
        if (cu != null)
        {
            YeuCau(cu.MaDocGia == ma && cu.DanhSachDongMuon.Select(d => d.MaCuon).SequenceEqual(cuon),
                "TRUNG", "Mã yêu cầu đã dùng cho nội dung khác.");
            return cu.MaPhieu;
        }
        YeuCau(cuon.Length > 0 && cuon.Distinct().Count() == cuon.Length && ConQuyen(ma) && ConCho(ma, cuon.Length),
            "MUON", "Kiểm tra danh sách cuốn, hạn mức, thẻ và công nợ.");
        var dg = Lay<DocGia>(ma);
        string phieu = MaMoi();
        var ds = new List<ChiTietMuon>();
        foreach (string id in cuon)
        {
            var sach = Lay<CuonSach>(id);
            var cho = TatCa<DangKyCho>().SingleOrDefault(c => c.MaCuon == id && c.TrangThai == TrangThaiDangKyCho.DangCoQuyenNhan);
            YeuCau(sach.TrangThaiLuuThong == TrangThaiCuonSach.SanSang ||
                (sach.TrangThaiLuuThong == TrangThaiCuonSach.DangGiuCho && cho?.MaDocGia == ma),
                "CUON", $"Cuốn {id} chưa thể giao.");
            if (cho != null)
            {
                Sua(cho with { TrangThai = TrangThaiDangKyCho.DaNhan });
                KetThucDotKhiMuon(cho);
            }
            Sua(sach with { TrangThaiLuuThong = TrangThaiCuonSach.DangMuon });
            ds.Add(new ChiTietMuon { MaDong = MaMoi(), MaCuon = id, MaPhieu = phieu, NgayMuon = Ngay,
                LoaiDocGiaLucMuon = dg.LoaiDocGia, HanTra = lich.CongNgayMoCua(Ngay, dg.SoNgayMuon()),
                TinhTrangKhiGiao = sach.TinhTrangVatLy.ToString() });
        }
        Them(new PhieuMuon { MaPhieu = phieu, MaDocGia = ma, NgayMuon = Ngay, MaNguoiLap = Phien.MaTaiKhoan,
            MaYeuCau = yeuCau, TrangThai = TrangThaiPhieuMuon.DangXuLy, DanhSachDongMuon = ds });
        return phieu;
    }
}

