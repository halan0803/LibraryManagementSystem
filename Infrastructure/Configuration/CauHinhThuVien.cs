using LibraryManagementSystem.Domain.Exceptions;

namespace LibraryManagementSystem.Infrastructure.Configuration;

public sealed class CauHinhThuVien
{
    public string DuongDanDuLieu { get; init; } =
        "Data/thu-vien.json";

    public TimeOnly GioDongCua { get; init; } =
        new(17, 0);

    public decimal DonGiaPhat { get; init; } =
        5000m;

    public List<DateOnly> NgayNghiDacBiet { get; init; } =
        new();

    public void KiemTra()
    {
        if (string.IsNullOrWhiteSpace(DuongDanDuLieu))
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Đường dẫn dữ liệu không được để trống.");
        }

        string duongDan = DuongDanDuLieu.Replace('\\', '/');

        if (Path.IsPathRooted(DuongDanDuLieu)
            || duongDan.StartsWith('/')
            || duongDan.Contains(':')
            || duongDan.Split('/').Contains(".."))
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Đường dẫn dữ liệu phải tương đối trong thư mục ứng dụng.");
        }

        // Giữ đúng quy tắc của phiên bản bài tập hiện tại.
        if (GioDongCua != new TimeOnly(17, 0))
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Giờ đóng cửa phải là 17:00 theo thiết kế.");
        }

        if (DonGiaPhat != 5000m)
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Đơn giá phạt phải là 5.000 đồng/ngày theo thiết kế.");
        }

        if (NgayNghiDacBiet is null)
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Danh sách ngày nghỉ không được null.");
        }

        if (NgayNghiDacBiet.Distinct().Count()
            != NgayNghiDacBiet.Count)
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Danh sách ngày nghỉ có ngày bị trùng.");
        }
    }
}