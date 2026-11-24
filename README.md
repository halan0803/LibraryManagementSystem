# LibraryManagementSystem — Hệ thống quản lý thư viện

## 1. Giới thiệu tổng quan

LibraryManagementSystem là ứng dụng hỗ trợ quản lý hoạt động của một thư viện quy mô nhỏ. Hệ thống dự kiến giúp thủ thư quản lý sách, độc giả và quá trình mượn – trả; đồng thời hỗ trợ tra cứu thông tin, theo dõi sách đang được mượn và các khoản mượn quá hạn.

Dự án được xây dựng bằng C#, tập trung vận dụng kiến thức lập trình hướng đối tượng vào việc phân tích, thiết kế và cài đặt một hệ thống có nghiệp vụ cụ thể. Phạm vi ban đầu hướng đến ứng dụng chạy trên một máy tính, sử dụng dữ liệu mẫu để kiểm thử và trình diễn.

## 2. Mục tiêu xây dựng và áp dụng OOP

### 2.1. Mục tiêu xây dựng

- Tập trung thông tin sách, độc giả và các phiếu mượn để thuận tiện quản lý.
- Hỗ trợ thực hiện nghiệp vụ mượn – trả, hạn chế sai sót khi cập nhật số lượng sách.
- Cho phép tìm kiếm nhanh và theo dõi tình trạng mượn sách.
- Xây dựng chương trình có cấu trúc rõ ràng, dễ kiểm thử, sửa đổi và mở rộng.
- Thực hành quy trình từ phân tích yêu cầu, thiết kế đến lập trình, tích hợp và kiểm thử.

### 2.2. Định hướng áp dụng OOP

| Nguyên lý | Cách áp dụng |
| --- | --- |
| Đóng gói | Các lớp bảo vệ dữ liệu và kiểm tra điều kiện trước khi thay đổi trạng thái. Ví dụ: CuonSach thay đổi trạng thái qua phương thức nghiệp vụ; ChiTietMuon chỉ cho phép gia hạn một lần khi đủ điều kiện. |
| Trừu tượng | Sử dụng lớp trừu tượng DocGia để biểu diễn thông tin và hành vi chung của độc giả; sử dụng interface để xác định hợp đồng lưu trữ, thông báo và các chính sách nghiệp vụ. |
| Kế thừa | SinhVien và GiangVien kế thừa DocGia, bổ sung đặc điểm và chính sách mượn tương ứng. NhanVien có trách nhiệm và quyền riêng, không mặc nhiên là độc giả. |
| Đa hình | Xử lý SinhVien và GiangVien thông qua kiểu DocGia; ghi đè SoNgayMuon() và GioiHanCuon() để áp dụng thời hạn và hạn mức theo từng loại độc giả. |

Mô hình thực thể được tổ chức trong Domain/Entities, gồm 26 lớp:

- Độc giả, nhân viên và tài khoản: DocGia, SinhVien, GiangVien, NhanVien, TheThuVien, TaiKhoan.
- Danh mục và sách vật lý: DauSach, CuonSach, TacGia, TheLoai, NhaXuatBan, KeSach.
- Mượn sách: PhieuMuon, ChiTietMuon.
- Hàng chờ và giữ chỗ: DangKyCho, DotGiuCho, LuotGiuCho.
- Sự cố, phê duyệt và chứng từ: BienBanSuCo, PheDuyet, KhoanThu, PhieuThu, ChiTietThu, BienNhanThayThe, BienBanThanhLy.
- Thông báo và lịch sử: ThongBao, NhatKyThaoTac.

DauSach lưu thông tin chung của một ấn bản; CuonSach quản lý từng bản sách vật lý. PhieuMuon chứa các ChiTietMuon để theo dõi riêng hạn trả, gia hạn và nhận trả của từng cuốn.

Giao diện, xử lý nghiệp vụ và lưu trữ được tách trách nhiệm. Các lớp trong Application/Services điều phối chức năng, kiểm tra quyền và gọi phương thức của thực thể. Giao diện không trực tiếp tính tiền phạt hoặc sửa trạng thái sách để bỏ qua quy tắc nghiệp vụ.

## 3. Các chức năng dự kiến

