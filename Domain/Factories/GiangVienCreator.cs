using LibraryManagementSystem.Domain.Entities;
namespace LibraryManagementSystem.Domain.Factories;

public sealed class GiangVienCreator : DocGiaCreator
{
    public override DocGia TaoDocGia(string maDocGia, string maTruong, string hoTen) => new GiangVien(maDocGia, maTruong, hoTen);
}