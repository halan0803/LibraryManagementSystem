using LibraryManagementSystem.Tests;

if (args is ["--kiem-thu"])
{
    KiemThuNghiepVu.Chay();
    return;
}

Console.WriteLine("Hệ thống quản lý thư viện. Dùng --kiem-thu để chạy kiểm thử nghiệp vụ.");
