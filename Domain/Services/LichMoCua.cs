namespace LibraryManagementSystem.Domain.Services;

public sealed class LichMoCua : ILichMoCua
{
    private static readonly TimeSpan GioVietNam = TimeSpan.FromHours(7);
    private readonly HashSet<DateOnly> ngayNghi;
    private readonly TimeOnly gioDongCua;

    public LichMoCua(IEnumerable<DateOnly>? ngayNghiDacBiet = null, TimeOnly? gioDongCua = null)
    {
        ngayNghi = ngayNghiDacBiet?.ToHashSet() ?? [];
        this.gioDongCua = gioDongCua ?? new TimeOnly(17, 0);
    }

    public bool LaNgayMoCua(DateOnly ngay) =>
        ngay.DayOfWeek != DayOfWeek.Sunday && !ngayNghi.Contains(ngay);

    public DateOnly NgayMoTiep(DateOnly ngay) => CongNgayMoCua(ngay, 1);

    public DateOnly CongNgayMoCua(DateOnly ngayBatDau, int soNgay)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(soNgay);

        DateOnly ngay = ngayBatDau;
        while (soNgay > 0)
        {
            ngay = ngay.AddDays(1);
            if (LaNgayMoCua(ngay))
                soNgay--;
        }

        return ngay;
    }

    public int DemNgayMoCua(DateOnly tuNgay, DateOnly denNgay)
    {
        int soNgay = 0;
        while (tuNgay < denNgay)
        {
            tuNgay = tuNgay.AddDays(1);
            if (LaNgayMoCua(tuNgay))
                soNgay++;
        }

        return soNgay;
    }

    public DateTimeOffset HanNhanSauHaiNgay(DateTimeOffset lucCap)
    {
        DateOnly ngayCap = DateOnly.FromDateTime(lucCap.ToOffset(GioVietNam).DateTime);
        DateOnly ngayHetHan = CongNgayMoCua(ngayCap, 2);
        return new DateTimeOffset(ngayHetHan.ToDateTime(gioDongCua), GioVietNam);
    }
}
