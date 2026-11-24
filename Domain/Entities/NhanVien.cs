namespace LibraryManagementSystem.Domain.Entities;

public sealed record NhanVien
{
    public required string MaNhanVien { get; init; }
    public string HoTen { get; init; } = "";
    public VaiTro VaiTro { get; init; }
}
