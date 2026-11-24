namespace LibraryManagementSystem.Domain.Entities;

public sealed record ThongBao
{
    public required string MaThongBao { get; init; }
    public string NguoiNhan { get; init; } = "";
    public string NoiDung { get; init; } = "";
    public TrangThaiThongBao TrangThai { get; init; }
    public LoaiThongBao Loai { get; init; }
    public string MaSuKien { get; init; } = "";
    public DateTimeOffset ThoiDiemTao { get; init; }
    public int SoLanGui { get; init; }
}
