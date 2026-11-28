namespace LibraryManagementSystem.Application.Events;

public sealed class ThuVienEventPublisher
{
    private readonly ITransactionManager tx; private readonly List<IThuVienObserver> observers = [];
    public string? LoiLuuTrangThai
    {
        get; private set;
    }
    public ThuVienEventPublisher(ITransactionManager tx)
    {
        this.tx = tx;
    }
    public void DangKy(IThuVienObserver observer)
    {
        if (observers.All(x => x.Ten != observer.Ten))
            observers.Add(observer);
    }
    public void HuyDangKy(IThuVienObserver observer) => observers.RemoveAll(x => x.Ten == observer.Ten);
    public void Phat(SuKienThuVien suKien)
    {
        if (tx.DangGiaoDich)
            throw new InvalidOperationException("Chỉ phát sự kiện sau khi giao dịch đã commit.");
        foreach (var o in observers.ToArray())
        {
            try
            {
                tx.BatDau();
                var e = BatBuoc(tx.Kho<SuKienThuVien>().LayTheoMa(suKien.MaSuKien), "sự kiện đã commit");
                var id = e.MaSuKien + ":" + o.Ten;
                var repo = tx.Kho<XuLySuKien>();
                var status = repo.LayTheoMa(id);
                if (status?.ThanhCong == true)
                {
                    tx.Rollback();
                    continue;
                }
                if (status == null)
                {
                    status = new()
                    {
                        MaXuLy = id,
                        MaSuKien = e.MaSuKien,
                        TenObserver = o.Ten
                    };
                    repo.Luu(status);
                }
                status.SoLan++;
                try
                {
                    o.CapNhat(e);
                    status.ThanhCong = true;
                    status.Loi = null;
                }
                catch (Exception ex) { status.ThanhCong = false; status.Loi = ex.GetType().Name + ": " + ex.Message; }
                repo.CapNhat(status);
                tx.Commit();
            }
            catch (Exception ex) { tx.Rollback(); LoiLuuTrangThai = ex.Message; }
        }
    }
    public void XuLyTonDong()
    {
        if (tx.DangGiaoDich)
            throw new InvalidOperationException("Chỉ xử lý sự kiện sau khi giao dịch đã commit.");
        SuKienThuVien[] events;
        try
        {
            tx.BatDau();
            events = tx.Kho<SuKienThuVien>().LayTatCa().OrderBy(x => x.ThoiDiem).ToArray();
            tx.Rollback();
        }
        catch (Exception e) { tx.Rollback(); LoiLuuTrangThai = e.Message; return; }
        foreach (var e in events)
            Phat(e);
    }
}
