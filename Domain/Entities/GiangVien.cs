namespace LibraryManagementSystem.Domain.Entities;

public sealed class GiangVien : DocGia
{
    [JsonConstructor]
    internal GiangVien()
    {
    }
    public GiangVien(string maDocGia, string maTruong, string hoTen) : base(maDocGia, maTruong, hoTen) { }
    public string MaGiangVien => MaTruong; public override LoaiDocGia LoaiDocGia => LoaiDocGia.GiangVien;
    public override int SoNgayMuon() => 30; public override int GioiHanCuon() => 5;
}