| Nhóm chức năng | Nội dung | Use Case |
| --- | --- | --- |
| Quản lý thẻ thư viện | Xác minh độc giả; cấp mới, cấp lại, gia hạn và khôi phục thẻ; bảo toàn hồ sơ và lịch sử. | UC01, UC02 |
| Mượn và gia hạn sách | Kiểm tra điều kiện mượn, lập phiếu cho các cuốn hợp lệ; gia hạn riêng từng dòng mượn theo quy định. | UC03, UC04 |
| Trả sách và thu tiền | Nhận từng cuốn, đối chiếu tình trạng, tính phạt và ghi nhận tiền thực thu; theo dõi riêng việc nhận trả và thanh toán. | UC05 |
| Hàng chờ và giữ chỗ | Đăng ký hoặc hủy chờ theo cuốn; xét tối đa hai người trong một đợt, cấp quyền nhận tuần tự và xử lý hết lượt. | UC06, UC07 |
| Xử lý mất, hỏng và bồi thường | Lập biên bản, xác định mức bồi thường, xử lý sách tìm lại; phê duyệt, nhận bản thay thế và thanh lý cuốn hư nặng. | UC08, UC09 |
| Xác nhận nghĩa vụ và đóng hồ sơ | Đối soát sách, công nợ; xác nhận hoàn tất nghĩa vụ, dừng quyền hoặc đóng hồ sơ đúng điều kiện. | UC10 |
| Quản lý danh mục và vị trí | Quản lý đầu sách, từng cuốn, tác giả, thể loại, nhà xuất bản và kệ sách; bảo toàn lịch sử lưu thông. | UC11 |
| Tra cứu và thống kê | Tra cứu sách, khả dụng, lịch sử và nghĩa vụ theo quyền; thống kê sách đang mượn, quá hạn, khoản chưa thu và tiền thực thu. | UC12 |

Các chức năng dùng chung gồm đăng nhập, kiểm tra quyền, thông báo trong hệ thống, ghi nhật ký và lưu/đọc dữ liệu JSON.

**Các ràng buộc nghiệp vụ chính:**

- Sinh viên được mượn tối đa 3 cuốn, thời hạn 14 ngày mở cửa; giảng viên tối đa 5 cuốn, thời hạn 30 ngày mở cửa.
- Hạn mức được tính trên tất cả phiếu mượn của độc giả.
- Thẻ có hiệu lực 6 tháng lịch; chỉ gia hạn thẻ còn hạn trong 30 ngày cuối.
- Mỗi dòng mượn chỉ được gia hạn một lần, chọn thêm 3, 5 hoặc 7 ngày mở cửa từ hạn trả cũ khi đủ điều kiện.
- Tiền phạt quá hạn là 5.000 đồng/cuốn/ngày mở cửa; không tính ngày thư viện đóng cửa.
- Mỗi độc giả chỉ có một đăng ký chờ hiệu lực trên toàn thư viện.
- Nhận trả sách và thanh toán được theo dõi riêng. Không tiếp tục tăng phạt quá hạn sau ngày thư viện thực nhận sách.
- Báo mất không tự dừng tiền phạt hoặc giải phóng hạn mức; việc tất toán thực hiện theo Q02.
- Không xóa hồ sơ hoặc chứng từ để làm mất lịch sử và nghĩa vụ đã phát sinh.

Quy tắc, điều kiện và các trường hợp ngoại lệ được trình bày đầy đủ trong:

- [Phân tích yêu cầu](Documents/Analysis/01-phan-tich-yeu-cau.md).
- [Đặc tả Use Case](Documents/Analysis/02-use-case.md).

## 4. Ngôn ngữ, framework và môi trường phát triển

| Thành phần | Lựa chọn dự kiến |
| --- | --- |
| Ngôn ngữ lập trình | C#. |
| Nền tảng | .NET 10. |
| Framework giao diện | Windows Forms (WinForms), xây dựng ứng dụng desktop trên Windows. |
| Môi trường phát triển | Visual Studio Code (VS Code). |
| Công cụ hỗ trợ C# | Tiện ích C# Dev Kit và .NET SDK; sử dụng .NET CLI trong terminal của VS Code để biên dịch và chạy chương trình. |
| Lưu trữ dữ liệu | Tệp JSON cho phiên bản ban đầu; tách phần lưu trữ để có thể thay đổi khi cần. |
| Quản lý mã nguồn | Git và GitHub để lưu mã nguồn, theo dõi thay đổi và phối hợp giữa các thành viên. |
| Tài liệu | Markdown cho README; công cụ vẽ UML để mô tả các lớp và quan hệ. |
| Kiểm thử | Kiểm thử phương thức nghiệp vụ bằng chương trình chạy thử; kiểm thử các luồng chức năng trên giao diện. |

Mã nguồn được phát triển trong VS Code trên máy tính, biên dịch và chạy bằng .NET CLI, sau đó đồng bộ lên GitHub. Cần cài đặt .NET SDK và tiện ích C# Dev Kit. 

Tài liệu tham khảo: [Windows Forms — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/).

