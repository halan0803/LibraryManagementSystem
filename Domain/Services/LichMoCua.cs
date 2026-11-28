namespace LibraryManagementSystem.Domain.Services;

public sealed class LichMoCua : ILichMoCua
{
    private readonly HashSet<DateOnly> ngayNghi; private readonly TimeOnly dongCua;
    public LichMoCua(IEnumerable<DateOnly>? ngayNghi = null, TimeOnly? dongCua = null)
    {
        this.ngayNghi = (ngayNghi ?? []).ToHashSet();
        this.dongCua = dongCua ?? new TimeOnly(17, 0);
    }
    public bool LaNgayMoCua(DateOnly ngay) => ngay.DayOfWeek != DayOfWeek.Sunday && !ngayNghi.Contains(ngay);
    public DateOnly NgayMoTiep(DateOnly ngay) => CongNgayMoCua(ngay, 1);
    public DateOnly CongNgayMoCua(DateOnly ngayBatDau, int soNgay)
    {
        YeuCau(soNgay >= 0, "SO_NGAY", "Số ngày không được âm.");
        var d = ngayBatDau;
        while (soNgay > 0)
        {
            d = d.AddDays(1);
            if (LaNgayMoCua(d))
                soNgay--;
        }
        return d;
    }
    public int DemNgayMoCua(DateOnly tuNgay, DateOnly denNgay)
    {
        int n = 0;
        for (var d = tuNgay; d < denNgay;)
        {
            d = d.AddDays(1);
            if (LaNgayMoCua(d))
                n++;
        }
        return n;
    }
    public DateTimeOffset HanNhanSauHaiNgay(DateTimeOffset lucCap)
    {
        var ngay = DateOnly.FromDateTime(lucCap.ToOffset(TimeSpan.FromHours(7)).DateTime);
        return new DateTimeOffset(CongNgayMoCua(ngay, 2).ToDateTime(dongCua), TimeSpan.FromHours(7));
    }
}
