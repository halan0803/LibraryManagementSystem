using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Infrastructure.Configuration;

namespace LibraryManagementSystem;

public sealed class AppBootstrapper
{
    public CauHinhThuVien CauHinh { get; }

    public IDongHo DongHo { get; }

    public string DuongDanDuLieu { get; }

    public AppBootstrapper(
        string thuMucGoc,
        IDongHo? dongHo = null)
    {
        if (string.IsNullOrWhiteSpace(thuMucGoc))
        {
            throw new ArgumentException(
                "Thư mục gốc không được để trống.",
                nameof(thuMucGoc));
        }

        string thuMucDayDu = Path.GetFullPath(thuMucGoc);

        string duongDanCauHinh = Path.Combine(
            thuMucDayDu,
            "appsettings.json");

        CauHinh = ConfigurationLoader.Tai(duongDanCauHinh);

        DongHo = dongHo ?? new DongHoHeThong();

        string duongDanTuongDoi = CauHinh.DuongDanDuLieu
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);

        DuongDanDuLieu = Path.GetFullPath(
            Path.Combine(thuMucDayDu, duongDanTuongDoi));
    }
}