namespace LibraryManagementSystem.Domain.Entities;

public sealed class GiangVien(string maDocGia, string maTruong, string hoTen)
    : DocGia(maDocGia, maTruong, hoTen)
{
    public string MaGiangVien => MaTruong;
    public override LoaiDocGia LoaiDocGia => LoaiDocGia.GiangVien;
    public override int SoNgayMuon() => 30;
    public override int GioiHanCuon() => 5;
}
