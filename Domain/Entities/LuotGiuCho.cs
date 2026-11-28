namespace LibraryManagementSystem.Domain.Entities;

public class LuotGiuCho
{
    [JsonInclude] public string MaLuot { get; private set; } = null!;
    [JsonInclude]
    public int ThuTu
    {
        get; private set;
    }
    [JsonInclude]
    public DateTimeOffset? ThoiDiemKichHoat
    {
        get; private set;
    }
    [JsonInclude]
    public DateTimeOffset? HanNhan
    {
        get; private set;
    }
    [JsonInclude]
    public TrangThaiLuotGiuCho TrangThai
    {
        get; private set;
    }
    [JsonInclude] public string MaDangKy { get; private set; } = null!;
    [JsonInclude] public string MaDot { get; private set; } = null!;
    [JsonConstructor]
    internal LuotGiuCho()
    {
    }

    public LuotGiuCho(string ma, string dangKy, string dot, int thuTu)
    {
        MaLuot = Chuoi(ma, nameof(ma));
        MaDangKy = Chuoi(dangKy, nameof(dangKy));
        MaDot = Chuoi(dot, nameof(dot));
        YeuCau(thuTu is 1 or 2, "THU_TU", "Đợt chỉ có hai lượt.");
        ThuTu = thuTu;
    }
    public void KichHoat(DateTimeOffset luc, ILichMoCua lich)
    {
        YeuCau(TrangThai == TrangThaiLuotGiuCho.ChuaKichHoat, "LUOT_CU", "Lượt không thể kích hoạt.");
        ThoiDiemKichHoat = luc;
        HanNhan = lich.HanNhanSauHaiNgay(luc);
        TrangThai = TrangThaiLuotGiuCho.DangCoQuyen;
    }
    public void DanhDauHetLuot()
    {
        YeuCau(TrangThai == TrangThaiLuotGiuCho.DangCoQuyen, "LUOT_CU", "Lượt không có quyền nhận.");
        TrangThai = TrangThaiLuotGiuCho.HetLuot;
    }
    public void XacNhanDaNhan()
    {
        YeuCau(TrangThai == TrangThaiLuotGiuCho.DangCoQuyen, "LUOT_CU", "Lượt không có quyền nhận.");
        TrangThai = TrangThaiLuotGiuCho.DaNhan;
    }
    internal void Huy()
    {
        if (TrangThai is TrangThaiLuotGiuCho.ChuaKichHoat or TrangThaiLuotGiuCho.DangCoQuyen or TrangThaiLuotGiuCho.TamDung)
            TrangThai = TrangThaiLuotGiuCho.DaHuy;
    }
    internal void TamDung()
    {
        if (TrangThai is TrangThaiLuotGiuCho.ChuaKichHoat or TrangThaiLuotGiuCho.DangCoQuyen)
            TrangThai = TrangThaiLuotGiuCho.TamDung;
    }

}
