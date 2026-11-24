namespace LibraryManagementSystem.Domain.Factories;

public sealed class SinhVienCreator : DocGiaCreator
{
    public override DocGia Tao(string ma, string maTruong, string hoTen) => new SinhVien(ma, maTruong, hoTen);
}
