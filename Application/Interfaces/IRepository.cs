namespace LibraryManagementSystem.Application.Interfaces;

public interface IRepository<T> where T : class
{
    // Không tìm thấy thì trả null.
    T? LayTheoMa(string ma);

    IReadOnlyList<T> LayTatCa();

    // Thêm mới; báo lỗi nếu mã đã tồn tại.
    void Luu(T doiTuong);

    // Cập nhật; báo lỗi nếu mã chưa tồn tại.
    void CapNhat(T doiTuong);
}