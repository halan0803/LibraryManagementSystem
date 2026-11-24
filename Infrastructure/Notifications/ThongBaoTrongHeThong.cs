namespace LibraryManagementSystem.Infrastructure.Notifications;

internal sealed class ThongBaoTrongHeThong(ITransactionManager db) : IKenhThongBao
{
    public void Gui(ThongBao banGhi) => db.Kho<ThongBao>().Luu(banGhi);
}
