namespace LibraryManagementSystem.Domain.Entities;

public class BienNhanThayThe
{
    [JsonInclude] public string MaBienNhan { get; private set; } = null!;
    [JsonInclude] public string MaCuonCu { get; private set; } = null!;
    [JsonInclude] public string MaCuonMoi { get; private set; } = null!;
    [JsonInclude] public string MaSuCo { get; private set; } = null!;
    [JsonInclude] public string MaPheDuyet { get; private set; } = null!;
    [JsonInclude] public string MaNguoiNhan { get; private set; } = null!;
    [JsonInclude]
    public DateOnly NgayNhan
    {
        get; private set;
    }
    [JsonConstructor]
    internal BienNhanThayThe()
    {
    }
    public BienNhanThayThe(string ma, string cu, string moi, string suCo, string pheDuyet, string nguoi, DateOnly ngay)
    {
        MaBienNhan = Chuoi(ma, nameof(ma));
        MaCuonCu = Chuoi(cu, nameof(cu));
        MaCuonMoi = Chuoi(moi, nameof(moi));
        YeuCau(cu != moi, "BAN_MOI", "Bản thay thế phải có mã mới.");
        MaSuCo = Chuoi(suCo, nameof(suCo));
        MaPheDuyet = Chuoi(pheDuyet, nameof(pheDuyet));
        MaNguoiNhan = Chuoi(nguoi, nameof(nguoi));
        NgayNhan = ngay;
    }
}
