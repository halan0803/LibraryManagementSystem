namespace LibraryManagementSystem.Domain.Entities;

public sealed class SinhVien : DocGia
{
    [JsonConstructor]
    internal SinhVien()
    {
    }
    public SinhVien(string maDocGia, string maTruong, string hoTen) : base(maDocGia, maTruong, hoTen) { }
    public string MaSinhVien => MaTruong; public override LoaiDocGia LoaiDocGia => LoaiDocGia.SinhVien;
    public override int SoNgayMuon() => 14; public override int GioiHanCuon() => 3;
}