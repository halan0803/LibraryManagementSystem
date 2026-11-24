namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal string DangKy(string ma, string cuon)
    {
        PhamVi(ma);
        YeuCau(ConQuyen(ma) && ConCho(ma, 1) && Lay<CuonSach>(cuon).TrangThaiLuuThong == TrangThaiCuonSach.DangMuon &&
            !DongCua(ma).Any(d => d.MaCuon == cuon && d.TrangThai == TrangThaiMuon.DangMuon) &&
            !TatCa<DangKyCho>().Any(c => c.MaDocGia == ma && c.TrangThai <= TrangThaiDangKyCho.TamDung),
            "HANG_CHO", "Không đủ điều kiện đăng ký chờ.");
        string id = MaMoi();
        Them(new DangKyCho { MaDangKy = id, MaCuon = cuon, MaCuonBanDau = cuon, MaDocGia = ma,
            NgayDangKy = luc, ThuTu = TatCa<DangKyCho>().Select(c => c.ThuTu).DefaultIfEmpty().Max() + 1 });
        return id;
    }

    internal void HuyCho(string id)
    {
        var c = Lay<DangKyCho>(id);
        PhamVi(c.MaDocGia);
        YeuCau(c.TrangThai <= TrangThaiDangKyCho.TamDung, "HANG_CHO", "Đăng ký đã kết thúc.");
        Sua(c with { TrangThai = TrangThaiDangKyCho.DaHuy, LyDoKetThuc = "Người dùng hủy" });
        if (c.TrangThai == TrangThaiDangKyCho.DangCoQuyenNhan) ChuyenLuot(c.MaCuon);
    }

    private void XetCho(ChiTietMuon dong)
    {
        var ds = new List<LuotGiuCho>();
        string dot = MaMoi();
        foreach (var c in TatCa<DangKyCho>().Where(c => c.MaCuon == dong.MaCuon && c.TrangThai == TrangThaiDangKyCho.DangCho)
                     .OrderBy(c => c.NgayDangKy).ThenBy(c => c.ThuTu))
        {
            var moc = lich.CongNgayMoCua(dong.NgayMuon, dong.LoaiDocGiaLucMuon == LoaiDocGia.SinhVien ? 21 : 37);
            if (!ConQuyen(c.MaDocGia) || !ConCho(c.MaDocGia, 1) || TheHienHanh(c.MaDocGia)!.NgayHetHan < moc)
            {
                Sua(c with { TrangThai = TrangThaiDangKyCho.DaKetThuc, LyDoKetThuc = "Không đủ điều kiện tại ngày xét" });
                continue;
            }
            ds.Add(new LuotGiuCho { MaLuot = MaMoi(), MaDot = dot, MaDangKy = c.MaDangKy, ThuTu = ds.Count + 1 });
            Sua(c with { TrangThai = TrangThaiDangKyCho.DaDuocChon, MaDongNguon = dong.MaDong });
            if (ds.Count == 2) break;
        }
        if (ds.Count == 0) return;
        Them(new DotGiuCho { MaDot = dot, MaCuon = dong.MaCuon, MaDongNguon = dong.MaDong,
            MaSuKien = dong.MaDong, DanhSachLuot = ds });
        ChuyenLuot(dong.MaCuon);
    }

    private void ChuyenLuot(string cuon)
    {
        var dot = TatCa<DotGiuCho>().LastOrDefault(d => d.MaCuon == cuon && d.TrangThai == TrangThaiDotGiuCho.DangXuLy);
        if (dot == null) return;
        var ds = dot.DanhSachLuot.ToArray();
        for (int i = 0; i < ds.Length; i++)
        {
            var c = Lay<DangKyCho>(ds[i].MaDangKy);
            if (c.TrangThai == TrangThaiDangKyCho.DangCoQuyenNhan && ds[i].HanNhan > luc) return;
            if (c.TrangThai == TrangThaiDangKyCho.DangCoQuyenNhan)
                Sua(c with { TrangThai = TrangThaiDangKyCho.DaKetThuc, LyDoKetThuc = "Hết hạn nhận" });
            if (c.TrangThai == TrangThaiDangKyCho.DaDuocChon)
            {
                ds[i] = ds[i] with { TrangThai = TrangThaiLuotGiuCho.DangCoQuyen, ThoiDiemKichHoat = luc, HanNhan = lich.HanNhanSauHaiNgay(luc) };
                Sua(c with { TrangThai = TrangThaiDangKyCho.DangCoQuyenNhan });
                Bao(c.MaDocGia, ds[i].MaLuot, LoaiThongBao.QuyenNhan, $"Được nhận cuốn {cuon} đến {ds[i].HanNhan:dd/MM/yyyy HH:mm}.");
                Sua(dot with { DanhSachLuot = ds });
                Sua(Lay<CuonSach>(cuon) with { TrangThaiLuuThong = TrangThaiCuonSach.DangGiuCho });
                return;
            }
            ds[i] = ds[i] with { TrangThai = c.TrangThai == TrangThaiDangKyCho.DaHuy ? TrangThaiLuotGiuCho.DaHuy : TrangThaiLuotGiuCho.HetLuot };
        }
        Sua(dot with { DanhSachLuot = ds, TrangThai = TrangThaiDotGiuCho.KetThuc });
        Sua(Lay<CuonSach>(cuon) with { TrangThaiLuuThong = TrangThaiCuonSach.SanSang });
    }
}

