namespace LibraryManagementSystem.Domain.Entities;

public sealed record TheLoai
{
    public required string MaTheLoai { get; init; }
    public string TenTheLoai { get; init; } = "";
}
