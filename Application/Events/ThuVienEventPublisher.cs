namespace LibraryManagementSystem.Application.Events;

public sealed class ThuVienEventPublisher
{
    private event Action<SuKienThuVien>? phatSinh;
    public void DangKy(IThuVienObserver observer) => phatSinh += observer.CapNhat;
    public void HuyDangKy(IThuVienObserver observer) => phatSinh -= observer.CapNhat;
    public void GhiNhan(SuKienThuVien suKien) => phatSinh?.Invoke(suKien);

    public event Action<string>? DaLuu;
    public void Phat(string hanhDong) => DaLuu?.Invoke(hanhDong);
}

