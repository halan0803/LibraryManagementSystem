namespace LibraryManagementSystem.Domain.Entities;

public sealed record PheDuyet
{
    public required string MaPheDuyet { get; init; }
    public string MaNguoiDuyet { get; init; } = "";
    public string CanCu { get; init; } = "";
    public QuyetDinhPheDuyet QuyetDinh { get; init; }
    public DateTimeOffset? NgayDuyet { get; init; }
    public LoaiPheDuyet LoaiPheDuyet { get; init; }
    public string LoaiDoiTuong { get; init; } = "";
    public string MaDoiTuong { get; init; } = "";
    public string? LyDo { get; init; }
}
