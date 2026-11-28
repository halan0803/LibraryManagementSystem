namespace LibraryManagementSystem.Domain.Strategies;

public sealed class DinhGiaThuVien : IChinhSachDinhGia
{
    public decimal GiaTriSach(CuonSach cuon) => cuon.GiaDinhGia is > 0 && cuon.MaPheDuyetGia is not null
        ? cuon.GiaDinhGia.Value : throw new NghiepVuException("DINH_GIA", "Cần giá do quản lý phê duyệt.");
}
