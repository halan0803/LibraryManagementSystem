using LibraryManagementSystem.Application.Services;
using LibraryManagementSystem.Domain.Services;
using LibraryManagementSystem.Infrastructure.Configuration;
using LibraryManagementSystem.Infrastructure.Gateways;
using LibraryManagementSystem.Infrastructure.Persistence;

namespace LibraryManagementSystem.Tests;

internal static class IntegrationTests
{
    internal static void Chay()
    {
        string tep = Path.Combine(AppContext.BaseDirectory, $"thu-vien-{Guid.NewGuid():N}.json");
        var luc = new DateTimeOffset(2026, 11, 24, 10, 0, 0, TimeSpan.FromHours(7));
        using var db = new GiaoDichJson(tep);
        try
        {
            db.BatDau();
            DuLieuDemo.Tao(db, new DateOnly(2026, 11, 24));
            db.Commit();
            object Lenh(string tk, string ten, params string[] a)
            {
                db.BatDau();
                try
                {
                    var nv = new NghiepVu(db, new LichMoCua(), luc, new DonViTruongGiaLap(DuLieuDemo.Truong));
                    nv.XacThuc(tk, "ThuVien@123");
                    var kq = nv.ThucHien(ten, a);
                    db.Commit();
                    return kq;
                }
                catch { db.Rollback(); throw; }
            }

            string DongDau(string phieu)
            {
                db.BatDau();
                var id = db.Kho<PhieuMuon>().LayTheoMa(phieu)!.DanhSachDongMuon[0].MaDong;
                db.Rollback();
                return id;
            }

            void TuChoi(Action thaoTac)
            {
                try { thaoTac(); }
                catch (NghiepVuException) { return; }
                throw new InvalidOperationException("Đã chấp nhận thao tác phải bị từ chối.");
            }

            string gv = (string)Lenh("thuthu", "Cấp thẻ", "GV01");
            TuChoi(() => Lenh("thuthu", "Cấp thẻ", "GV01"));
            Lenh("thuthu", "Cấp lại thẻ", gv);
            TuChoi(() => Lenh("thuthu", "Gia hạn thẻ", gv));
            TuChoi(() => Lenh("thuthu", "Mượn sách", "DG01", "C01 SAI", "YC1"));
            string pm = (string)Lenh("thuthu", "Mượn sách", "DG01", "C01", "YC1");
            if ((string)Lenh("thuthu", "Mượn sách", "DG01", "C01", "YC1") != pm)
                throw new InvalidOperationException("Lặp phiếu mượn.");
            string dong = DongDau(pm);
            Lenh("docgia", "Gia hạn mượn", dong, "3");
            TuChoi(() => Lenh("docgia", "Gia hạn mượn", dong, "3"));
            Lenh("thuthu", "Đăng ký chờ", gv, "C01");
            TuChoi(() => Lenh("thuthu", "Đăng ký chờ", gv, "C01"));
            Lenh("thuthu", "Nhận trả", dong, "Tot");
            TuChoi(() => Lenh("thuthu", "Mượn sách", "DG01", "C01", "YC2"));
            string tiep = (string)Lenh("thuthu", "Mượn sách", gv, "C01", "YC3");
            string dongMat = DongDau(tiep);
            Lenh("thuthu", "Báo mất", dongMat, "Đã xác minh");
            TuChoi(() => Lenh("quanly", "Đóng hồ sơ", gv, "Rời trường"));
            Lenh("thuthu", "Thu tiền", gv, "100000", "PT01");
            Lenh("quanly", "Đóng hồ sơ", gv, "Hoàn tất nghĩa vụ");
            Lenh("docgia", "Tra cứu sách", "lập trình");
            TuChoi(() => Lenh("docgia", "Thống kê"));
            Lenh("quanly", "Thống kê");
            string phieuHong = (string)Lenh("thuthu", "Mượn sách", "DG01", "C02", "YC4");
            Lenh("thuthu", "Nhận trả", DongDau(phieuHong), "HuNang");
            db.BatDau();
            string suCo = db.Kho<BienBanSuCo>().LayTatCa().Single(s => s.Loai == LoaiSuCo.Hong).MaSuCo;
            db.Rollback();
            Lenh("quanly", "Nhận bản thay thế", suCo, "C04", "DS01", "K01", "Đúng ấn bản");
            TuChoi(() => Lenh("quanly", "Nhận bản thay thế", suCo, "C05", "DS01", "K01", "Trùng"));
            Lenh("quanly", "Thanh lý", "C02", "Hư nặng");
            Lenh("thuthu", "Nhập sách", "DS02", "OOP", "Mai", "1", "Việt", "C06", "90000", "K01");
            Console.WriteLine("Đạt: luồng tích hợp UC01–UC12 qua giao dịch JSON.");
        }
        finally
        {
            db.Rollback();
            File.Delete(tep);
            File.Delete(tep + ".lock");
        }
    }
}

