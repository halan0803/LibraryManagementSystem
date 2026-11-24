namespace LibraryManagementSystem.Domain.Entities;

public sealed record TaiKhoan
{
    public required string MaTaiKhoan { get; init; }
    public string TenDangNhap { get; init; } = "";
    public string MatKhauBam { get; init; } = "";
    public VaiTro VaiTro { get; init; }
    public TrangThaiTaiKhoan TrangThai { get; init; }
    public string? MaDocGia { get; init; }
    public string? MaNhanVien { get; init; }
}
