namespace LibraryManagementSystem.Domain.Factories;

public abstract class DocGiaCreator
{
    public abstract DocGia Tao(string ma, string maTruong, string hoTen);
    public static DocGiaCreator Chon(LoaiDocGia loai) => loai switch
    {
        LoaiDocGia.SinhVien => new SinhVienCreator(),
        LoaiDocGia.GiangVien => new GiangVienCreator(),
        _ => throw new ArgumentOutOfRangeException(nameof(loai))
    };
}
