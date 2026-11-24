namespace LibraryManagementSystem.Domain.Entities;

public sealed record TheThuVien
{
    public required string MaThe { get; init; }
    public string MaDocGia { get; init; } = "";
    public DateOnly NgayCap { get; init; }
    public DateOnly NgayHetHan { get; init; }
    public TrangThaiThe TrangThai { get; init; }
    public DateOnly NgayHieuLuc { get; init; }
    public string? MaTheTruoc { get; init; }
}
