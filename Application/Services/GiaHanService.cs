namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    internal void GiaHan(string maDong, int ngayThem)
    {
        var dong = Dong(maDong);
        string ma = Lay<PhieuMuon>(dong.MaPhieu).MaDocGia;
        PhamVi(ma);
        YeuCau(dong.TrangThai == TrangThaiMuon.DangMuon && !dong.DaGiaHan && Ngay <= dong.HanTra
            && ngayThem is 3 or 5 or 7 && ConQuyen(ma) && !TatCa<DangKyCho>().Any(c => c.MaCuon == dong.MaCuon
            && c.TrangThai <= TrangThaiDangKyCho.TamDung), "GIA_HAN", "Dòng mượn không đủ điều kiện gia hạn.");
        SuaDong(dong with { HanTra = lich.CongNgayMoCua(dong.HanTra, ngayThem), DaGiaHan = true });
    }
}
