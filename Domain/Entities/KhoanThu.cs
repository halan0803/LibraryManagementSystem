namespace LibraryManagementSystem.Domain.Entities;

public class KhoanThu
{
    [JsonInclude] public string MaKhoan { get; private set; } = null!;
    [JsonInclude] public string MaDocGia { get; private set; } = null!;
    [JsonInclude] public string MaDongMuon { get; private set; } = null!;
    [JsonInclude] public decimal SoTienChot { get; private set; }
    [JsonInclude] public decimal SoTienDaThu { get; private set; }
    [JsonInclude] public TrangThaiKhoanThu TrangThai { get; private set; }
    [JsonInclude] public LoaiKhoanThu Loai { get; private set; }
    [JsonInclude] public bool ChoXacMinhTienNhan { get; private set; }

    [JsonConstructor]
    internal KhoanThu() { }

    public KhoanThu(string ma, string docGia, string dong, decimal soTien,
        TrangThaiKhoanThu trangThai = TrangThaiKhoanThu.ChuaThu, LoaiKhoanThu loai = LoaiKhoanThu.PhatQuaHan)
    {
        MaKhoan = Chuoi(ma, nameof(ma));
        MaDocGia = Chuoi(docGia, nameof(docGia));
        MaDongMuon = Chuoi(dong, nameof(dong));
        YeuCau(soTien >= 0, "TIEN", "Tiền không được âm.");
        SoTienChot = soTien;
        TrangThai = trangThai;
        Loai = loai;
    }

    public decimal SoTienConNo() => TrangThai is TrangThaiKhoanThu.DaHuy or TrangThaiKhoanThu.DaTatToanBangSach
        ? 0 : Math.Max(0, SoTienChot - SoTienDaThu);

    public void XacNhanThuDu(decimal tien)
    {
        YeuCau(TrangThai == TrangThaiKhoanThu.ChuaThu && tien > 0 && tien == SoTienConNo(),
            "TIEN", "Số tiền thực nhận phải bằng số còn thiếu của khoản có thể thu.");
        SoTienDaThu += tien;
        TrangThai = TrangThaiKhoanThu.DaThuDu;
    }

    internal void ChotLai(decimal tien)
    {
        YeuCau(Loai == LoaiKhoanThu.PhatQuaHan && tien >= 0, "TIEN", "Chỉ được chốt lại khoản phạt.");
        SoTienChot = tien;
        TrangThai = ChoXacMinhTienNhan || tien < SoTienDaThu ? TrangThaiKhoanThu.ChoDoiSoat :
            tien == SoTienDaThu ? TrangThaiKhoanThu.DaThuDu : TrangThaiKhoanThu.ChuaThu;
    }

    internal void HuyDoTimLai()
    {
        YeuCau(Loai == LoaiKhoanThu.BoiThuong, "KHOAN_THU", "Khoản này không phải bồi thường.");
        var canDoiSoat = ChoXacMinhTienNhan || SoTienDaThu > 0 || TrangThai == TrangThaiKhoanThu.DaTatToanBangSach;
        ChoXacMinhTienNhan = false;
        TrangThai = canDoiSoat
            ? TrangThaiKhoanThu.ChoDoiSoat : TrangThaiKhoanThu.DaHuy;
    }

    internal void ChoDoiSoatTienNhan()
    {
        YeuCau(TrangThai == TrangThaiKhoanThu.ChuaThu && SoTienConNo() > 0,
            "DOI_SOAT", "Chỉ đánh dấu khoản chưa thu còn dư nợ.");
        ChoXacMinhTienNhan = true;
        TrangThai = TrangThaiKhoanThu.ChoDoiSoat;
    }

    internal void HoanTatDoiSoat()
    {
        YeuCau(TrangThai == TrangThaiKhoanThu.ChoDoiSoat, "DOI_SOAT", "Khoản không chờ đối soát.");
        if (ChoXacMinhTienNhan)
        {
            ChoXacMinhTienNhan = false;
            TrangThai = SoTienConNo() > 0 ? TrangThaiKhoanThu.ChuaThu : TrangThaiKhoanThu.DaThuDu;
            return;
        }
        TrangThai = Loai == LoaiKhoanThu.BoiThuong ? TrangThaiKhoanThu.DaHuy : TrangThaiKhoanThu.DaThuDu;
    }

    internal void TatToanBangSach()
    {
        YeuCau(Loai == LoaiKhoanThu.BoiThuong && TrangThai == TrangThaiKhoanThu.ChuaThu && SoTienDaThu == 0,
            "BOI_THUONG_TRUNG", "Khoản bồi thường đã được xử lý.");
        TrangThai = TrangThaiKhoanThu.DaTatToanBangSach;
    }
}