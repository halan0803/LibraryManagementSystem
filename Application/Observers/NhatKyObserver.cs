using LibraryManagementSystem.Application.Events;

namespace LibraryManagementSystem.Application.Observers;

internal sealed class NhatKyObserver(INhatKyService dichVu) : IThuVienObserver
{
    public void CapNhat(SuKienThuVien suKien)
    {
        if (suKien.NhatKy is { } banGhi) dichVu.Ghi(banGhi);
    }
}