Hướng dẫn môi trường: [C# trong VS Code](https://code.visualstudio.com/docs/csharp/get-started).

## 5. Các giai đoạn thực hiện 

| Thời gian | Giai đoạn | Công việc chính | Kết quả cần đạt |
| --- | --- | --- | --- |
| Tuần 1 — Ngày 1–3 | Phân tích yêu cầu | Mô tả hoạt động thư viện; xác định người sử dụng, phạm vi, chức năng và quy tắc mượn – trả; lựa chọn công nghệ. | Tài liệu yêu cầu, danh sách chức năng và ràng buộc nghiệp vụ được thống nhất. |
| Tuần 1 — Ngày 4–7 | Phân tích và thiết kế | Xác định các lớp, trách nhiệm, thuộc tính, phương thức, bất biến và quan hệ; vẽ sơ đồ lớp; phác thảo giao diện, thiết kế lưu trữ và chia nhiệm vụ theo lớp. | Thiết kế ban đầu, chữ ký phương thức dùng chung, bảng phân công và kho mã nguồn. |
| Tuần 2 — Ngày 8–11 | Lập trình các lớp | Cài đặt lớp theo thiết kế; xử lý quản lý sách, độc giả và mượn – trả; xây dựng phần đọc, ghi dữ liệu; kiểm thử từng phần ngay khi hoàn thành. | Các lớp nghiệp vụ và phần lưu trữ chạy được với dữ liệu mẫu. |
| Tuần 2 — Ngày 12–14 | Xây dựng giao diện và tích hợp | Hoàn thiện các màn hình chính; kết nối giao diện với nghiệp vụ; tích hợp mã nguồn của các thành viên và kiểm tra luồng mượn – trả. | Phiên bản đầu tiên thực hiện được các chức năng cốt lõi. |
| Tuần 3 — Ngày 15–18 | Kiểm thử và hoàn thiện | Kiểm tra luồng bình thường, dữ liệu sai và trường hợp biên; kiểm tra lưu, tải lại dữ liệu; sửa lỗi và rà soát cách áp dụng OOP. | Bảng kết quả kiểm thử, lỗi đã xử lý và phiên bản chạy ổn định trong phạm vi đã chọn. |
| Tuần 3 — Ngày 19–21 | Hoàn thiện tài liệu và bàn giao | Cập nhật README và sơ đồ theo mã nguồn thực tế; hoàn thiện báo cáo; chuẩn bị dữ liệu và kịch bản trình diễn; kiểm tra chạy trên máy khác. | Mã nguồn hoàn chỉnh, hướng dẫn chạy, báo cáo, kết quả kiểm thử và bản trình diễn. |

### Nguyên tắc phối hợp

- Các thành viên cùng phân tích và thống nhất thiết kế trước khi chia lớp để lập trình.
- Mỗi thành viên phụ trách các lớp cụ thể, cài đặt và kiểm thử phần được giao.
- Thống nhất tên lớp, chữ ký phương thức, kiểu dữ liệu và cách xử lý lỗi để các phần có thể tích hợp.
- Khi thay đổi thiết kế ảnh hưởng đến lớp khác, trao đổi và cập nhật tài liệu trước khi tích hợp.
- Các thành viên cùng tích hợp chương trình, kiểm thử toàn hệ thống và rà soát báo cáo.
- Cập nhật README theo tiến độ thực tế; chỉ đánh dấu hoàn thành sau khi chức năng đã được kiểm tra.

## 6. Quy ước viết code

1. **Đặt tên nhất quán.** Dùng tiếng Việt không dấu cho tên nghiệp vụ. Viết tên lớp, thuộc tính và phương thức theo PascalCase (`CuonSach`, `MaCuon`, `DanhDauMuon`), tham số và biến cục bộ theo camelCase (`maCuon`). Tên interface bắt đầu bằng `I`, ví dụ `ILichMoCua`.
2. **Thống nhất thay đổi hợp đồng dùng chung.** Khi đổi constructor, chữ ký phương thức, giá trị enum hoặc mã lỗi mà phần khác sử dụng, mô tả thay đổi trong pull request để người phụ trách phần liên quan rà soát. Cập nhật [hợp đồng giữa các thành phần](Documents/Design/04-kien-truc-va-pattern.md) và các chỗ gọi bị ảnh hưởng.
3. **Đặt nghiệp vụ đúng tầng.** WinForms nhận dữ liệu và hiển thị kết quả. Service kiểm tra quyền và điều phối thao tác. Thực thể và chính sách trong Domain thực thi quy tắc nghiệp vụ. Ví dụ, mọi luồng tính hạn trả đều dùng `ILichMoCua` để xử lý ngày nghỉ nhất quán.
4. **Mỗi giao dịch có một nơi quyết định lưu.** Khi mượn nhiều cuốn, `MuonTraService` chịu trách nhiệm `Commit` hoặc `Rollback` toàn bộ yêu cầu. Phần kho cập nhật các cuốn trong cùng giao dịch. Chỉ phát thông báo mượn thành công sau khi commit thành công.
5. **Kiểm thử trước khi gộp.** Mỗi thay đổi nghiệp vụ phải build thành công và có kiểm thử cho cả trường hợp thành công lẫn trường hợp bị từ chối. Ví dụ, nếu một trong hai cuốn được yêu cầu không đủ điều kiện mượn, kiểm thử phải xác nhận hệ thống từ chối cả yêu cầu, không tạo phiếu và cuốn còn lại vẫn sẵn sàng.
