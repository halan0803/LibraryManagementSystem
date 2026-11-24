namespace LibraryManagementSystem.Domain.Strategies;

public sealed class DinhGiaThuVien : IChinhSachDinhGia
{
    public decimal GiaTriSach(CuonSach cuon) => cuon.GiaDinhGia is >= 0 && cuon.MaPheDuyetGia != null
        ? cuon.GiaDinhGia.Value : throw new NghiepVuException("GIA", "Thiếu định giá được phê duyệt.");
}
