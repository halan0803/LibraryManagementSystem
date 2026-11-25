namespace LibraryManagementSystem.Domain.Interfaces;

public interface ILichMoCua
{
    bool LaNgayMoCua(DateOnly ngay);

    // Ngày mở cửa đầu tiên sau ngày truyền vào.
    DateOnly NgayMoTiep(DateOnly ngay);

    // Không tính ngày bắt đầu.
    // soNgay = 0: giữ nguyên ngày bắt đầu.
    // soNgay < 0: báo lỗi.
    DateOnly CongNgayMoCua(DateOnly ngayBatDau, int soNgay);

    // Đếm ngày mở cửa sau tuNgay đến hết denNgay.
    // denNgay <= tuNgay: trả 0.
    int DemNgayMoCua(DateOnly tuNgay, DateOnly denNgay);

    // Giờ đóng cửa của ngày mở thứ hai sau ngày cấp quyền.
    // Ngày cấp quyền được xác định theo giờ Việt Nam.
    DateTimeOffset HanNhanSauHaiNgay(DateTimeOffset lucCap);
}