namespace LibraryManagementSystem.Application.Observers;

public sealed class NhatKyObserver : IThuVienObserver
{
    private readonly INhatKyService nhatKy; public string Ten => nameof(NhatKyObserver);
    public NhatKyObserver(INhatKyService nhatKy)
    {
        this.nhatKy = nhatKy;
    }
    public void CapNhat(SuKienThuVien e) => nhatKy.Ghi(new NhatKyThaoTac(MaMoi(), e.MaTaiKhoanThucHien, e.LoaiSuKien, e.ThoiDiem, "SuKien", e.MaDoiTuong, e.MaSuKien, e.NoiDung));
}
