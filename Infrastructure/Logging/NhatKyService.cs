namespace LibraryManagementSystem.Infrastructure.Logging;

public sealed class NhatKyService : INhatKyService
{
    private readonly ITransactionManager tx; public NhatKyService(ITransactionManager tx)
    {
        this.tx = tx;
    }
    public void Ghi(NhatKyThaoTac nk)
    {
        nk.GhiLaiThaoTac();
        var repo = tx.Kho<NhatKyThaoTac>();
        if (!repo.LayTatCa().Any(x => x.MaSuKien == nk.MaSuKien && x.HanhDong == nk.HanhDong))
            repo.Luu(nk);
    }
}
