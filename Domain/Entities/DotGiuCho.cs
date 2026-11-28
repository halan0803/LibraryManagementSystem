namespace LibraryManagementSystem.Domain.Entities;

public class DotGiuCho
{
    [JsonInclude] public string MaDot { get; private set; } = null!;
    [JsonInclude] public string MaCuon { get; private set; } = null!;
    [JsonInclude] public string MaDongNguon { get; private set; } = null!;
    [JsonInclude]
    public TrangThaiDotGiuCho TrangThai
    {
        get; private set;
    }
    [JsonInclude] public string MaSuKien { get; private set; } = null!;
    [JsonInclude] public IReadOnlyList<LuotGiuCho> DanhSachLuot { get; private set; } = Array.Empty<LuotGiuCho>();
    [JsonConstructor]
    internal DotGiuCho()
    {
    }

    public DotGiuCho(string ma, string cuon, string dongNguon, string suKien)
    {
        MaDot = Chuoi(ma, nameof(ma));
        MaCuon = Chuoi(cuon, nameof(cuon));
        MaDongNguon = Chuoi(dongNguon, nameof(dongNguon));
        MaSuKien = Chuoi(suKien, nameof(suKien));
        DanhSachLuot = Array.Empty<LuotGiuCho>();
    }
    public void GhiNhanUngVien(DangKyCho dk)
    {
        YeuCau(TrangThai == TrangThaiDotGiuCho.DangXuLy && DanhSachLuot.Count < 2 && DanhSachLuot.All(x => x.TrangThai == TrangThaiLuotGiuCho.ChuaKichHoat) && dk.MaCuon == MaCuon && dk.ConHieuLuc() && DanhSachLuot.All(x => x.MaDangKy != dk.MaDangKy), "UNG_VIEN", "Không thể bổ sung ứng viên vào đợt.");
        DanhSachLuot = Array.AsReadOnly(DanhSachLuot.Append(new LuotGiuCho(MaMoi(), dk.MaDangKy, MaDot, DanhSachLuot.Count + 1)).ToArray());
    }
    public void KetThuc()
    {
        foreach (var x in DanhSachLuot)
            x.Huy();
        TrangThai = TrangThaiDotGiuCho.KetThuc;
    }
    internal void TamDung()
    {
        YeuCau(TrangThai == TrangThaiDotGiuCho.DangXuLy, "DOT_CU", "Đợt không đang xử lý.");
        foreach (var x in DanhSachLuot)
            x.TamDung();
        TrangThai = TrangThaiDotGiuCho.TamDung;
    }

}
