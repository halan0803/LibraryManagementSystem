using System.Text;
using LibraryManagementSystem.Tests;

Console.OutputEncoding = Encoding.UTF8;

int soCaLoi = KiemThuNenTang.RunAll();
soCaLoi += KiemThuTheThuVien.RunAll();
soCaLoi += KiemThuTheService.RunAll();
soCaLoi += KiemThuMuonGiaHan.RunAll();

// 0: kiểm thử đạt; 1: có ca thất bại.
Environment.ExitCode = soCaLoi == 0 ? 0 : 1;
