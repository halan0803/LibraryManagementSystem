namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal void DinhGia(string ma, decimal gia, string canCu)
    {
        var cu = Lay<CuonSach>(ma);
        YeuCau(cu.GiaBia == null && gia > 0, "GIA", "Chỉ định giá khi thiếu giá bìa.");
        var duyet = Duyet(LoaiPheDuyet.DinhGia, "CuonSach", ma, canCu);
        Sua(cu with { GiaDinhGia = gia, MaPheDuyetGia = duyet });
        foreach (var sc in TatCa<BienBanSuCo>().Where(s => s.TrangThai == TrangThaiSuCo.ChoPheDuyet && Dong(s.MaDongMuon).MaCuon == ma))
        {
            Sua(sc with { GiaTriSachChot = gia, MaPheDuyet = duyet, TrangThai = TrangThaiSuCo.DangGiaiQuyet });
            TaoKhoan(Dong(sc.MaDongMuon), LoaiKhoanThu.BoiThuong, gia * sc.TyLeBoiThuong!.Value, sc.MaSuCo);
        }
    }

    internal void TimLai(string id, TinhTrangSach tinhTrang)
    {
        NhanVien();
        var dong = Dong(id);
        if (dong.TrangThai == TrangThaiMuon.MatChoXuLy) { NhanTra(id, tinhTrang); return; }
        var cu = Lay<CuonSach>(dong.MaCuon);
        YeuCau(dong.TrangThai == TrangThaiMuon.HoanTat && cu.TrangThaiLuuThong == TrangThaiCuonSach.Mat &&
            TatCa<BienBanSuCo>().Any(s => s.MaDongMuon == id && s.Loai == LoaiSuCo.Mat && s.TrangThai == TrangThaiSuCo.HoanTat),
            "TIM_LAI", "Chưa có nghĩa vụ mất đã tất toán.");
        Sua(cu with { QuyenSoHuu = QuyenSoHuuSach.NguoiDoc, MaKe = null, TrangThaiLuuThong = TrangThaiCuonSach.KhongLuuThong });
    }

    private void Bao(string ma, string suKien, LoaiThongBao loai, string noiDung)
    {
        if (TatCa<ThongBao>().Any(t => t.MaSuKien == suKien && t.NguoiNhan == ma)) return;
        Them(new ThongBao { MaThongBao = MaMoi(), MaSuKien = suKien, NguoiNhan = ma,
            NoiDung = noiDung, Loai = loai, ThoiDiemTao = luc, TrangThai = TrangThaiThongBao.DaGui, SoLanGui = 1 });
    }

    private void NhacQuaHan()
    {
        foreach (var p in TatCa<PhieuMuon>())
            foreach (var d in p.DanhSachDongMuon.Where(d => d.TrangThai == TrangThaiMuon.DangMuon && Ngay > d.HanTra))
                Bao(p.MaDocGia, d.MaDong + "-qua-han", LoaiThongBao.NhacQuaHan, $"Cuốn {d.MaCuon} quá hạn từ {d.HanTra:dd/MM/yyyy}.");
    }

    private void KetThucDotKhiMuon(DangKyCho cho)
    {
        var dot = TatCa<DotGiuCho>().Single(d => d.MaCuon == cho.MaCuon && d.TrangThai == TrangThaiDotGiuCho.DangXuLy);
        foreach (var luot in dot.DanhSachLuot.Where(l => l.MaDangKy != cho.MaDangKy))
        {
            var c = Lay<DangKyCho>(luot.MaDangKy);
            if (c.TrangThai == TrangThaiDangKyCho.DaDuocChon) Sua(c with { TrangThai = TrangThaiDangKyCho.DangCho });
        }
        Sua(dot with { TrangThai = TrangThaiDotGiuCho.KetThuc, DanhSachLuot = dot.DanhSachLuot.Select(l =>
            l with { TrangThai = l.MaDangKy == cho.MaDangKy ? TrangThaiLuotGiuCho.DaNhan : TrangThaiLuotGiuCho.DaHuy }).ToList() });
    }
}
