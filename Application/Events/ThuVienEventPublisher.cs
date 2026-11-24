namespace LibraryManagementSystem.Application.Events;

public sealed class ThuVienEventPublisher
{
    public event Action<string>? DaLuu;
    public void Phat(string hanhDong) => DaLuu?.Invoke(hanhDong);
}
