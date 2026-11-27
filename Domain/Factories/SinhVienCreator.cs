using LibraryManagementSystem.Domain.Entities;
namespace LibraryManagementSystem.Domain.Factories;

public sealed class SinhVienCreator : DocGiaCreator
{
    public override DocGia TaoDocGia(string maDocGia, string maTruong, string hoTen) => new SinhVien(maDocGia, maTruong, hoTen);
}