namespace LibraryManagementSystem.Domain.Entities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$loaiDocGia")]
[JsonDerivedType(typeof(SinhVien), "sinhVien")]
[JsonDerivedType(typeof(GiangVien), "giangVien")]
public abstract class DocGia
{
    protected DocGia(string maDocGia, string maTruong, string hoTen)
    {
        MaDocGia = Chuoi(maDocGia, nameof(maDocGia));
        MaTruong = Chuoi(maTruong, nameof(maTruong));
        HoTen = Chuoi(hoTen, nameof(hoTen));
    }

    public string MaDocGia { get; }
    public string MaTruong { get; }
    public string HoTen { get; }
    public DateOnly? NgayRoiTruong { get; init; }
    public abstract LoaiDocGia LoaiDocGia { get; }

    [JsonInclude]
    public TrangThaiDocGia TinhTrangQuyen { get; private set; }

    [JsonInclude]
    public string? LyDoDungQuyen { get; private set; }

    public abstract int SoNgayMuon();
    public abstract int GioiHanCuon();
    public bool DuocSuDung() => TinhTrangQuyen == TrangThaiDocGia.HoatDong;

    public void DungQuyen(string lyDo)
    {
        YeuCau(TinhTrangQuyen != TrangThaiDocGia.DaDong, "HO_SO_DA_DONG", "Hồ sơ đã đóng.");
        LyDoDungQuyen = Chuoi(lyDo, nameof(lyDo));
        TinhTrangQuyen = TrangThaiDocGia.DungQuyen;
    }
}
