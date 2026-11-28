namespace LibraryManagementSystem.Domain.Entities;

public class BienBanThanhLy
{
    [JsonInclude] public string MaThanhLy { get; private set; } = null!;
    [JsonInclude] public string MaCuon { get; private set; } = null!;
    [JsonInclude] public string LyDo { get; private set; } = null!;
    [JsonInclude]
    public DateOnly NgayThanhLy
    {
        get; private set;
    }
    [JsonInclude] public string MaNguoiDuyet { get; private set; } = null!;
    [JsonInclude] public string MaPheDuyet { get; private set; } = null!;
    [JsonInclude]
    public string? MaSuCo
    {
        get; private set;
    }
    [JsonConstructor]
    internal BienBanThanhLy()
    {
    }
    public BienBanThanhLy(string ma, string cuon, string lyDo, DateOnly ngay, string nguoi, string pheDuyet, string? suCo)
    {
        MaThanhLy = Chuoi(ma, nameof(ma));
        MaCuon = Chuoi(cuon, nameof(cuon));
        LyDo = Chuoi(lyDo, nameof(lyDo));
        NgayThanhLy = ngay;
        MaNguoiDuyet = Chuoi(nguoi, nameof(nguoi));
        MaPheDuyet = Chuoi(pheDuyet, nameof(pheDuyet));
        MaSuCo = suCo;
    }
    public void XacNhanThanhLy() => Chuoi(MaPheDuyet, nameof(MaPheDuyet));
}
