namespace LibraryManagementSystem.Domain.Entities;

public sealed record ChiTietThu
{
    public required string MaDongThu { get; init; }
    public string MaKhoan { get; init; } = "";
    public decimal SoTien { get; init; }
    public string MaPhieuThu { get; init; } = "";
}
