using System.Text.Json;
using LibraryManagementSystem.Domain.Exceptions;

namespace LibraryManagementSystem.Infrastructure.Configuration;

public static class ConfigurationLoader
{
    public static CauHinhThuVien Tai(string duongDan)
    {
        if (string.IsNullOrWhiteSpace(duongDan))
        {
            throw new NghiepVuException(
                "CAU_HINH",
                "Chưa xác định đường dẫn file cấu hình.");
        }

        try
        {
            string json = File.ReadAllText(duongDan);

            var tuyChon = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CauHinhThuVien? cauHinh =
                JsonSerializer.Deserialize<CauHinhThuVien>(
                    json, tuyChon);

            if (cauHinh is null)
            {
                throw new NghiepVuException(
                    "CAU_HINH",
                    "File cấu hình không chứa đối tượng cấu hình.");
            }

            cauHinh.KiemTra();

            return cauHinh;
        }
        catch (JsonException ex)
        {
            throw new NghiepVuException(
                "CAU_HINH_JSON",
                "File cấu hình sai định dạng hoặc sai kiểu dữ liệu.",
                ex);
        }
        catch (IOException ex)
        {
            throw new NghiepVuException(
                "CAU_HINH_DOC_FILE",
                "Không đọc được file cấu hình. Kiểm tra đường dẫn và file.",
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new NghiepVuException(
                "CAU_HINH_QUYEN",
                "Ứng dụng không có quyền đọc file cấu hình.",
                ex);
        }
    }
}