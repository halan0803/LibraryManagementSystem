namespace LibraryManagementSystem.Application.Events;

public sealed record SuKienThuVien(string MaSuKien, string LoaiSuKien, string MaDoiTuong, string? MaDocGia, DateTimeOffset ThoiDiem, string? MaTaiKhoanThucHien, string NoiDung, LoaiThongBao? LoaiThongBao = null);
public sealed class XuLySuKien
{
    public string MaXuLy { get; set; } = null!; public string MaSuKien { get; set; } = null!; public string TenObserver { get; set; } = null!;
    public bool ThanhCong
    {
        get; set;
    }
    public int SoLan
    {
        get; set;
    }
    public string? Loi
    {
        get; set;
    }
}
public sealed record LanSanSangDaXet(string MaLan, string MaCuon, string MaDongNguon, string? MaDot);
