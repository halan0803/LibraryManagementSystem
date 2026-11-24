namespace LibraryManagementSystem.Domain.Entities;

public sealed record DauSach
{
    public required string MaDauSach { get; init; }
    public string TenSach { get; init; } = "";
    public string PhienBan { get; init; } = "";
    public string NgonNgu { get; init; } = "";
    public decimal? GiaBia { get; init; }
    public IReadOnlyList<string> DanhSachMaTacGia { get; init; } = [];
    public string MaTheLoai { get; init; } = "";
    public string? MaNXB { get; init; }
}
