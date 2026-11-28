using LibraryManagementSystem.Domain.Entities;
namespace LibraryManagementSystem.Domain.Factories;

public abstract class DocGiaCreator
{
    public abstract DocGia TaoDocGia(string maDocGia, string maTruong, string hoTen);
}