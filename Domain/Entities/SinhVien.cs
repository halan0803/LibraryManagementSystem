namespace LibraryManagementSystem.Domain.Entities;

public sealed class SinhVien(string maDocGia, string maTruong, string hoTen)
    : DocGia(maDocGia, maTruong, hoTen)
{
    public string MaSinhVien => MaTruong;
    public override LoaiDocGia LoaiDocGia => LoaiDocGia.SinhVien;
    public override int SoNgayMuon() => 14;
    public override int GioiHanCuon() => 3;
}
