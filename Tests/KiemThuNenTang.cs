using System.Text.Json;
using LibraryManagementSystem.Application.Interfaces;
using LibraryManagementSystem.Domain.Exceptions;
using LibraryManagementSystem.Infrastructure.Configuration;

namespace LibraryManagementSystem.Tests;

public static class KiemThuNenTang
{
    public static int RunAll()
    {
        int soDat = 0;
        int soLoi = 0;

        // Dùng thư mục tạm để không sửa cấu hình thật của dự án.
        string thuMucTam = Path.Combine(
            Path.GetTempPath(),
            "LibraryTests-" + Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(thuMucTam);

        string fileTam = Path.Combine(
            thuMucTam, "appsettings.json");

        void Chay(string ten, Action kiemThu)
        {
            try
            {
                kiemThu();
                soDat++;
                Console.WriteLine($"[PASS] {ten}");
            }
            catch (Exception ex)
            {
                soLoi++;
                Console.WriteLine($"[FAIL] {ten}");
                Console.WriteLine($"       {ex.Message}");
            }
        }

        void GhiCauHinh(CauHinhThuVien cauHinh)
        {
            string json = JsonSerializer.Serialize(cauHinh);
            File.WriteAllText(fileTam, json);
        }

        Console.WriteLine("===== KIEM THU NEN TANG =====");

        try
        {
            Chay("01. Đọc cấu hình thật và khởi tạo ứng dụng", () =>
            {
                var app = new AppBootstrapper(
                    AppContext.BaseDirectory);

                XacNhan(
                    app.CauHinh.DonGiaPhat == 5000m,
                    "Đơn giá phạt phải là 5000.");

                XacNhan(
                    app.CauHinh.GioDongCua == new TimeOnly(17, 0),
                    "Giờ đóng cửa phải là 17:00.");

                XacNhan(
                    Path.IsPathRooted(app.DuongDanDuLieu),
                    "Đường dẫn dữ liệu sau khởi tạo phải là tuyệt đối.");
            });

            Chay("02. Đọc đúng ngày nghỉ từ JSON hợp lệ", () =>
            {
                GhiCauHinh(new CauHinhThuVien
                {
                    NgayNghiDacBiet = new List<DateOnly>
                    {
                        new DateOnly(2026, 1, 1)
                    }
                });

                var cauHinh = ConfigurationLoader.Tai(fileTam);

                XacNhan(
                    cauHinh.NgayNghiDacBiet.Count == 1
                    && cauHinh.NgayNghiDacBiet[0]
                        == new DateOnly(2026, 1, 1),
                    "Ngày nghỉ đọc từ JSON không đúng.");
            });

            Chay("03. Báo lỗi khi thiếu file cấu hình", () =>
            {
                string fileKhongTonTai = Path.Combine(
                    thuMucTam, "khong-ton-tai.json");

                MongDoiLoi(
                    () => ConfigurationLoader.Tai(fileKhongTonTai),
                    "CAU_HINH_DOC_FILE");
            });

            Chay("04. Báo lỗi khi JSON sai cú pháp", () =>
            {
                File.WriteAllText(fileTam, "{ JSON sai");

                MongDoiLoi(
                    () => ConfigurationLoader.Tai(fileTam),
                    "CAU_HINH_JSON");
            });

            Chay("05. Từ chối mức phạt sai quy định", () =>
            {
                GhiCauHinh(new CauHinhThuVien
                {
                    DonGiaPhat = 1000m
                });

                MongDoiLoi(
                    () => ConfigurationLoader.Tai(fileTam),
                    "CAU_HINH");
            });

            Chay("06. Từ chối giờ đóng cửa sai quy định", () =>
            {
                GhiCauHinh(new CauHinhThuVien
                {
                    GioDongCua = new TimeOnly(18, 0)
                });

                MongDoiLoi(
                    () => ConfigurationLoader.Tai(fileTam),
                    "CAU_HINH");
            });

            Chay("07. Từ chối đường dẫn đi ra ngoài thư mục gốc", () =>
            {
                GhiCauHinh(new CauHinhThuVien
                {
                    DuongDanDuLieu = "../thu-vien.json"
                });

                MongDoiLoi(
                    () => ConfigurationLoader.Tai(fileTam),
                    "CAU_HINH");
            });

            Chay("08. Kiểm tra chuỗi hợp lệ và chuỗi trống", () =>
            {
                string ketQua = KiemTra.Chuoi("  DG01  ", "Mã độc giả");

                XacNhan(
                    ketQua == "DG01",
                    "Chưa loại bỏ khoảng trắng đầu và cuối.");

                MongDoiLoi(
                    () => KiemTra.Chuoi("   ", "Mã độc giả"),
                    "DU_LIEU");
            });

            Chay("09. Báo lỗi khi đối tượng bắt buộc không tồn tại", () =>
            {
                MongDoiLoi(
                    () => KiemTra.BatBuoc<object>(null, "độc giả"),
                    "KHONG_TIM_THAY");
            });

            Chay("10. Ngoại lệ giữ mã lỗi, thông báo và lỗi gốc", () =>
            {
                var loiGoc = new IOException("Lỗi đọc thử nghiệm.");

                var loi = new NghiepVuException(
                    "DOC_FILE",
                    "Không đọc được dữ liệu.",
                    loiGoc);

                XacNhan(
                    loi.MaLoi == "DOC_FILE",
                    "Mã lỗi không đúng.");

                XacNhan(
                    loi.Message == "Không đọc được dữ liệu.",
                    "Thông báo lỗi không đúng.");

                XacNhan(
                    ReferenceEquals(loi.InnerException, loiGoc),
                    "Không giữ được lỗi gốc.");
            });

            Chay("11. Đồng hồ tính đúng ngày Việt Nam khi UTC chưa sang ngày", () =>
            {
                // 18:00 ngày 24 UTC = 01:00 ngày 25 tại Việt Nam.
                IDongHo dongHo = new DongHoGiaLap(
                    new DateTimeOffset(
                        2026, 11, 24, 18, 0, 0, TimeSpan.Zero));

                XacNhan(
                    dongHo.HomNay == new DateOnly(2026, 11, 25),
                    "Chưa chuyển ngày theo giờ Việt Nam.");
            });

            Chay("12. Khởi tạo dùng đúng đồng hồ và thư mục được truyền vào", () =>
            {
                GhiCauHinh(new CauHinhThuVien());

                IDongHo dongHo = new DongHoGiaLap(
                    new DateTimeOffset(
                        2026, 11, 25, 10, 0, 0,
                        TimeSpan.FromHours(7)));

                var app = new AppBootstrapper(
                    thuMucTam, dongHo);

                XacNhan(
                    ReferenceEquals(app.DongHo, dongHo),
                    "Ứng dụng không dùng đồng hồ được truyền vào.");

                string duongDanMongDoi = Path.GetFullPath(
                    Path.Combine(
                        thuMucTam, "Data", "thu-vien.json"));

                XacNhan(
                    app.DuongDanDuLieu == duongDanMongDoi,
                    "Đường dẫn dữ liệu không dựa trên thư mục gốc.");
            });
        }
        finally
        {
            // Chỉ xóa thư mục tạm do lần kiểm thử này tạo.
            Directory.Delete(thuMucTam, recursive: true);
        }

        Console.WriteLine();
        Console.WriteLine($"Kết quả: {soDat} PASS, {soLoi} FAIL");

        return soLoi;
    }

    private static void XacNhan(bool dieuKien, string thongBao)
    {
        if (!dieuKien)
        {
            throw new Exception(thongBao);
        }
    }

    private static void MongDoiLoi(
        Action thaoTac,
        string maLoiMongDoi)
    {
        try
        {
            thaoTac();
        }
        catch (NghiepVuException ex)
        {
            XacNhan(
                ex.MaLoi == maLoiMongDoi,
                $"Mong đợi mã lỗi {maLoiMongDoi}, nhận được {ex.MaLoi}.");

            return;
        }

        // Không phát sinh lỗi cũng là thất bại đối với ca này.
        throw new Exception(
            $"Mong đợi lỗi {maLoiMongDoi} nhưng thao tác không báo lỗi.");
    }

    private sealed class DongHoGiaLap : IDongHo
    {
        public DateTimeOffset BayGio { get; }

        public DongHoGiaLap(DateTimeOffset bayGio)
        {
            BayGio = bayGio;
        }
    }
}