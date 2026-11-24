namespace LibraryManagementSystem.Domain.Entities;

public sealed record NhaXuatBan
{
    public required string MaNXB { get; init; }
    public string TenNXB { get; init; } = "";
}
