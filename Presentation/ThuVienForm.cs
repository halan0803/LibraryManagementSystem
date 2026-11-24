using System.Text.Json;
using LibraryManagementSystem.Application.Services;

namespace LibraryManagementSystem.Presentation;

internal sealed class ThuVienForm : Form
{
    private readonly PhienThuVien phien;
    private readonly TextBox ten = new() { Width = 230 };
    private readonly TextBox matKhau = new() { Width = 230, UseSystemPasswordChar = true };
    private readonly ComboBox chon = new() { Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly FlowLayoutPanel nhap = Cot();
    private readonly TextBox ketQua = new() { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true,
        ScrollBars = ScrollBars.Both, WordWrap = false, Font = new Font("Consolas", 11) };
    private readonly Label trangThai = new() { Dock = DockStyle.Bottom, Height = 28 };
    private readonly FlowLayoutPanel benTrai = Cot();
    private readonly List<TextBox> oNhap = [];

    internal ThuVienForm(PhienThuVien phien)
    {
        this.phien = phien;
        Text = "Quản lý thư viện";
        Size = new Size(1050, 760);
        MinimumSize = new Size(800, 550);
        Font = new Font("Segoe UI", 10);
        StartPosition = FormStartPosition.CenterScreen;
        benTrai.Width = 365;
        benTrai.Dock = DockStyle.Left;
        benTrai.AutoScroll = true;
        benTrai.Padding = new Padding(14);
        Controls.Add(ketQua);
        Controls.Add(benTrai);
        Controls.Add(trangThai);
        phien.SuKien.DaLuu += ten => trangThai.Text = $"  {ten} · Đã lưu";
        chon.SelectedIndexChanged += (_, _) => DienMau();
        DangNhap();
    }

    private static FlowLayoutPanel Cot() => new() { FlowDirection = FlowDirection.TopDown, WrapContents = false, AutoSize = true };
    private static Label Nhan(string s) => new() { Text = s, AutoSize = true, Margin = new Padding(3, 8, 3, 2) };
    private Button Nut(string text, Action action)
    {
        var nut = new Button { Text = text, AutoSize = true };
        nut.Click += (_, _) => {
            try { action(); }
            catch (Exception ex) { trangThai.Text = ex.Message; MessageBox.Show(this, ex.Message, "Thư viện"); }
        };
        return nut;
    }

    private void DangNhap()
    {
        benTrai.Controls.Clear();
        matKhau.Clear();
        ketQua.Clear();
        var vao = Nut("Đăng nhập", () => { phien.Chay(ten.Text, matKhau.Text); MoNghiepVu(); });
        benTrai.Controls.AddRange([Nhan("Tên đăng nhập"), ten, Nhan("Mật khẩu"), matKhau, vao]);
        AcceptButton = vao;
    }

    private void MoNghiepVu()
    {
        benTrai.Controls.Clear();
        chon.Items.Clear();
        chon.Items.AddRange(NghiepVu.BieuMau.Keys.Cast<object>().ToArray());
        var chay = Nut("Thực hiện", () => {
            var kq = phien.Chay(ten.Text, matKhau.Text, (string)chon.SelectedItem!, oNhap.Select(o => o.Text.Trim()).ToArray());
            ketQua.Text = kq is string s ? s : JsonSerializer.Serialize(kq, new JsonSerializerOptions {
                WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() } });
        });
        benTrai.Controls.AddRange([Nhan("Nghiệp vụ"), chon, nhap, chay, Nut("Đăng xuất", DangNhap)]);
        chon.SelectedIndex = 0;
        AcceptButton = chay;
    }

    private void DienMau()
    {
        nhap.Controls.Clear();
        oNhap.Clear();
        if (chon.SelectedItem is not string tenMau) return;
        foreach (string nhan in NghiepVu.BieuMau[tenMau])
        {
            var o = new TextBox { Width = 310 };
            nhap.Controls.Add(Nhan(nhan));
            nhap.Controls.Add(o);
            oNhap.Add(o);
        }
    }
}
