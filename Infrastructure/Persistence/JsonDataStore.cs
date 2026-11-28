using System.Text.Json;
namespace LibraryManagementSystem.Infrastructure.Persistence;

public sealed class JsonDataStore : IDisposable
{
    internal readonly object Khoa = new(); internal string DuongDan
    {
        get;
    }
    private readonly FileStream khoaTienTrinh;
    public string? CanhBaoKhoiPhuc
    {
        get; private set;
    }
    internal static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    public JsonDataStore(string path)
    {
        DuongDan = Path.GetFullPath(path);
        Directory.CreateDirectory(Path.GetDirectoryName(DuongDan)!);
        try
        {
            khoaTienTrinh = new FileStream(DuongDan + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException e) { throw new IOException("Dữ liệu đang được một tiến trình khác sử dụng.", e); }
        try
        {
            Doc();
        }
        catch { khoaTienTrinh.Dispose(); throw; }
    }
    internal AnhChup Doc()
    {
        if (!File.Exists(DuongDan))
        {
            if (File.Exists(DuongDan + ".bak"))
                throw new InvalidDataException("Thiếu tệp chính nhưng còn bản dự phòng; cần phục hồi có đối soát.");
            return new();
        }
        // Không tự coi tệp hỏng là kho rỗng hoặc âm thầm quay về bản cũ.
        try
        {
            var d = JsonSerializer.Deserialize<AnhChup>(File.ReadAllText(DuongDan), Json) ?? throw new InvalidDataException("Kho rỗng.");
            if (d.PhienBanCauTruc != 1)
                throw new InvalidDataException("Phiên bản cấu trúc không được hỗ trợ.");
            return d;
        }
        catch (JsonException e) { throw new InvalidDataException("Tệp dữ liệu hỏng. Giữ nguyên tệp chính và .bak để đối soát, không ghi đè.", e); }
    }
    internal void Ghi(AnhChup d)
    {
        d.PhienBanDuLieu++;
        var tmp = DuongDan + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(d, Json);
            using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                fs.Write(bytes);
                fs.Flush(true);
            }
            _ = JsonSerializer.Deserialize<AnhChup>(File.ReadAllBytes(tmp), Json) ?? throw new InvalidDataException("Ảnh chụp không hợp lệ.");
            if (File.Exists(DuongDan))
                File.Replace(tmp, DuongDan, DuongDan + ".bak");
            else
                File.Move(tmp, DuongDan);
        }
        finally { if (File.Exists(tmp)) File.Delete(tmp); }
    }
    public void Dispose() => khoaTienTrinh.Dispose();
}
internal sealed class AnhChup
{
    [JsonRequired]
    public int PhienBanCauTruc { get; set; } = 1;

    public long PhienBanDuLieu
    {
        get; set;
    }
    [JsonRequired]
    public Dictionary<string, Dictionary<string, JsonElement>> Bang { get; set; } = new();
}
