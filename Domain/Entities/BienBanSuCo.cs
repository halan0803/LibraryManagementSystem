namespace LibraryManagementSystem.Domain.Entities;

public class BienBanSuCo
{
    [JsonInclude] public string MaSuCo { get; private set; } = null!;
    [JsonInclude] public string MaDongMuon { get; private set; } = null!;
    [JsonInclude] public LoaiSuCo Loai { get; private set; }
    [JsonInclude] public TrangThaiSuCo TrangThai { get; private set; }
    [JsonInclude] public DateOnly NgayLap { get; private set; }
    [JsonInclude] public string? MinhChung { get; private set; }
    [JsonInclude] public TinhTrangSach? TinhTrang { get; private set; }
    [JsonInclude] public string? MaKhoanBoiThuong { get; private set; }
    [JsonInclude] public decimal? GiaTriChot { get; private set; }
    [JsonInclude] public decimal? TyLeBoiThuong { get; private set; }
    [JsonInclude] public bool DaTimLai { get; private set; }
    [JsonInclude] public string? CanCuBoiThuong { get; private set; }

    [JsonConstructor]
    internal BienBanSuCo() { }

    public BienBanSuCo(string ma, string dong, LoaiSuCo loai, DateOnly ngayLap = default,
        string? minhChung = null, TinhTrangSach? tinhTrang = null)
    {
        MaSuCo = Chuoi(ma, nameof(ma));
        MaDongMuon = Chuoi(dong, nameof(dong));
        Loai = loai;
        NgayLap = ngayLap;
        MinhChung = minhChung;
        TinhTrang = tinhTrang;
    }

    internal void Chot(string? maKhoan, decimal gia, decimal tyLe, string canCu)
    {
        MaKhoanBoiThuong = maKhoan;
        GiaTriChot = gia;
        TyLeBoiThuong = tyLe;
        CanCuBoiThuong = canCu;
        TrangThai = maKhoan is null ? TrangThaiSuCo.HoanTat : TrangThaiSuCo.DangGiaiQuyet;
    }

    internal void GhiTimLai(KhoanThu? khoan)
    {
        DaTimLai = true;
        khoan?.HuyDoTimLai();
        TrangThai = khoan?.TrangThai == TrangThaiKhoanThu.ChoDoiSoat
            ? TrangThaiSuCo.ChoDoiSoat : TrangThaiSuCo.HoanTat;
    }

    public void HoanTat() => TrangThai = TrangThaiSuCo.HoanTat;
}
