using LibraryManagementSystem.Application.Observers;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Services;
using LibraryManagementSystem.Infrastructure.Configuration;
using LibraryManagementSystem.Infrastructure.Logging;
using LibraryManagementSystem.Infrastructure.Notifications;
using LibraryManagementSystem.Infrastructure.Persistence;

namespace LibraryManagementSystem;

public sealed class NghiepVuMuon : IDisposable
{
    private readonly JsonDataStore khoDuLieu;

    public ITransactionManager GiaoDich { get; }
    public ILichMoCua Lich { get; }
    public TaiKhoanService TaiKhoan { get; }
    public TheService The { get; }
    public HangChoService HangCho { get; }
    public KhoSachService KhoSach { get; }
    public MuonTraService MuonTra { get; }
    public GiaHanService GiaHan { get; }

    internal NghiepVuMuon(string duongDan, CauHinhThuVien cauHinh, IDongHo dongHo,
        IDonViTruongGateway nhaTruong)
    {
        khoDuLieu = new JsonDataStore(duongDan);
        GiaoDich = new JsonTransactionManager(khoDuLieu);
        Lich = new LichMoCua(cauHinh.NgayNghiDacBiet, cauHinh.GioDongCua);
        TaiKhoan = new TaiKhoanService(GiaoDich);
        var dieuPhoi = new BoDieuPhoi(GiaoDich, dongHo, TaiKhoan);
        The = new TheService(dieuPhoi, nhaTruong);
        HangCho = new HangChoService(dieuPhoi, Lich);
        KhoSach = new KhoSachService(dieuPhoi, HangCho);
        MuonTra = new MuonTraService(dieuPhoi, Lich, KhoSach, HangCho);
        GiaHan = new GiaHanService(dieuPhoi, Lich);

        var suKien = new ThuVienEventPublisher(GiaoDich);
        suKien.DangKy(new NhatKyObserver(new NhatKyService(GiaoDich)));
        suKien.DangKy(new ThongBaoObserver(dieuPhoi, new ThongBaoTrongHeThong(GiaoDich), Lich));
        dieuPhoi.SauCommit = suKien.XuLyTonDong;
    }

    public void Dispose() => khoDuLieu.Dispose();
}
