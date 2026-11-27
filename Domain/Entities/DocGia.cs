namespace LibraryManagementSystem.Domain.Entities;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$loai")]
[JsonDerivedType(typeof(SinhVien), "SinhVien")]
[JsonDerivedType(typeof(GiangVien), "GiangVien")]
public abstract class DocGia
{
    [JsonInclude] public string MaDocGia { get; private set; } = null!;
    [JsonInclude] public string MaTruong { get; private set; } = null!;
    [JsonInclude] public string HoTen { get; private set; } = null!;
    [JsonInclude]
    public TrangThaiDocGia TinhTrangQuyen
    {
        get; private set;
    }
    [JsonInclude]
    public DateOnly? NgayRoiTruong
    {
        get; private set;
    }
    [JsonInclude]
    public string? LyDoDungQuyen
    {
        get; private set;
    }
    public abstract LoaiDocGia LoaiDocGia
    {
        get;
    }
    protected DocGia()
    {
    }
    protected DocGia(string ma, string truong, string ten)
    {
        MaDocGia = Chuoi(ma, nameof(ma));
        MaTruong = Chuoi(truong, nameof(truong));
        HoTen = Chuoi(ten, nameof(ten));
    }
    public abstract int SoNgayMuon(); public abstract int GioiHanCuon();
    public bool DuocSuDung() => TinhTrangQuyen == TrangThaiDocGia.HoatDong;
    public void DungQuyen(string lyDo)
    {
        YeuCau(TinhTrangQuyen != TrangThaiDocGia.DaDong, "HO_SO_DONG", "Hồ sơ đã đóng.");
        LyDoDungQuyen = Chuoi(lyDo, nameof(lyDo));
        TinhTrangQuyen = TrangThaiDocGia.DungQuyen;
    }
    public void GhiNgayRoiTruong(DateOnly ngay) => NgayRoiTruong = ngay;
    public void DongHoSo(string lyDo)
    {
        LyDoDungQuyen = Chuoi(lyDo, nameof(lyDo));
        TinhTrangQuyen = TrangThaiDocGia.DaDong;
    }
}