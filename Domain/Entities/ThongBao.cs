namespace LibraryManagementSystem.Domain.Entities;

public class ThongBao
{
    [JsonInclude] public string MaThongBao { get; private set; } = null!;
    [JsonInclude] public string NguoiNhan { get; private set; } = null!;
    [JsonInclude] public string NoiDung { get; private set; } = null!;
    [JsonInclude]
    public TrangThaiThongBao TrangThai
    {
        get; private set;
    }
    [JsonInclude]
    public LoaiThongBao Loai
    {
        get; private set;
    }
    [JsonInclude] public string MaSuKien { get; private set; } = null!;
    [JsonInclude]
    public DateTimeOffset ThoiDiemTao
    {
        get; private set;
    }
    [JsonInclude]
    public int SoLanGui
    {
        get; private set;
    }
    [JsonConstructor]
    internal ThongBao()
    {
    }

    public ThongBao(string ma, string nguoiNhan, string noiDung, LoaiThongBao loai, string suKien, DateTimeOffset luc)
    {
        MaThongBao = Chuoi(ma, nameof(ma));
        NguoiNhan = Chuoi(nguoiNhan, nameof(nguoiNhan));
        NoiDung = Chuoi(noiDung, nameof(noiDung));
        Loai = loai;
        MaSuKien = Chuoi(suKien, nameof(suKien));
        ThoiDiemTao = luc;
    }
    public void DanhDauDaGui()
    {
        TrangThai = TrangThaiThongBao.DaGui;
        SoLanGui++;
    }
    public void YeuCauGuiLai()
    {
        YeuCau(TrangThai != TrangThaiThongBao.DaGui, "DA_GUI", "Thông báo đã gửi.");
        TrangThai = TrangThaiThongBao.ChoGui;
    }
    public void DanhDauLoi()
    {
        TrangThai = TrangThaiThongBao.GuiLoi;
        SoLanGui++;
    }

}
