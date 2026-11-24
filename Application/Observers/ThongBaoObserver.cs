using LibraryManagementSystem.Application.Events;

namespace LibraryManagementSystem.Application.Observers;

internal sealed class ThongBaoObserver(IKenhThongBao dichVu) : IThuVienObserver
{
    public void CapNhat(SuKienThuVien suKien)
    {
        if (suKien.ThongBao is { } banGhi) dichVu.Gui(banGhi);
    }
}
