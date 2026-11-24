using LibraryManagementSystem.Presentation;
using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Tests;

namespace LibraryManagementSystem;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args is ["--kiem-thu"]) { KiemThuNghiepVu.Chay(); return; }
        ApplicationConfiguration.Initialize();
        var app = new AppBootstrapper(AppContext.BaseDirectory);
        System.Windows.Forms.Application.Run(new ThuVienForm(new PhienThuVien(app)));
    }
}
