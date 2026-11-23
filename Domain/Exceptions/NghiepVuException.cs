namespace LibraryManagementSystem.Domain.Exceptions;

public class NghiepVuException : Exception
{
    public string MaLoi { get; }

    public NghiepVuException(string maLoi, string thongBao)
        : base(thongBao)
    {
        if (string.IsNullOrWhiteSpace(maLoi))
        {
            throw new ArgumentException(
                "Mã lỗi không được để trống.",
                nameof(maLoi));
        }

        MaLoi = maLoi.Trim();
    }

    public NghiepVuException(
        string maLoi,
        string thongBao,
        Exception innerException)
        : base(thongBao, innerException)
    {
        if (string.IsNullOrWhiteSpace(maLoi))
        {
            throw new ArgumentException(
                "Mã lỗi không được để trống.",
                nameof(maLoi));
        }

        MaLoi = maLoi.Trim();
    }
}