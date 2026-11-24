namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal void XuLyHoSo(string ma, string lyDo, bool dong)
    {
        NhanVien(true);
        var dg = Lay<DocGia>(ma);
        if (dong)
        {
            YeuCau(!CoNo(ma) && DongCua(ma).All(d => d.TrangThai == TrangThaiMuon.HoanTat), "HO_SO", "Hồ sơ còn nghĩa vụ.");
            dg.DongHoSo(lyDo);
            if (TheHienHanh(ma) is { } the) Sua(the with { TrangThai = TrangThaiThe.DaHuy });
            foreach (var tk in TatCa<TaiKhoan>().Where(t => t.MaDocGia == ma))
                Sua(tk with { TrangThai = TrangThaiTaiKhoan.VoHieuHoa });
        }
        else dg.DungQuyen(lyDo);
        Sua<DocGia>(dg);
        foreach (var cho in TatCa<DangKyCho>().Where(c => c.MaDocGia == ma && c.TrangThai <= TrangThaiDangKyCho.TamDung))
            HuyCho(cho.MaDangKy);
    }
}
