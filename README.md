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

| Nguyên lý | Cách áp dụng dự kiến |
| --- | --- |
| Đóng gói | Quản lý dữ liệu bên trong các lớp; kiểm tra dữ liệu qua thuộc tính và phương thức. Ví dụ: cập nhật số sách còn có thể cho mượn thông qua nghiệp vụ mượn – trả, không cho phép số lượng âm. |
| Trừu tượng | Xác định trách nhiệm và hành vi của từng lớp; dùng lớp trừu tượng hoặc interface cho phần có chung hợp đồng xử lý. Ví dụ: interface lưu trữ quy định thao tác đọc và ghi dữ liệu. |
| Kế thừa | Dự kiến xây dựng lớp cơ sở `Nguoi` chứa thông tin chung như mã, họ tên và thông tin liên hệ; các lớp `DocGia`, `NhanVien` bổ sung dữ liệu và hành vi riêng. |
| Đa hình | Xử lý các đối tượng `DocGia`, `NhanVien` qua kiểu cơ sở `Nguoi`; ghi đè phương thức lấy thông tin để thể hiện nội dung phù hợp với từng loại đối tượng. |

Các lớp nghiệp vụ dự kiến gồm `Sach`, `DocGia`, `NhanVien`, `PhieuMuon` và `ChiTietPhieuMuon`. 

Giao diện, xử lý nghiệp vụ và lưu trữ dữ liệu được tách trách nhiệm. Các quy tắc mượn – trả được đặt trong phần xử lý nghiệp vụ để có thể kiểm thử độc lập với giao diện. Kế thừa và interface chỉ được sử dụng khi phù hợp với quan hệ và trách nhiệm thực tế.

## 3. Các chức năng dự kiến

| Nhóm chức năng | Nội dung |
| --- | --- |
| Quản lý sách | Thêm, sửa, xóa hoặc ngừng sử dụng thông tin sách; quản lý mã sách, tên sách, tác giả, thể loại, tổng số lượng và số lượng còn có thể cho mượn. |
| Quản lý độc giả | Thêm, cập nhật, tra cứu và quản lý trạng thái độc giả; lưu mã độc giả, họ tên và thông tin liên hệ. |
| Tra cứu | Tìm sách theo mã, tên, tác giả hoặc thể loại; tìm độc giả theo mã hoặc tên. |
| Mượn sách | Lập phiếu mượn, chọn độc giả và sách, ghi nhận ngày mượn và hạn trả; kiểm tra điều kiện mượn và cập nhật số lượng sách. |
| Trả sách | Ghi nhận sách đã trả và ngày trả theo từng chi tiết phiếu mượn; cập nhật số lượng và trạng thái phiếu. |
| Theo dõi quá hạn | Hiển thị các sách chưa trả đã vượt quá hạn trả và thông tin độc giả liên quan. |
| Thống kê cơ bản | Thống kê số đầu sách, tổng số cuốn sách, số độc giả và số cuốn đang được mượn. |
| Lưu trữ dữ liệu | Lưu và đọc lại dữ liệu để tiếp tục sử dụng sau khi đóng ứng dụng. |

**Một số ràng buộc nghiệp vụ dự kiến:**

- Mã sách, mã độc giả và mã phiếu mượn không được trùng trong từng nhóm.
- Chỉ cho mượn khi độc giả đang hoạt động và số sách còn đủ.
- Số lượng mượn phải là số nguyên dương; hạn trả không được trước ngày mượn.
- Không ghi nhận trả vượt số lượng đang mượn hoặc trả lặp lại cùng một khoản mượn.
- Không xóa thông tin làm mất lịch sử mượn – trả; ưu tiên chuyển trạng thái ngừng sử dụng.
- Giới hạn số sách được mượn và thời hạn mượn sẽ được thống nhất trong tuần đầu.

Các chức năng mở rộng như đăng nhập, phân quyền, đặt trước sách, tính tiền phạt hoặc xuất báo cáo chỉ được xem xét sau khi hoàn thành phần cốt lõi.

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