using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.Exceptions;

namespace LibraryManagementSystem.Tests;

public static class KiemThuTheThuVien
{
    public static int RunAll()
    {
        int soCaDat = 0;
        int soCaLoi = 0;

        void Chay(string ten, Action kiemThu)
        {
            try
            {
                kiemThu();
                Console.WriteLine($"PASS: {ten}");
                soCaDat++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL: {ten} — {ex.Message}");
                soCaLoi++;
            }
        }

        Chay("Thời hạn và ranh giới hiệu lực", () =>
        {
            var the = new TheThuVien(
                "THE01", "DG01", new DateOnly(2026, 1, 1));

            XacNhan(
                the.NgayHetHan == new DateOnly(2026, 7, 1),
                "Thẻ phải có thời hạn 6 tháng.");

            XacNhan(
                the.ConHieuLuc(new DateOnly(2026, 7, 1)),
                "Thẻ phải còn hiệu lực trong ngày hết hạn.");

            XacNhan(
                !the.ConHieuLuc(new DateOnly(2026, 7, 2)),
                "Thẻ phải hết hiệu lực từ ngày tiếp theo.");
        });

        Chay("Gia hạn hợp lệ tính từ hạn cũ", () =>
        {
            var the = new TheThuVien(
                "THE02", "DG02", new DateOnly(2026, 1, 1));

            the.GiaHanThe(new DateOnly(2026, 6, 15));

            XacNhan(
                the.NgayHetHan == new DateOnly(2027, 1, 1),
                "Phải cộng 6 tháng từ hạn cũ, không từ ngày gia hạn.");
        });

        Chay("Gia hạn quá sớm bị từ chối và không đổi dữ liệu", () =>
        {
            var the = new TheThuVien(
                "THE03", "DG03", new DateOnly(2026, 1, 1));

            DateOnly hanCu = the.NgayHetHan;
            bool daTuChoi = false;

            try
            {
                the.GiaHanThe(new DateOnly(2026, 3, 1));
            }
            catch (NghiepVuException ex)
            {
                XacNhan(
                    ex.MaLoi == "NGOAI_CUA_SO",
                    $"Mong đợi NGOAI_CUA_SO nhưng nhận {ex.MaLoi}.");

                daTuChoi = true;
            }

            XacNhan(daTuChoi, "Phải từ chối gia hạn quá sớm.");
            XacNhan(
                the.NgayHetHan == hanCu,
                "Thao tác bị từ chối phải giữ nguyên hạn cũ.");
        });

        Chay("Khôi phục tính từ ngày xử lý", () =>
        {
            var the = new TheThuVien(
                "THE04", "DG04", new DateOnly(2026, 1, 1));

            the.KhoiPhucThe(new DateOnly(2026, 7, 10));

            XacNhan(
                the.NgayHieuLuc == new DateOnly(2026, 7, 10),
                "Ngày hiệu lực phải là ngày khôi phục.");

            XacNhan(
                the.NgayHetHan == new DateOnly(2027, 1, 10),
                "Hạn mới phải bằng ngày khôi phục cộng 6 tháng.");
        });

        Console.WriteLine(
            $"Kiểm thử lớp thẻ: {soCaDat} PASS, {soCaLoi} FAIL");

        return soCaLoi;
    }

    private static void XacNhan(bool dieuKien, string thongBao)
    {
        if (!dieuKien)
        {
            throw new Exception(thongBao);
        }
    }
}