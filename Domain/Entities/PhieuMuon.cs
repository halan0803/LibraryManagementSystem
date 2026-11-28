namespace LibraryManagementSystem.Domain.Entities;

public class PhieuMuon
{
    [JsonInclude] public string MaPhieu { get; private set; } = null!;
    [JsonInclude] public string MaDocGia { get; private set; } = null!;
    [JsonInclude]
    public DateOnly NgayMuon
    {
        get; private set;
    }
    [JsonInclude] public string MaNguoiLap { get; private set; } = null!;
    [JsonInclude] public string MaYeuCau { get; private set; } = null!;
    [JsonConstructor]
    internal PhieuMuon()
    {
    }
    public PhieuMuon(string ma, string docGia, DateOnly ngay, string nguoiLap, string yeuCau)
    {
        MaPhieu = Chuoi(ma, nameof(ma));
        MaDocGia = Chuoi(docGia, nameof(docGia));
        NgayMuon = ngay;
        MaNguoiLap = Chuoi(nguoiLap, nameof(nguoiLap));
        MaYeuCau = Chuoi(yeuCau, nameof(yeuCau));
    }
}
