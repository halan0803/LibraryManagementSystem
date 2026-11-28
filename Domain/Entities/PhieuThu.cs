namespace LibraryManagementSystem.Domain.Entities;


public sealed record PhieuThu(string MaPhieuThu, string MaDocGia, string MaNguoiThu,
    DateOnly NgayThu, string CanCuThu, ChiTietThu[] ChiTiet, TrangThaiPhieuThu TrangThai)
{
    public decimal TongTien => ChiTiet.Sum(x => x.SoTien);
}
