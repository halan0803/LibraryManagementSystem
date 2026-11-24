namespace LibraryManagementSystem.Application.Services;

internal sealed partial class NghiepVu
{
    private bool CoNo(string ma) => TatCa<KhoanThu>().Any(k => k.MaDocGia == ma &&
        k.TrangThai is TrangThaiKhoanThu.ChuaThu or TrangThaiKhoanThu.ChoDoiSoat);

    private TheThuVien? TheHienHanh(string ma) => TatCa<TheThuVien>().SingleOrDefault(
        t => t.MaDocGia == ma && t.TrangThai == TrangThaiThe.HienHanh);

    internal string CapThe(string maTruong)
    {
        NhanVien();
        var thongTin = BatBuoc(truong.TinhTrang(maTruong), maTruong);
        YeuCau(thongTin.ConThuocTruong, "TRUONG", "Độc giả đã rời trường.");
        var dg = TatCa<DocGia>().SingleOrDefault(d => d.MaTruong == maTruong);
        if (dg == null)
        {
            var creator = Domain.Factories.DocGiaCreator.Chon(thongTin.LoaiDocGia);
            dg = creator.Tao(MaMoi(), maTruong, thongTin.HoTen);
            Them<DocGia>(dg);
        }
        YeuCau(dg.DuocSuDung() && TheHienHanh(dg.MaDocGia) == null, "THE", "Hồ sơ đã có thẻ hoặc bị dừng quyền.");
        Them(new TheThuVien { MaThe = MaMoi(), MaDocGia = dg.MaDocGia, NgayCap = Ngay,
            NgayHieuLuc = Ngay, NgayHetHan = Ngay.AddMonths(6) });
        return dg.MaDocGia;
    }

    internal void DoiThe(string ma, string thaoTac)
    {
        NhanVien();
        var dg = Lay<DocGia>(ma);
        var the = BatBuoc(TheHienHanh(ma), "thẻ");
        YeuCau(dg.DuocSuDung() && truong.DuDoiTuong(dg.MaTruong, dg.LoaiDocGia), "TRUONG", "Không đủ tư cách.");
        if (thaoTac == "Cấp lại thẻ")
        {
            Sua(the with { TrangThai = TrangThaiThe.DaThayThe });
            Them(the with { MaThe = MaMoi(), MaTheTruoc = the.MaThe, NgayCap = Ngay });
            return;
        }
        bool hetHan = the.NgayHetHan < Ngay;
        YeuCau(!CoNo(ma) && (thaoTac == "Khôi phục thẻ" ? hetHan :
            !hetHan && the.NgayHetHan.DayNumber - Ngay.DayNumber <= 30), "THE", "Chưa đủ điều kiện thay đổi hạn.");
        Sua(the with { NgayHetHan = (hetHan ? Ngay : the.NgayHetHan).AddMonths(6) });
    }
}
