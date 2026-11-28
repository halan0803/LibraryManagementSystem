using System.Text.Json;
namespace LibraryManagementSystem.Infrastructure.Persistence;

public sealed class JsonTransactionManager : ITransactionManager
{
    private readonly JsonDataStore store; private AnhChup? working; private int owner;
    private readonly Dictionary<(Type, string), object> cache = new();
    public JsonTransactionManager(JsonDataStore store)
    {
        this.store = store;
    }
    public bool DangGiaoDich => working != null && owner == Environment.CurrentManagedThreadId;
    public void BatDau()
    {
        Monitor.Enter(store.Khoa);
        if (working != null)
        {
            Monitor.Exit(store.Khoa);
            throw new InvalidOperationException("Không mở giao dịch lồng; dùng cùng phạm vi.");
        }
        try
        {
            working = store.Doc();
            owner = Environment.CurrentManagedThreadId;
            cache.Clear();
            KiemTraToanVen();
        }
        catch { working = null; owner = 0; cache.Clear(); Monitor.Exit(store.Khoa); throw; }
    }
    private void Check()
    {
        if (!DangGiaoDich)
            throw new InvalidOperationException("Cần giao dịch trên luồng hiện tại.");
    }
    internal Dictionary<string, JsonElement> Bang<T>()
    {
        Check();
        var key = typeof(T).Name;
        if (!working!.Bang.TryGetValue(key, out var table))
            working.Bang[key] = table = new();
        return table;
    }
    internal T LayCache<T>(string id, Func<T> create) where T : class
    {
        Check();
        var key = (typeof(T), id);
        if (!cache.TryGetValue(key, out var x))
            cache[key] = x = create();
        return (T)x;
    }
    internal void DatCache<T>(string id, T value) where T : class => cache[(typeof(T), id)] = value;
    public IRepository<T> Kho<T>() where T : class
    {
        Check();
        return new JsonRepository<T>(this);
    }
    public void Commit()
    {
        Check();
        try
        {
            KiemTraToanVen();
            store.Ghi(working!);
        }
        finally { working = null; cache.Clear(); owner = 0; Monitor.Exit(store.Khoa); }
    }
    public void Rollback()
    {
        if (!DangGiaoDich)
            return;
        working = null;
        cache.Clear();
        owner = 0;
        Monitor.Exit(store.Khoa);
    }
    private void KiemTraToanVen()
    {
        // Serialize all loaded entities once, preserving changes inside the shared transaction.
        foreach (var entry in cache)
        {
            var table = working!.Bang[entry.Key.Item1.Name];
            table[entry.Key.Item2] = JsonSerializer.SerializeToElement(entry.Value, entry.Key.Item1, JsonDataStore.Json);
        }
        void Unique<T>(IEnumerable<T> source, Func<T, string> key, string message)
        {
            YeuCau(!source.GroupBy(key).Any(g => g.Count() > 1), "TOAN_VEN", message);
        }
        foreach (var table in working!.Bang.Values)
        foreach (var row in table)
        {
            if (string.IsNullOrWhiteSpace(row.Key))
                throw new InvalidDataException("Mã dữ liệu trống.");
        }
        var dg = Kho<DocGia>().LayTatCa();
        var books = Kho<CuonSach>().LayTatCa();
        var accounts = Kho<TaiKhoan>().LayTatCa();
        var loans = Kho<ChiTietMuon>().LayTatCa();
        var slips = Kho<PhieuMuon>().LayTatCa();
        Unique(dg, x => x.MaTruong, "Trùng mã trường.");
        Unique(accounts, x => x.TenDangNhap.ToUpperInvariant(), "Trùng tên đăng nhập.");
        Unique(Kho<TheThuVien>().LayTatCa().Where(x => x.TrangThai == TrangThaiThe.HienHanh), x => x.MaDocGia, "Hai thẻ hiện hành.");
        Unique(Kho<DangKyCho>().LayTatCa().Where(x => x.ConHieuLuc()), x => x.MaDocGia, "Hai đăng ký hiệu lực.");
        Unique(loans.Where(x => x.NgayNhan is null && x.NgayKetThucNghiaVu is null), x => x.MaCuon, "Hai lượt cùng chiếm cuốn.");
        Unique(Kho<DotGiuCho>().LayTatCa().Where(x => x.TrangThai != TrangThaiDotGiuCho.KetThuc), x => x.MaCuon, "Hai đợt mở cùng cuốn.");
        foreach (var c in books)
            YeuCau(Kho<DauSach>().LayTheoMa(c.MaDauSach) != null && (c.MaKe is null || Kho<KeSach>().LayTheoMa(c.MaKe) != null), "LIEN_KET", "Cuốn thiếu đầu sách/kệ.");
        foreach (var t in Kho<TheThuVien>().LayTatCa())
            YeuCau(dg.Any(x => x.MaDocGia == t.MaDocGia), "LIEN_KET", "Thẻ thiếu độc giả.");
        foreach (var a in accounts)
            YeuCau(a.VaiTro == VaiTro.DocGia ? a.MaNhanVien is null && dg.Any(x => x.MaDocGia == a.MaDocGia) : a.MaDocGia is null && Kho<NhanVien>().LayTatCa().Any(x => x.MaNhanVien == a.MaNhanVien && x.VaiTro == a.VaiTro), "LIEN_KET", "Chủ tài khoản/vai trò không thống nhất.");
        foreach (var d in loans)
            YeuCau(books.Any(x => x.MaCuon == d.MaCuon) && slips.Any(x => x.MaPhieu == d.MaPhieu), "LIEN_KET", "Dòng mượn thiếu cuốn/phiếu.");
        foreach (var d in Kho<DangKyCho>().LayTatCa())
            YeuCau(dg.Any(x => x.MaDocGia == d.MaDocGia) && books.Any(x => x.MaCuon == d.MaCuon), "LIEN_KET", "Hàng chờ thiếu độc giả/cuốn.");

        foreach (var d in Kho<DauSach>().LayTatCa())
            YeuCau(d.DanhSachMaTacGia.Count > 0 && d.DanhSachMaTacGia.All(id => Kho<TacGia>().LayTheoMa(id) != null) && Kho<TheLoai>().LayTheoMa(d.MaTheLoai) != null && (d.MaNXB is null || Kho<NhaXuatBan>().LayTheoMa(d.MaNXB) != null), "LIEN_KET", "Đầu sách thiếu tác giả/thể loại/NXB.");
        foreach (var p in slips)
            YeuCau(dg.Any(x => x.MaDocGia == p.MaDocGia) && Kho<NhanVien>().LayTheoMa(p.MaNguoiLap) != null, "LIEN_KET", "Phiếu mượn thiếu độc giả/người lập.");
        foreach (var k in Kho<KhoanThu>().LayTatCa())
            YeuCau(dg.Any(x => x.MaDocGia == k.MaDocGia) && loans.Any(x => x.MaDong == k.MaDongMuon), "LIEN_KET", "Khoản thu thiếu độc giả/dòng mượn.");
        foreach (var s in Kho<BienBanSuCo>().LayTatCa())
            YeuCau(loans.Any(x => x.MaDong == s.MaDongMuon), "LIEN_KET", "Sự cố thiếu dòng mượn.");
        foreach (var item in cache)
        {
            var id = DinhDanh.Lay(item.Value);
            YeuCau(id == item.Key.Item2, "DINH_DANH", "Mã bảng khác định danh thực thể.");
        }

        foreach (var d in Kho<DotGiuCho>().LayTatCa())
        {
            YeuCau(d.DanhSachLuot.Count is >= 1 and <= 2 && d.DanhSachLuot.Count(x => x.TrangThai == TrangThaiLuotGiuCho.DangCoQuyen) <= 1, "DOT", "Đợt không hợp lệ.");
            foreach (var l in d.DanhSachLuot)
                YeuCau(l.MaDot == d.MaDot && Kho<DangKyCho>().LayTheoMa(l.MaDangKy) != null, "LIEN_KET", "Lượt thiếu đăng ký.");
        }
    }
}
