namespace LibraryManagementSystem.Domain.Entities;

public sealed record DotGiuCho
{
    public required string MaDot { get; init; }
    public string MaCuon { get; init; } = "";
    public string MaDongNguon { get; init; } = "";
    public TrangThaiDotGiuCho TrangThai { get; init; }
    public string MaSuKien { get; init; } = "";
    public IReadOnlyList<LuotGiuCho> DanhSachLuot { get; init; } = [];
}
