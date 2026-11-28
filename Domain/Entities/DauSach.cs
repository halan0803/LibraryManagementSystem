using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Domain.Entities;

public class DauSach
{
    [JsonInclude] public string MaDauSach { get; private set; } = null!;
    [JsonInclude] public string TenSach { get; private set; } = null!;
    [JsonInclude] public string PhienBan { get; private set; } = null!;
    [JsonInclude] public string NgonNgu { get; private set; } = null!;
    [JsonInclude]
    public decimal? GiaBia
    {
        get; private set;
    }
    [JsonInclude] public IReadOnlyList<string> DanhSachMaTacGia { get; private set; } = Array.Empty<string>();
    [JsonInclude] public string MaTheLoai { get; private set; } = null!;
    [JsonInclude]
    public string? MaNXB
    {
        get; private set;
    }
    [JsonConstructor]
    internal DauSach()
    {
    }

    public DauSach(string ma, string ten, string phienBan, string ngonNgu, IEnumerable<string> tacGia, string theLoai, string? nxb = null, decimal? giaBia = null)
    {
        MaDauSach = Chuoi(ma, nameof(ma));
        CapNhatThongTin(ten, ngonNgu, phienBan);
        GanTheLoai(theLoai);
        MaNXB = nxb;
        YeuCau(giaBia is null || giaBia >= 0, "GIA_AM", "Giá không được âm.");
        GiaBia = giaBia;
        var authors = tacGia.Select(x => Chuoi(x, "Tác giả")).Distinct().ToArray();
        YeuCau(authors.Length > 0, "TAC_GIA", "Cần ít nhất một tác giả.");
        DanhSachMaTacGia = Array.AsReadOnly(authors);
    }
    public void CapNhatThongTin(string ten, string ngonNgu, string phienBan)
    {
        var a = Chuoi(ten, nameof(ten));
        var b = Chuoi(ngonNgu, nameof(ngonNgu));
        var c = Chuoi(phienBan, nameof(phienBan));
        TenSach = a;
        NgonNgu = b;
        PhienBan = c;
    }
    public void GanTheLoai(string ma) => MaTheLoai = Chuoi(ma, nameof(ma));

}