namespace LibraryManagementSystem.Application.Events;

public interface IThuVienObserver
{
    string Ten
    {
        get;
    }
    void CapNhat(SuKienThuVien suKien);
}
