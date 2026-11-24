namespace LibraryManagementSystem.Domain.Entities;

public sealed record KeSach
{
    public required string MaKe { get; init; }
    public ViTriKe ViTri { get; init; } = new("", "");
    public bool DangSuDung { get; init; }
}
