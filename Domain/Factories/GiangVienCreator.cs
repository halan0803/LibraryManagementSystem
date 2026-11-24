namespace LibraryManagementSystem.Domain.Factories;

public sealed class GiangVienCreator : DocGiaCreator
{
    public override DocGia Tao(string ma, string maTruong, string hoTen) => new GiangVien(ma, maTruong, hoTen);
}
