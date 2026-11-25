namespace LibraryManagementSystem.Domain.Exceptions;

internal static class KiemTra
{
    public static void YeuCau(
        bool dieuKien,
        string maLoi,
        string thongBao)
    {
        if (!dieuKien)
        {
            throw new NghiepVuException(maLoi, thongBao);
        }
    }

    public static string Chuoi(string? giaTri, string ten)
    {
        YeuCau(
            !string.IsNullOrWhiteSpace(giaTri),
            "DU_LIEU",
            $"{ten} không được để trống.");

        return giaTri!.Trim();
    }

    public static T BatBuoc<T>(T? giaTri, string ten)
        where T : class
    {
        if (giaTri is null)
        {
            throw new NghiepVuException(
                "KHONG_TIM_THAY",
                $"Không tìm thấy {ten}.");
        }

        return giaTri;
    }

    public static string MaMoi()
    {
        return Guid.NewGuid().ToString("N");
    }
}