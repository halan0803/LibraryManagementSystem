namespace LibraryManagementSystem.Infrastructure.Notifications;
// Gửi trong ứng dụng = lưu thông báo đọc được ở màn hình thông báo.
public sealed class ThongBaoTrongHeThong : IKenhThongBao
{
    private readonly ITransactionManager tx; public ThongBaoTrongHeThong(ITransactionManager tx)
    {
        this.tx = tx;
    }
    public void Gui(ThongBao tb)
    {
        tb.DanhDauDaGui();
        tx.Kho<ThongBao>().CapNhat(tb);
    }
}
