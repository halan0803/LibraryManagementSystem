namespace LibraryManagementSystem.Domain.Entities;

public class PheDuyet
{
    [JsonInclude] public string MaPheDuyet { get; private set; } = null!;
    [JsonInclude] public string MaNguoiDuyet { get; private set; } = null!;
    [JsonInclude] public string CanCu { get; private set; } = null!;
    [JsonInclude]
    public QuyetDinhPheDuyet QuyetDinh
    {
        get; private set;
    }
    [JsonInclude]
    public DateTimeOffset? NgayDuyet
    {
        get; private set;
    }
    [JsonInclude]
    public LoaiPheDuyet LoaiPheDuyet
    {
        get; private set;
    }
    [JsonInclude] public string LoaiDoiTuong { get; private set; } = null!;
    [JsonInclude] public string MaDoiTuong { get; private set; } = null!;
    [JsonInclude]
    public string? LyDo
    {
        get; private set;
    }
    [JsonConstructor]
    internal PheDuyet()
    {
    }

    public PheDuyet(string ma, string nguoi, string canCu, LoaiPheDuyet loai, string loaiDoiTuong, string doiTuong, DateTimeOffset luc)
    {
        MaPheDuyet = Chuoi(ma, nameof(ma));
        MaNguoiDuyet = Chuoi(nguoi, nameof(nguoi));
        CanCu = Chuoi(canCu, nameof(canCu));
        LoaiPheDuyet = loai;
        LoaiDoiTuong = Chuoi(loaiDoiTuong, nameof(loaiDoiTuong));
        MaDoiTuong = Chuoi(doiTuong, nameof(doiTuong));
        NgayDuyet = luc;
    }
    [JsonInclude] public string? MaDauSachThayThe { get; private set; }

    internal void GanBanThayThe(string maDauSach) => MaDauSachThayThe = Chuoi(maDauSach, nameof(maDauSach));

    public void ChapThuan() => QuyetDinh = QuyetDinhPheDuyet.ChapThuan;
    public void TuChoi(string lyDo)
    {
        LyDo = Chuoi(lyDo, nameof(lyDo));
        QuyetDinh = QuyetDinhPheDuyet.TuChoi;
    }

}
