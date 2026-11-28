namespace LibraryManagementSystem.Domain.Entities;

public class NhaXuatBan
{
    [JsonInclude] public string MaNXB { get; private set; } = null!;
    [JsonInclude] public string TenNXB { get; private set; } = null!;
    [JsonConstructor]
    internal NhaXuatBan()
    {
    }
    public NhaXuatBan(string ma, string ten)
    {
        MaNXB = Chuoi(ma, nameof(ma));
        TenNXB = Chuoi(ten, nameof(ten));
    }
    public void CapNhatTen(string ten) => TenNXB = Chuoi(ten, nameof(ten));
}