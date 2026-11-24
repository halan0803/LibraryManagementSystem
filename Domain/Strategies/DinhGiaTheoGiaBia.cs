namespace LibraryManagementSystem.Domain.Strategies;

public sealed class DinhGiaTheoGiaBia : IChinhSachDinhGia
{
    public decimal GiaTriSach(CuonSach cuon) => cuon.GiaBia is >= 0 ? cuon.GiaBia.Value
        : throw new NghiepVuException("GIA", "Thiếu giá bìa hợp lệ.");
}
