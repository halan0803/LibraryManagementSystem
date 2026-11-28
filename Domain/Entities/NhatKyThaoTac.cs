namespace LibraryManagementSystem.Domain.Entities;

public class NhatKyThaoTac
{
    [JsonInclude] public string MaNhatKy { get; private set; } = null!;
    [JsonInclude]
    public string? MaTaiKhoanThucHien
    {
        get; private set;
    }
    [JsonInclude] public string HanhDong { get; private set; } = null!;
    [JsonInclude]
    public DateTimeOffset ThoiDiem
    {
        get; private set;
    }
    [JsonInclude]
    public string? LyDo
    {
        get; private set;
    }
    [JsonInclude] public string LoaiDoiTuong { get; private set; } = null!;
    [JsonInclude] public string MaDoiTuong { get; private set; } = null!;
    [JsonInclude]
    public string? DuLieuTruoc
    {
        get; private set;
    }
    [JsonInclude]
    public string? DuLieuSau
    {
        get; private set;
    }
    [JsonInclude]
    public LoaiChuTheThaoTac LoaiChuThe
    {
        get; private set;
    }
    [JsonInclude] public string MaSuKien { get; private set; } = null!;
    [JsonConstructor]
    internal NhatKyThaoTac()
    {
    }

    public NhatKyThaoTac(string ma, string? taiKhoan, string hanhDong, DateTimeOffset luc, string loaiDoiTuong, string doiTuong, string suKien, string? lyDo = null, string? truoc = null, string? sau = null)
    {
        MaNhatKy = Chuoi(ma, nameof(ma));
        MaTaiKhoanThucHien = taiKhoan;
        HanhDong = Chuoi(hanhDong, nameof(hanhDong));
        ThoiDiem = luc;
        LoaiDoiTuong = Chuoi(loaiDoiTuong, nameof(loaiDoiTuong));
        MaDoiTuong = Chuoi(doiTuong, nameof(doiTuong));
        MaSuKien = Chuoi(suKien, nameof(suKien));
        LyDo = lyDo;
        DuLieuTruoc = truoc;
        DuLieuSau = sau;
        LoaiChuThe = taiKhoan is null ? LoaiChuTheThaoTac.HeThong : LoaiChuTheThaoTac.NguoiDung;
    }
    public void GhiLaiThaoTac()
    {
        YeuCau(!string.IsNullOrWhiteSpace(HanhDong), "NHAT_KY", "Thiếu hành động nhật ký.");
    }

}
