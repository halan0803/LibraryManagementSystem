namespace LibraryManagementSystem.Infrastructure.Logging;

internal sealed class NhatKyService(ITransactionManager db) : INhatKyService
{
    public void Ghi(NhatKyThaoTac banGhi) => db.Kho<NhatKyThaoTac>().Luu(banGhi);
}
