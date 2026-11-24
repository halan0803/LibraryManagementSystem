# Thiết kế lớp hệ thống quản lý thư viện

Tài liệu xác định lớp, thuộc tính, phương thức, quan hệ và vị trí thực hiện quy tắc nghiệp vụ của LibraryManagementSystem.

- [Phân tích yêu cầu](../Analysis/01-phan-tich-yeu-cau.md): nguồn quy tắc BR01–BR36, Q01–Q04 và phạm vi hai phân hệ.
- [Đặc tả Use Case](../Analysis/02-use-case.md): tác nhân, đầu vào, luồng xử lý và kết quả của UC01–UC12.
- [Kiến trúc và pattern](04-kien-truc-va-pattern.md): tổ chức các tầng, phân quyền, lưu trữ và mẫu thiết kế.
- Sơ đồ lớp: `UML/sơ đồ lớp.drawio`.

## 1. Quy ước thiết kế

Tài liệu dùng ký hiệu UML `TenHam(thamSo: Kieu): KieuTraVe`; đây là chữ ký thiết kế, khi viết C# đổi sang cú pháp khai báo tương ứng. Tên hàm chỉ có `()` trong bảng ánh xạ là tên rút gọn, không có nghĩa hàm không cần dữ liệu đầu vào. Hợp đồng dữ liệu của service nằm ở mục 5.3.

- `DocGia` là lớp trừu tượng; `SinhVien` và `GiangVien` kế thừa và ghi đè chính sách mượn. `NhanVien` không kế thừa `DocGia`.
- Định danh dùng `string`, không rỗng, không thay đổi sau khi tạo; kiểm tra trùng giữa nhiều đối tượng tại service và nơi lưu trữ.
- Dùng `DateOnly` cho ngày nghiệp vụ, `DateTimeOffset` cho thời điểm, `TimeOnly` cho giờ đóng cửa. Thời điểm xử lý được quy về giờ Việt Nam.
- Tiền dùng `decimal`, đơn vị đồng. Giá chưa biết dùng `decimal?`; không thay giá chưa biết bằng 0. Số ngày và số lượng dùng số nguyên.
- Dấu `?` biểu thị có thể chưa có giá trị. Các enum trong `Domain/Enums` biểu diễn những trạng thái riêng biệt, không dùng một trạng thái chung cho nhận trả, công nợ và khả dụng.
- Thuộc tính định danh chỉ đọc; trạng thái và tiền có bộ gán không công khai. Danh sách lưu nội bộ bằng `List<T>`, cung cấp ra ngoài dưới dạng `IReadOnlyList<T>`; thay đổi qua phương thức có kiểm tra.
- Constructor kiểm tra dữ liệu tối thiểu của đối tượng. Phương thức thực thể bảo vệ bất biến nội bộ; service kiểm tra quyền và quy tắc cần dữ liệu nhiều đối tượng. Không đặt repository hoặc lớp giao diện vào thực thể.
- Các bảng mô tả thiết kế trước cài đặt; không dùng số file trong khung làm bằng chứng rằng hành vi nghiệp vụ đã chạy đúng.

### 1.1. Quyền sở hữu và nguồn dữ liệu

Mã liên kết xác định quan hệ giữa các thực thể. Khi đọc JSON, service/repository ghép các danh sách từ mã liên kết; không tạo hai bản ghi có thể cập nhật độc lập cho cùng một thực thể. `PhieuMuon` quản lý các dòng mượn, `PhieuThu` quản lý các dòng thu và `DotGiuCho` quản lý các lượt. `KhoanThu.DanhSachChiTietThu` là danh sách tra cứu từ các dòng thu đã xác nhận, không phải bản sao chứng từ có quyền sửa riêng.

`MaSinhVien` và `MaGiangVien` là cách truy cập mã trường theo loại độc giả, lấy giá trị từ `MaTruong`; không lưu thêm một mã khác có thể lệch nhau. `TaiKhoan.VaiTro` của nhân viên phải đồng bộ với `NhanVien.VaiTro` trong cùng lần cập nhật; vai trò của phiên đăng nhập phải được kiểm tra lại trước thao tác có thẩm quyền.

## 2. Danh sách lớp thực thể

| STT | Lớp | Trách nhiệm |
| --- | --- | --- |
| 1 | `DocGia` | Lớp trừu tượng chứa định danh, tư cách và quyền sử dụng; cung cấp chính sách mượn qua phương thức đa hình. |
| 2 | `SinhVien` | Độc giả sinh viên: tối đa 3 cuốn, 14 ngày mở cửa. |
| 3 | `GiangVien` | Độc giả giảng viên: tối đa 5 cuốn, 30 ngày mở cửa. |
| 4 | `NhanVien` | Nhân viên thủ thư hoặc quản lý, có quyền thực hiện nghiệp vụ theo vai trò. |
| 5 | `TheThuVien` | Hiệu lực và lịch sử cấp/cấp lại thẻ gắn với một hồ sơ độc giả. |
| 6 | `TaiKhoan` | Danh tính đăng nhập, trạng thái tài khoản và vai trò truy cập. |
| 7 | `DauSach` | Thông tin chung của một ấn bản; liên kết tác giả, thể loại và nhà xuất bản. |
| 8 | `CuonSach` | Một bản vật lý, có mã, tình trạng, vị trí và quyền sở hữu riêng. |
| 9 | `TacGia` | Thông tin tác giả được nhiều đầu sách tham chiếu. |
| 10 | `TheLoai` | Phân loại đầu sách; mô hình dùng một thể loại chính cho một đầu sách. |
| 11 | `NhaXuatBan` | Thông tin nhà xuất bản của ấn bản. |
| 12 | `KeSach` | Kệ vật lý và vị trí của kệ trong thư viện. |
| 13 | `PhieuMuon` | Giao dịch mượn của độc giả, chứa một hoặc nhiều dòng mượn. |
| 14 | `ChiTietMuon` | Theo dõi một cuốn trong một lượt mượn, độc lập hạn trả, nhận trả và gia hạn. |
| 15 | `DangKyCho` | Yêu cầu chờ một cuốn theo thứ tự; chưa đồng nghĩa có quyền nhận. |
| 16 | `DotGiuCho` | Một lần xét ưu tiên cho tối đa hai người khi cuốn có thể giao. |
| 17 | `LuotGiuCho` | Lượt nhận của một người trong đợt, chỉ chạy thời hạn khi kích hoạt. |
| 18 | `BienBanSuCo` | Mất/hỏng, minh chứng, mức bồi thường và kết quả xử lý. |
| 19 | `PheDuyet` | Quyết định có thẩm quyền đối với định giá, thay thế hoặc thanh lý. |
| 20 | `KhoanThu` | Nghĩa vụ phải thu; phân biệt tiền phạt, bồi thường và tiền đã thực nhận. |
| 21 | `PhieuThu` | Chứng từ tiền thực nhận, người thu, giao dịch và các dòng phân bổ. |
| 22 | `ChiTietThu` | Phần tiền thực thu được phân bổ vào một khoản phải thu. |
| 23 | `BienNhanThayThe` | Chứng từ nhận bản mới để hoàn thành phần bồi thường bằng sách. |
| 24 | `BienBanThanhLy` | Căn cứ đưa cuốn hư nặng ra khỏi lưu thông, giữ lịch sử. |
| 25 | `ThongBao` | Thông báo trong hệ thống, sự kiện nguồn và trạng thái gửi. |
| 26 | `NhatKyThaoTac` | Dấu vết tác nhân, hành động và dữ liệu trước/sau khi xử lý. |

26 lớp gồm 25 lớp cụ thể và 1 lớp trừu tượng; không cộng enum, interface, service hoặc ViTriKe vào số lớp thực thể. Kế thừa được dùng cho SinhVien/GiangVien vì cùng là DocGia nhưng khác chính sách mượn. NhanVien có nhiệm vụ và quyền riêng, không mặc nhiên là một DocGia.

## 3. Thuộc tính, phương thức và bất biến của thực thể

Các mã liên kết trong bảng thể hiện quan hệ của sơ đồ lớp. Không lưu đồng thời một quan hệ bằng nhiều nguồn có thể tự sửa độc lập. Các phương thức bên dưới thể hiện hành vi của thực thể; kiểm tra cần dữ liệu của nhiều lớp do service điều phối.

### 3.1. DocGia

**Trách nhiệm:** Lớp trừu tượng chứa định danh, tư cách và quyền sử dụng; cung cấp chính sách mượn qua phương thức đa hình.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDocGia` | `string` | Mã định danh duy nhất của DocGia; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaTruong` | `string` | Mã thuộc trường đã xác minh; duy nhất theo hồ sơ độc giả. |
| `HoTen` | `string` | Họ tên để nhận diện đúng người; không để trống. |
| `TinhTrangQuyen` | `TrangThaiDocGia` | Trạng thái được sử dụng, bị dừng quyền hoặc đã đóng hồ sơ; độc lập với hạn thẻ. |
| `NgayRoiTruong` | `DateOnly?` | Ngày rời trường nếu có căn cứ; không tự thay đổi hạn trả sách. |
| `LyDoDungQuyen` | `string?` | Lý do dừng quyền sử dụng; bắt buộc khi dừng quyền. |
| `LoaiDocGia` | `LoaiDocGia` | Loại hồ sơ; thống nhất với lớp SinhVien/GiangVien, không đổi tự do. |

**Phương thức:** `SoNgayMuon(): int {abstract}`; `GioiHanCuon(): int {abstract}`; `DuocSuDung(): bool`; `DungQuyen(lyDo: string): void`.

**Bất biến:** MaTruong duy nhất; dừng quyền giữ nguyên nghĩa vụ; chỉ đóng hồ sơ khi đủ điều kiện. SinhVien/GiangVien kế thừa, không tạo hai hồ sơ độc giả cho cùng người.

### 3.2. SinhVien

**Trách nhiệm:** Độc giả sinh viên: tối đa 3 cuốn, 14 ngày mở cửa.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaSinhVien` | `string` | Mã sinh viên; phải thống nhất với MaTruong của hồ sơ, không tạo hai nguồn định danh mâu thuẫn. |

**Phương thức:** `SoNgayMuon(): int {result = 14}`; `GioiHanCuon(): int {result = 3}`.

**Bất biến:** SoNgayMuon() = 14 và GioiHanCuon() = 3; các thuộc tính chung kế thừa DocGia.

### 3.3. GiangVien

**Trách nhiệm:** Độc giả giảng viên: tối đa 5 cuốn, 30 ngày mở cửa.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaGiangVien` | `string` | Mã giảng viên; phải thống nhất với MaTruong của hồ sơ. |

**Phương thức:** `SoNgayMuon(): int {result = 30}`; `GioiHanCuon(): int {result = 5}`.

**Bất biến:** SoNgayMuon() = 30 và GioiHanCuon() = 5; các thuộc tính chung kế thừa DocGia.

### 3.4. NhanVien

**Trách nhiệm:** Nhân viên thủ thư hoặc quản lý, có quyền thực hiện nghiệp vụ theo vai trò.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaNhanVien` | `string` | Mã định danh duy nhất của NhanVien; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `HoTen` | `string` | Họ tên để nhận diện đúng người; không để trống. |
| `VaiTro` | `VaiTro` | Vai trò thủ thư hoặc người quản lý; không nhận vai trò độc giả. Quyền phải được kiểm tra tại service. |

**Phương thức:** `CoQuyen(chucNang: string): bool`.

**Bất biến:** Mã nhân viên duy nhất; vai trò chỉ là thủ thư/quản lý và thống nhất với tài khoản nhân viên. Đổi vai trò không xóa lịch sử.

### 3.5. TheThuVien

**Trách nhiệm:** Hiệu lực và lịch sử cấp/cấp lại thẻ gắn với một hồ sơ độc giả.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaThe` | `string` | Mã định danh duy nhất của TheThuVien; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaDocGia` | `string` | Mã liên kết tới hồ sơ DocGia. |
| `NgayCap` | `DateOnly` | Ngày cấp bản thẻ hiện tại. |
| `NgayHetHan` | `DateOnly` | Hạn thẻ tính theo tháng lịch; cấp lại giữ hạn cũ. |
| `TrangThai` | `TrangThaiThe` | Trạng thái của TheThuVien, theo `TrangThaiThe`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `NgayHieuLuc` | `DateOnly` | Ngày bắt đầu hiệu lực hiện tại; khôi phục theo UC02, tính bằng tháng lịch. |
| `MaTheTruoc` | `string?` | Thẻ đã bị thay khi cấp lại; rỗng khi cấp mới. |

**Phương thức:** `ConHieuLuc(ngay: DateOnly): bool`; `GiaHanThe(ngay: DateOnly): void`; `KhoiPhucThe(ngay: DateOnly): void`; `KhoaThe(lyDo: string): void`.

**Bất biến:** Một thẻ hiện hành cho một độc giả; cấp lại giữ hạn cũ; hiệu lực theo ngày và trạng thái.

### 3.6. TaiKhoan

**Trách nhiệm:** Danh tính đăng nhập, trạng thái tài khoản và vai trò truy cập.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaTaiKhoan` | `string` | Mã định danh duy nhất của TaiKhoan; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `TenDangNhap` | `string` | Tên đăng nhập duy nhất. |
| `MatKhauBam` | `string` | Dữ liệu mật khẩu đã băm kèm tham số/salt phù hợp; không lưu mật khẩu rõ. |
| `VaiTro` | `VaiTro` | Vai trò độc giả/thủ thư/quản lý; quyền phải kiểm tra ở service. |
| `TrangThai` | `TrangThaiTaiKhoan` | Trạng thái của TaiKhoan, theo `TrangThaiTaiKhoan`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `MaDocGia` | `string?` | Chủ tài khoản độc giả; độc quyền với MaNhanVien. |
| `MaNhanVien` | `string?` | Chủ tài khoản nhân viên; đúng một trong hai mã chủ sở hữu có giá trị. |

**Phương thức:** `XacThuc(matKhau: string): bool`; `VoHieuHoa(): void`.

**Bất biến:** Một tài khoản thuộc độc giả hoặc nhân viên, không đồng thời cả hai; vô hiệu hóa không xóa lịch sử.

### 3.7. DauSach

**Trách nhiệm:** Thông tin chung của một ấn bản; liên kết tác giả, thể loại và nhà xuất bản.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDauSach` | `string` | Mã định danh duy nhất của DauSach; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `TenSach` | `string` | Tên sách dùng tra cứu và đối chiếu bản thay thế. |
| `PhienBan` | `string` | Thông tin ấn bản để xác định bản tương đương. |
| `NgonNgu` | `string` | Ngôn ngữ ấn bản. |
| `GiaBia` | `decimal?` | Giá bìa có căn cứ, không âm; thiếu thì để rỗng, không tự gán 0. Trên đầu sách chỉ là giá tham khảo; giá chốt bồi thường lấy theo cuốn/sự cố. |
| `DanhSachMaTacGia` | `IReadOnlyList<string>` | Các tác giả của ấn bản; ít nhất một tác giả trong phạm vi mô hình. |
| `MaTheLoai` | `string` | Thể loại chính của đầu sách. |
| `MaNXB` | `string?` | Nhà xuất bản của ấn bản; rỗng nếu chưa có thông tin. |

**Phương thức:** `CapNhatThongTin(ten: string, ngonNgu: string, phienBan: string): void`; `GanTheLoai(ma: string): void`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.8. CuonSach

**Trách nhiệm:** Một bản vật lý, có mã, tình trạng, vị trí và quyền sở hữu riêng.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaCuon` | `string` | Mã định danh duy nhất của CuonSach; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaDauSach` | `string` | Mã liên kết tới ấn bản DauSach. |
| `GiaBia` | `decimal?` | Giá bìa có căn cứ, không âm; thiếu thì để rỗng, không tự gán 0. |
| `GiaDinhGia` | `decimal?` | Giá được quản lý phê duyệt khi thiếu giá bìa; gắn phê duyệt định giá. |
| `MaKe` | `string?` | Mã liên kết tới kệ hiện hành KeSach; rỗng khi chưa xếp kệ hoặc đã thanh lý. |
| `TinhTrangVatLy` | `TinhTrangSach` | Tốt, hao mòn hoặc mức hỏng; không đồng nhất trạng thái lưu thông. |
| `TrangThaiLuuThong` | `TrangThaiCuonSach` | Khả dụng/đang mượn/giữ chỗ/chờ xét ưu tiên/sửa/mất/thanh lý theo nghiệp vụ. |
| `ViTriChiTiet` | `string?` | Vị trí của cuốn trên kệ, có thể chưa xác định. |
| `PhienBanDuLieu` | `long` | Số phiên bản tăng khi cập nhật, hỗ trợ phát hiện dữ liệu thay đổi đồng thời. |
| `QuyenSoHuu` | `QuyenSoHuuSach` | ThuVien hoặc NguoiDoc; sách tìm lại sau tất toán thuộc người đọc theo BR34. |
| `MaPheDuyetGia` | `string?` | Phê duyệt định giá còn hiệu lực cho giá được sử dụng. |

**Phương thức:** `CoTheMuon(): bool`; `DanhDauMuon(): void`; `NhanTra(tinhTrang: TinhTrangSach): void`; `ChuyenKe(maKe: string, viTri: string?): void`; `ThanhLy(): void`.

**Bất biến:** Mã duy nhất; không đồng thời giao cho hai người; trạng thái thay qua nghiệp vụ. Cuốn thanh lý không có kệ hiện hành.

### 3.9. TacGia

**Trách nhiệm:** Thông tin tác giả được nhiều đầu sách tham chiếu.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaTacGia` | `string` | Mã định danh duy nhất của TacGia; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `HoTen` | `string` | Họ tên để nhận diện đúng người; không để trống. |

**Phương thức:** `CapNhatTen(ten: string): void`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.10. TheLoai

**Trách nhiệm:** Phân loại đầu sách; mô hình dùng một thể loại chính cho một đầu sách.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaTheLoai` | `string` | Mã định danh duy nhất của TheLoai; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `TenTheLoai` | `string` | Tên hiển thị và tra cứu, không để trống. |

**Phương thức:** `DoiTen(ten: string): void`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.11. NhaXuatBan

**Trách nhiệm:** Thông tin nhà xuất bản của ấn bản.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaNXB` | `string` | Mã định danh duy nhất của nhà xuất bản; không rỗng. Chỉ liên kết MaNXB trên DauSach mới có thể chưa được xác định. |
| `TenNXB` | `string` | Tên hiển thị và tra cứu, không để trống. |

**Phương thức:** `CapNhatTen(ten: string): void`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.12. KeSach

**Trách nhiệm:** Kệ vật lý và vị trí của kệ trong thư viện.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaKe` | `string` | Mã định danh duy nhất của KeSach; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `ViTri` | `ViTriKe` | Giá trị vị trí kệ trong thư viện; dùng ViTriKe để gom các thành phần vị trí. |
| `DangSuDung` | `bool` | Kệ còn sử dụng; không ngừng dùng khi chưa xử lý cuốn đang liên kết. |

**Phương thức:** `DoiViTri(viTri: ViTriKe): void`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.13. PhieuMuon

**Trách nhiệm:** Giao dịch mượn của độc giả, chứa một hoặc nhiều dòng mượn.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaPhieu` | `string` | Mã định danh duy nhất của PhieuMuon; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaDocGia` | `string` | Mã liên kết tới hồ sơ DocGia. |
| `NgayMuon` | `DateOnly` | Ngày giao sách; không tính ngày này khi cộng thời hạn mượn. |
| `TrangThai` | `TrangThaiPhieuMuon` | Trạng thái của PhieuMuon, theo `TrangThaiPhieuMuon`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `MaNguoiLap` | `string` | Mã liên kết tới nhân viên lập giao dịch/biên bản. |
| `MaYeuCau` | `string` | Định danh yêu cầu chống xử lý lặp; cùng mã khác nội dung báo xung đột. |
| `DanhSachDongMuon` | `IReadOnlyList<ChiTietMuon>` | Các dòng thuộc phiếu; phiếu được xác nhận có ít nhất một dòng. |

**Phương thức:** `ThemDongMuon(dong: ChiTietMuon): void`; `XacNhanPhieu(): void`; `DaHoanTat(): bool`. DaHoanTat chỉ trả đúng khi tất cả dòng đã có NgayKetThucNghiaVu; không suy ra từ trạng thái khả dụng của cuốn.

**Bất biến:** Một phiếu thuộc một độc giả; các dòng nhất quán với trạng thái cuốn; chỉ hoàn tất khi đủ điều kiện mọi dòng.

### 3.14. ChiTietMuon

**Trách nhiệm:** Theo dõi một cuốn trong một lượt mượn, độc lập hạn trả, nhận trả và gia hạn.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDong` | `string` | Mã định danh duy nhất của ChiTietMuon; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaCuon` | `string` | Mã liên kết tới cuốn vật lý CuonSach. |
| `NgayMuon` | `DateOnly` | Ngày giao sách; không tính ngày này khi cộng thời hạn mượn. |
| `HanTra` | `DateOnly` | Hạn trả đã chốt; chỉ thay đổi qua gia hạn hợp lệ. |
| `NgayNhan` | `DateOnly?` | Ngày thực nhận sách; không ghi nếu chỉ báo mất hoặc nộp tiền. |
| `DaGiaHan` | `bool` | Đánh dấu đã gia hạn một lần; chọn thêm 3 ngày cũng không được gia hạn lần hai. |
| `TinhTrangKhiGiao` | `string` | Dấu vết lúc giao, căn cứ đối chiếu khi nhận. |
| `TinhTrangKhiNhan` | `string?` | Tình trạng nhận thực tế, rỗng khi chưa nhận. |
| `TrangThai` | `TrangThaiMuon` | Trạng thái của ChiTietMuon, theo `TrangThaiMuon`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `NgayChotPhat` | `DateOnly?` | Ngày nhận thực tế theo Q01 hoặc ngày tất toán mất theo Q02; không dùng ngày báo mất. |
| `MaPhieu` | `string` | Phiếu chứa dòng mượn. |
| `LoaiDocGiaLucMuon` | `LoaiDocGia` | Loại độc giả lúc giao, giữ căn cứ 14/30 và mốc 21/37 ngay cả khi hồ sơ đổi loại. |
| `NgayKetThucNghiaVu` | `DateOnly?` | Ngày đủ điều kiện kết thúc dòng; sách mất chỉ khi tất toán Q02. |

**Phương thức:** `NhanTra(ngay: DateOnly, tinhTrang: string): void`; `GiaHanMotLan(soNgay: int, ngayXuLy: DateOnly, lich: ILichMoCua): void`; `BaoMat(ngayBao: DateOnly): void`; `ChotPhat(ngay: DateOnly): void`; `KetThucNghiaVu(ngay: DateOnly): void`; `TinhNgayTre(den: DateOnly, lich: ILichMoCua): int`.

**Bất biến:** Chỉ gia hạn dòng đang mượn chưa quá hạn, một lần 3/5/7 ngày; không gia hạn dòng đã nhận trả hoặc đã báo mất. Kiểm tra quyền, thẻ, nợ và hàng chờ do GiaHanService thực hiện trước khi gọi. Không giả ngày nhận sách mất; nhận trả và tiền nợ tách riêng theo Q01.

### 3.15. DangKyCho

**Trách nhiệm:** Yêu cầu chờ một cuốn theo thứ tự; chưa đồng nghĩa có quyền nhận.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDangKy` | `string` | Mã định danh duy nhất của DangKyCho; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaDocGia` | `string` | Mã liên kết tới hồ sơ DocGia. |
| `MaCuon` | `string` | Mã liên kết tới cuốn vật lý CuonSach. |
| `ThuTu` | `long` | Số thứ tự trong hàng chờ hoặc đợt; không tự đổi thứ tự của người khác. |
| `TrangThai` | `TrangThaiDangKyCho` | Trạng thái của DangKyCho, theo `TrangThaiDangKyCho`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `NgayDangKy` | `DateTimeOffset` | Thời điểm đăng ký ban đầu, dùng sắp hàng cùng ThuTu. |
| `LyDoKetThuc` | `string?` | Lý do hủy/kết thúc, lưu cùng lịch sử. |
| `MaCuonBanDau` | `string` | Cuốn lúc đăng ký, giữ liên kết lịch sử khi chuyển bản thay thế. |
| `MaDongNguon` | `string?` | Dòng nguồn để kiểm tra mốc thẻ; không tự suy diễn nếu thiếu. |
| `TiepTucChoBanThayThe` | `bool?` | Chưa chọn, đồng ý hoặc không đồng ý tiếp tục; chỉ chuyển người đã đồng ý. |

**Phương thức:** `Huy(lyDo: string): void`; `KetThuc(lyDo: string): void`; `ChuyenBan(maCuonMoi: string): void`; `ConHieuLuc(): bool`.

**Bất biến:** Mỗi độc giả tối đa một đăng ký hiệu lực toàn thư viện; hủy không xóa lịch sử và không đảo thứ tự.

### 3.16. DotGiuCho

**Trách nhiệm:** Một lần xét ưu tiên cho tối đa hai người khi cuốn có thể giao.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDot` | `string` | Mã định danh duy nhất của DotGiuCho; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaCuon` | `string` | Mã liên kết tới cuốn vật lý CuonSach. |
| `MaDongNguon` | `string` | Mã liên kết tới ChiTietMuon dùng xác định ngày bắt đầu và loại người mượn nguồn. |
| `TrangThai` | `TrangThaiDotGiuCho` | Trạng thái của DotGiuCho, theo `TrangThaiDotGiuCho`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `MaSuKien` | `string` | Định danh sự kiện nguồn để chống thông báo/đợt xử lý trùng. |
| `DanhSachLuot` | `IReadOnlyList<LuotGiuCho>` | Một hoặc hai lượt được chọn; không thêm người thứ ba vào đợt. |

**Phương thức:** `GhiNhanUngVien(dangKy: DangKyCho): void`; `KetThuc(): void`.

**Bất biến:** Tối đa hai người, tối đa một quyền nhận đang hiệu lực; không tạo đợt mới cùng lần sẵn sàng để lách BR26.

### 3.17. LuotGiuCho

**Trách nhiệm:** Lượt nhận của một người trong đợt, chỉ chạy thời hạn khi kích hoạt.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaLuot` | `string` | Mã định danh duy nhất của LuotGiuCho; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `ThuTu` | `int` | Số thứ tự trong hàng chờ hoặc đợt; không tự đổi thứ tự của người khác. |
| `ThoiDiemKichHoat` | `DateTimeOffset?` | Thời điểm thật sự cấp quyền nhận; rỗng nếu chưa đến lượt. |
| `HanNhan` | `DateTimeOffset?` | 17:00 ngày mở cửa thứ hai sau kích hoạt; người thứ hai có hạn riêng. |
| `TrangThai` | `TrangThaiLuotGiuCho` | Trạng thái của LuotGiuCho, theo `TrangThaiLuotGiuCho`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `MaDangKy` | `string` | Mã liên kết tới DangKyCho được chọn vào lượt. |
| `MaDot` | `string` | Đợt chứa lượt nhận. |

**Phương thức:** `KichHoat(luc: DateTimeOffset, lich: ILichMoCua): void`; `DanhDauHetLuot(): void`; `XacNhanDaNhan(): void`.

**Bất biến:** ThuTu chỉ 1 hoặc 2; người thứ hai chỉ chạy hạn khi thực sự được kích hoạt.

### 3.18. BienBanSuCo

**Trách nhiệm:** Mất/hỏng, minh chứng, mức bồi thường và kết quả xử lý.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaSuCo` | `string` | Mã định danh duy nhất của BienBanSuCo; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaDongMuon` | `string` | Mã liên kết tới ChiTietMuon xảy ra sự cố. |
| `Loai` | `LoaiSuCo` | Loại nghiệp vụ theo `LoaiSuCo`, xác định cách xử lý tương ứng. |
| `MucDo` | `TinhTrangSach` | Mức hỏng vật lý; khi Loai = Mat áp dụng mất sách 100%, không suy ra từ mức hỏng. |
| `TrangThai` | `TrangThaiSuCo` | Trạng thái của BienBanSuCo, theo `TrangThaiSuCo`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `NgayBao` | `DateOnly` | Ngày ghi nhận mất/hỏng; báo mất không dừng phạt. |
| `MinhChung` | `string` | Mô tả và tham chiếu bằng chứng sự cố. |
| `GiaTriSachChot` | `decimal?` | Giá đã chốt cho sự cố, không sửa ngược khi đổi giá danh mục; rỗng nếu chờ định giá. |
| `TyLeBoiThuong` | `decimal?` | Tỷ lệ 0, 0.5 hoặc 1 ứng với mức độ; rỗng khi chưa kết luận. |
| `NgayHoanTat` | `DateOnly?` | Ngày đủ điều kiện kết thúc sự cố; mất sách theo Q02. |
| `MaNguoiLap` | `string` | Nhân viên lập biên bản. |
| `MaPheDuyet` | `string?` | Phê duyệt căn cứ giá/phương án khi cần. |

**Phương thức:** `LapBienBan(ngay: DateOnly, minhChung: string): void`; `GhiKetLuan(mucDo: TinhTrangSach, gia: decimal): void`; `GhiNhanTimLai(ngay: DateOnly): void`; `HoanTat(ngay: DateOnly): void`.

**Bất biến:** Chỉ chốt giá có căn cứ; hao mòn 0%, hư nhẹ 50%, mất/hư nặng 100%; tất toán mất đúng Q02.

### 3.19. PheDuyet

**Trách nhiệm:** Quyết định có thẩm quyền đối với định giá, thay thế hoặc thanh lý.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaPheDuyet` | `string` | Mã định danh duy nhất của PheDuyet; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaNguoiDuyet` | `string` | Mã liên kết tới nhân viên quản lý có thẩm quyền. |
| `CanCu` | `string` | Căn cứ xác minh/phê duyệt, không rỗng khi ra quyết định. |
| `QuyetDinh` | `QuyetDinhPheDuyet` | Chờ duyệt, chấp thuận hoặc từ chối. |
| `NgayDuyet` | `DateTimeOffset?` | Thời điểm quyết định thực tế; rỗng khi còn chờ. |
| `LoaiPheDuyet` | `LoaiPheDuyet` | Phân biệt định giá, nhận bản thay thế và thanh lý. |
| `LoaiDoiTuong` | `string` | Loại thực thể chịu tác động. |
| `MaDoiTuong` | `string` | Mã đối tượng được xác định bởi LoaiDoiTuong; phải tham chiếu đúng đối tượng nghiệp vụ. |
| `LyDo` | `string?` | Lý do từ chối hoặc giải thích quyết định. |

**Phương thức:** `ChapThuan(): void`; `TuChoi(lyDo: string): void`.

**Bất biến:** Người duyệt có thẩm quyền, quyết định gắn đúng loại/đối tượng; không sử dụng phê duyệt của đối tượng khác.

### 3.20. KhoanThu

**Trách nhiệm:** Nghĩa vụ phải thu; phân biệt tiền phạt, bồi thường và tiền đã thực nhận.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaKhoan` | `string` | Mã định danh duy nhất của KhoanThu; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `Loai` | `LoaiKhoanThu` | Loại nghiệp vụ theo `LoaiKhoanThu`, xác định cách xử lý tương ứng. |
| `SoTienChot` | `decimal` | Số phải thu tại lần chốt, không âm; không đồng nghĩa đã nhận tiền. |
| `TrangThai` | `TrangThaiKhoanThu` | Trạng thái của KhoanThu, theo `TrangThaiKhoanThu`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `NgayChot` | `DateOnly?` | Mốc tính số tiền đã chốt; phạt mất còn tăng đến ngày tất toán. |
| `CachTatToan` | `CachTatToan?` | Tiền hoặc bản thay thế; bản thay thế chỉ cho phần bồi thường đủ điều kiện. |
| `MaDocGia` | `string` | Người có nghĩa vụ. |
| `MaDongMuon` | `string` | Dòng mượn làm phát sinh khoản. |
| `MaSuCo` | `string?` | Sự cố nếu là bồi thường. |
| `MaBienNhan` | `string?` | Biên nhận dùng tất toán phần bồi thường bằng sách. |
| `DanhSachChiTietThu` | `IReadOnlyList<ChiTietThu>` | Các lần thu đã xác nhận; tổng đã thu tính từ đây, không lưu hai tổng độc lập. |

**Phương thức:** `SoTienConNo(): decimal`; `ChotSoTien(soTien: decimal, ngay: DateOnly): void`; `XacNhanThuDu(soTien: decimal): void`; `TatToanBangBanThayThe(bienNhan: BienNhanThayThe): void`.

**Bất biến:** Tách phạt và bồi thường; không trả góp khoản đã chốt, nhưng được ghi nhận phần phạt tăng sau lần thu trước đến tất toán Q02; không thu trùng. SoTienConNo bằng số phải thu trừ tổng dòng thu đã xác nhận; khoản được hủy hoặc tất toán hợp lệ bằng bản thay thế không còn nợ. XacNhanThuDu chỉ được gọi từ ThanhToanService sau khi có chứng từ thực nhận và phải kiểm tra đủ phần còn phải thu. Chốt lại phạt không sửa số tiền trên phiếu thu cũ.

### 3.21. PhieuThu

**Trách nhiệm:** Chứng từ tiền thực nhận, người thu, giao dịch và các dòng phân bổ.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaPhieuThu` | `string` | Mã định danh duy nhất của PhieuThu; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `NgayThu` | `DateTimeOffset` | Thời điểm tiền được thực nhận. |
| `SoTienThucThu` | `decimal` | Tổng tiền thực nhận, bằng tổng ChiTietThu; không ghi tiền của bản thay thế vào đây. |
| `MaNguoiThu` | `string` | Mã liên kết tới nhân viên xác nhận tiền thực nhận. |
| `MaGiaoDich` | `string` | Mã đối soát giao dịch, giúp tránh nhận/thu lại sau lỗi lưu. |
| `HinhThuc` | `HinhThucThanhToan` | Hình thức thu tiền; không triển khai cổng thanh toán ngoài phạm vi. |
| `TrangThai` | `TrangThaiPhieuThu` | Trạng thái của PhieuThu, theo `TrangThaiPhieuThu`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `MaYeuCau` | `string` | Định danh yêu cầu chống xử lý lặp; cùng mã khác nội dung báo xung đột. |
| `MaDocGia` | `string` | Người nộp tiền. |
| `DanhSachChiTiet` | `IReadOnlyList<ChiTietThu>` | Các dòng phân bổ tiền; tổng bằng SoTienThucThu. |

**Phương thức:** `ThemChiTiet(dong: ChiTietThu): void`; `XacNhanDaThu(): void`.

**Bất biến:** Chỉ xác nhận tiền thực nhận; tổng chi tiết bằng tổng phiếu; xử lý lặp không tạo phiếu thứ hai.

### 3.22. ChiTietThu

**Trách nhiệm:** Phần tiền thực thu được phân bổ vào một khoản phải thu.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaDongThu` | `string` | Mã định danh duy nhất của ChiTietThu; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaKhoan` | `string` | Mã liên kết tới KhoanThu được phân bổ thanh toán. |
| `SoTien` | `decimal` | Số tiền thực nhận phân bổ vào một khoản, lớn hơn 0 và không thu trùng phần đã nhận. |
| `MaPhieuThu` | `string` | Phiếu thu chứa dòng phân bổ. |

**Phương thức:** `KiemTraSoTien(khoan: KhoanThu): bool`.

**Bất biến:** Mã duy nhất, dữ liệu hợp lệ; cập nhật thông tin không làm mất liên kết hoặc lịch sử đã phát sinh.

### 3.23. BienNhanThayThe

**Trách nhiệm:** Chứng từ nhận bản mới để hoàn thành phần bồi thường bằng sách.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaBienNhan` | `string` | Mã định danh duy nhất của BienNhanThayThe; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaCuonCu` | `string` | Mã liên kết tới cuốn mất/hư nặng cần thay thế. |
| `MaCuonMoi` | `string` | Mã liên kết tới bản mới có mã khác cuốn cũ. |
| `NgayNhan` | `DateOnly` | Ngày thực nhận bản thay thế, không phải ngày trả vật lý cuốn cũ. |
| `MaSuCo` | `string` | Mã liên kết tới BienBanSuCo liên quan. |
| `MaNguoiDuyet` | `string` | Mã liên kết tới nhân viên quản lý có thẩm quyền. |
| `MaPheDuyet` | `string` | Phê duyệt đúng bản thay thế. |
| `MaNguoiNhan` | `string` | Nhân viên kiểm tra và nhận bản thật. |

**Phương thức:** `XacNhanBanThayThe(pheDuyet: PheDuyet): void`.

**Bất biến:** Cuốn mới khác mã cũ, có phê duyệt tương đương; không nhận thêm nếu đã bồi thường đủ bằng tiền cùng nghĩa vụ.

### 3.24. BienBanThanhLy

**Trách nhiệm:** Căn cứ đưa cuốn hư nặng ra khỏi lưu thông, giữ lịch sử.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaThanhLy` | `string` | Mã định danh duy nhất của BienBanThanhLy; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaCuon` | `string` | Mã liên kết tới cuốn vật lý CuonSach. |
| `LyDo` | `string` | Lý do thực hiện/từ chối hoặc thanh lý; lưu phù hợp nghiệp vụ. |
| `NgayThanhLy` | `DateOnly` | Ngày thực hiện thanh lý đã được duyệt. |
| `MaNguoiDuyet` | `string` | Mã liên kết tới nhân viên quản lý có thẩm quyền. |
| `MaPheDuyet` | `string` | Quyết định cho phép thanh lý. |
| `MaSuCo` | `string?` | Sự cố liên quan nếu có. |

**Phương thức:** `XacNhanThanhLy(): void`.

**Bất biến:** Chỉ thanh lý cuốn hư nặng có duyệt; hư nhẹ chuyển sửa; giữ lịch sử cuốn cũ.

### 3.25. ThongBao

**Trách nhiệm:** Thông báo trong hệ thống, sự kiện nguồn và trạng thái gửi.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaThongBao` | `string` | Mã định danh duy nhất của ThongBao; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `NguoiNhan` | `string` | Mã độc giả nhận thông báo, chỉ người có quyền được xem. |
| `NoiDung` | `string` | Nội dung thông báo và hạn nhận nếu có. |
| `TrangThai` | `TrangThaiThongBao` | Trạng thái của ThongBao, theo `TrangThaiThongBao`; chỉ chuyển qua nghiệp vụ hợp lệ. |
| `Loai` | `LoaiThongBao` | Loại nghiệp vụ theo `LoaiThongBao`, xác định cách xử lý tương ứng. |
| `MaSuKien` | `string` | Định danh sự kiện nguồn để chống thông báo/đợt xử lý trùng. |
| `ThoiDiemTao` | `DateTimeOffset` | Thời điểm tạo bản ghi. |
| `SoLanGui` | `int` | Số lần thử gửi, không âm; lỗi gửi không hủy giao dịch. |

**Phương thức:** `DanhDauDaGui(): void`; `YeuCauGuiLai(): void`.

**Bất biến:** Không tạo trùng cùng sự kiện/người nhận/loại; gửi lỗi không hoàn tác giao dịch gốc.

### 3.26. NhatKyThaoTac

**Trách nhiệm:** Dấu vết tác nhân, hành động và dữ liệu trước/sau khi xử lý.

| Thuộc tính | Kiểu | Ý nghĩa và ràng buộc |
| --- | --- | --- |
| `MaNhatKy` | `string` | Mã định danh duy nhất của NhatKyThaoTac; không đổi hoặc tái sử dụng cho bản ghi khác. |
| `MaTaiKhoanThucHien` | `string?` | Mã liên kết tới TaiKhoan của người thao tác; rỗng cho tác vụ hệ thống. |
| `HanhDong` | `string` | Thao tác cần truy vết. |
| `ThoiDiem` | `DateTimeOffset` | Thời điểm ghi nhận thao tác. |
| `LyDo` | `string?` | Lý do thực hiện/từ chối hoặc thanh lý; lưu phù hợp nghiệp vụ. |
| `LoaiDoiTuong` | `string` | Loại thực thể chịu tác động. |
| `MaDoiTuong` | `string` | Mã đối tượng được xác định bởi LoaiDoiTuong; phải tham chiếu đúng đối tượng nghiệp vụ. |
| `DuLieuTruoc` | `string?` | Dữ liệu cần truy vết trước thao tác; không chứa mật khẩu. |
| `DuLieuSau` | `string?` | Dữ liệu cần truy vết sau thao tác; không chứa mật khẩu. |
| `LoaiChuThe` | `LoaiChuTheThaoTac` | NguoiDung hoặc HeThong; tác vụ nội bộ có thể không có tài khoản. |

**Phương thức:** `GhiLaiThaoTac(): void`.

**Bất biến:** Không ghi mật khẩu; phân biệt người dùng/tác vụ hệ thống; không xóa lịch sử để che nghĩa vụ.

### 3.27. ViTriKe — đối tượng giá trị

`Domain/ValueObjects/ViTriKe.cs` biểu diễn vị trí của kệ, gồm `Khu: string`, `Tang: string`, `Day: string`. Các thành phần không rỗng; hai vị trí có cùng giá trị được coi là bằng nhau. `KeSach.MaKe` là định danh kệ, còn `KeSach.ViTri` là giá trị vị trí; đổi vị trí không tạo một kệ mới. `CuonSach.ViTriChiTiet` mô tả chỗ trên kệ khi cần, không thay thế MaKe.

## 4. Quan hệ giữa các lớp

| Quan hệ | Bội số và ràng buộc |
| --- | --- |
| DocGia — SinhVien/GiangVien | Kế thừa; các hàm SoNgayMuon/GioiHanCuon được ghi đè đúng loại. |
| DocGia — TheThuVien | 1 — 0..n trong lịch sử; tối đa một thẻ hiện hành. |
| TaiKhoan — DocGia/NhanVien | Mỗi tài khoản thuộc đúng một hồ sơ độc giả hoặc nhân viên; không đồng thời cả hai. |
| DauSach — TacGia | Nhiều–nhiều; mỗi đầu sách có ít nhất một tác giả trong mô hình. |
| DauSach — TheLoai/NhaXuatBan | Một thể loại chính; 0 hoặc 1 nhà xuất bản; mỗi loại/NXB liên kết nhiều đầu sách. |
| DauSach — CuonSach | 1 — 0..n; mỗi cuốn thuộc đúng một ấn bản. |
| KeSach — CuonSach | Một kệ chứa nhiều cuốn; một cuốn có 0 hoặc 1 kệ hiện hành. |
| PhieuMuon — ChiTietMuon | Composition; phiếu xác nhận có 1..n dòng, mỗi dòng thuộc một phiếu. |
| CuonSach — ChiTietMuon | Một cuốn có nhiều dòng lịch sử, tối đa một lượt đang chiếm quyền mượn tại một thời điểm. |
| DocGia — DangKyCho | Một độc giả có nhiều đăng ký lịch sử, tối đa một đăng ký hiệu lực toàn thư viện. |
| DotGiuCho — LuotGiuCho | Composition; đợt có 1..2 lượt, chỉ một lượt đang có quyền; không có ứng viên thì không tạo quyền nhận. |
| DangKyCho — LuotGiuCho | Một đăng ký có thể tham gia nhiều lượt trong lịch sử qua các đợt; không có hai quyền nhận đồng thời. |
| DotGiuCho — ChiTietMuon | Dòng nguồn là ChiTietMuon dùng tính mốc 21/37; không phải một thực thể mới. |
| ChiTietMuon — BienBanSuCo/KhoanThu | Dòng liên kết sự cố và các nghĩa vụ; phạt, bồi thường có khoản riêng. |
| PhieuThu — ChiTietThu — KhoanThu | Phiếu có 1..n chi tiết, mỗi chi tiết trỏ một khoản. Khoản có 0..n chi tiết lịch sử; phần phạt tăng sau lần thu trước theo Q02 phải khấu trừ số đã thu. |
| BienNhanThayThe — CuonSach | Hai vai trò cuốn cũ/cuốn mới, mã khác nhau; không nhân đôi đối tượng cuốn. |
| PheDuyet — đối tượng nghiệp vụ | Mã và loại đối tượng xác định định giá, thay thế hoặc thanh lý; kiểm tra đúng người duyệt và đối tượng. |
| NhatKyThaoTac — TaiKhoan | Tác vụ hệ thống có thể không có mã tài khoản; phải có LoaiChuThe và căn cứ thao tác. |

Nhận trả vật lý, kết thúc nghĩa vụ sách và thanh toán là các trạng thái riêng. Lưu ngày nhận thực tế theo Q01; không tiếp tục phạt cuốn đã trả vì nợ chưa thanh toán. Không cho báo mất tự giải phóng hạn mức. Cuốn dùng được chỉ giao tiếp sau kiểm tra tình trạng và hàng chờ.

### 4.1. Các trạng thái cần phân biệt

| Enum | Giá trị thiết kế chính | Ràng buộc |
| --- | --- | --- |
| `TrangThaiDocGia` | HoatDong, DungQuyen, DaDong | Còn nợ và hết hạn thẻ là điều kiện kiểm tra riêng; không tự đóng hồ sơ. |
| `TrangThaiThe` | HienHanh, DaThayThe, DaHuy | Còn hạn được tính từ ngày hiệu lực/hết hạn; thẻ hiện hành có thể đã hết hạn và được khôi phục. |
| `TrangThaiCuonSach` | SanSang, DangMuon, ChoXetUuTien, DangGiuCho, DangSua, Mat, DaThanhLy, KhongLuuThong | Chỉ giao cuốn sẵn sàng hoặc giữ đúng người; cuốn thuộc người đọc không được lưu thông dù còn tốt. |
| `TrangThaiMuon` | DangMuon, DaNhanChoXuLy, MatChoXuLy, HoanTat | DaNhanChoXuLy có NgayNhan nhưng có thể còn nghĩa vụ; MatChoXuLy không có ngày nhận giả. |
| `TrangThaiPhieuMuon` | Nhap, DangXuLy, HoanTat | Nhap chỉ ở bộ nhớ trước xác nhận; phiếu đã lưu phải có dòng hợp lệ. |
| `TrangThaiDangKyCho` | DangCho, DaDuocChon, DangCoQuyenNhan, TamDung, DaNhan, DaHuy, DaKetThuc | Bốn trạng thái đầu còn hiệu lực khi kiểm tra một đăng ký/độc giả; ba trạng thái cuối không còn hiệu lực. |
| `TrangThaiDotGiuCho` | DangXuLy, TamDung, KetThuc | Không mở lại đợt đã kết thúc để thêm người thứ ba trong cùng lần sẵn sàng. |
| `TrangThaiLuotGiuCho` | ChuaKichHoat, DangCoQuyen, TamDung, DaNhan, HetLuot, DaHuy | Tạm dừng thì không có quyền giao sách; không coi lượt chưa kích hoạt là đã hết hạn. |
| `TrangThaiKhoanThu` | ChuaThu, DaThuDu, ChoDoiSoat, DaTatToanBangSach, DaHuy | Phạt mất tăng có thể tạo phần còn phải thu mới; chứng từ cũ giữ nguyên. |
| `TrangThaiSuCo` | ChoXuLy, ChoPheDuyet, DangGiaiQuyet, ChoDoiSoat, HoanTat | Chưa đủ căn cứ hoặc còn đối soát thì chưa hoàn tất. |
| `TrangThaiTaiKhoan` | HoatDong, BiKhoa, VoHieuHoa | Khóa đăng nhập không đồng nghĩa xóa công nợ; nhân viên vẫn tiếp nhận trả sách theo quyền. |
| `TrangThaiPhieuThu` | ChoXacNhan, DaXacNhan | Chỉ DaXacNhan được cộng vào thực thu; không sửa/xóa phiếu đã xác nhận để điều chỉnh công nợ. |
| `TrangThaiThongBao` | ChoGui, DaGui, GuiLoi | Thử gửi lại giữ định danh, không tạo thông báo trùng. |

Các enum phân loại còn lại gồm: `LoaiDocGia` (SinhVien/GiangVien), `VaiTro` (DocGia/ThuThu/QuanLy), `TinhTrangSach` (Tot/HaoMon/HuNhe/HuNang), `LoaiSuCo` (Mat/Hong), `LoaiKhoanThu` (PhatQuaHan/BoiThuong), `LoaiPheDuyet` (DinhGia/BanThayThe/ThanhLy), `QuyetDinhPheDuyet` (ChoDuyet/ChapThuan/TuChoi), `CachTatToan` (Tien/BanThayThe), `QuyenSoHuuSach` (ThuVien/NguoiDoc), `LoaiChuTheThaoTac` (NguoiDung/HeThong), `HinhThucThanhToan` (TienMat), `LoaiThongBao` (NhacQuaHan/QuyenNhan/HetLuot/SuCoHangCho). Bản thay thế không phải một hình thức tiền thực thu.

### 4.2. Điều kiện chuyển trạng thái quan trọng

- Nhận trả: CuonSach chuyển sang ChoXetUuTien hoặc DangSua theo tình trạng; ChiTietMuon ghi NgayNhan và NgayChotPhat. Còn nợ thì dòng chưa hoàn tất, nhưng cuốn không còn thuộc quyền giữ của người cũ.
- Báo mất: CuonSach = Mat, ChiTietMuon = MatChoXuLy; giữ hạn mức và tiếp tục tính phạt theo Q02. Tìm lại trước tất toán chuyển sang quy trình nhận trả; nghĩa vụ bồi thường mất được hủy và khoản đã nhận phải đối soát.
- Tất toán mất: chỉ khi bồi thường và tiền phạt đủ điều kiện mới ghi NgayKetThucNghiaVu. Tìm lại sau tất toán không ghi một lần trả mới; cuốn cũ thuộc người đọc, không tái lưu thông.
- Người thứ nhất mượn thành công: kết thúc đợt; lượt chưa kích hoạt của người thứ hai kết thúc cùng đợt, nhưng DangKyCho có thể trở lại DangCho sau kiểm tra hợp lệ theo dòng nguồn mới.
- Khi cuốn hỏng đã sửa xong, nhân viên kiểm tra lại trước khi xét hàng chờ. Không phục hồi hạn nhận cũ đã trôi qua; cấp lượt hợp lệ với thời hạn mới từ lúc thực sự cấp quyền. Quy tắc tối đa hai người vẫn áp dụng trong mỗi đợt.


**Các ca Lan và Quân cần đối chiếu khi tích hợp**

| Tình huống | Trạng thái trước | Kết quả phải lưu | Điều kiện cần kiểm tra |
| --- | --- | --- | --- |
| UC05 · Nhận lại cuốn dùng được, còn tiền phạt | Cuốn và dòng mượn đều `DangMuon` | Dòng thành `DaNhanChoXuLy`, ghi ngày nhận và ngày chốt phạt. Cuốn chuyển `ChoXetUuTien`. Khoản phạt `ChuaThu`. | Phạt dừng ở ngày nhận thực tế. Nếu đã nhận tiền mà chưa xác minh được chứng từ, chuyển xử lý đối soát trước khi thu lại. |
| UC08 · Báo mất, chưa tất toán | Dòng `DangMuon`, chưa ghi ngày nhận | Cuốn thành `Mat`, dòng thành `MatChoXuLy`. Ghi biên bản và giữ nguyên số cuốn đang tính vào hạn mức. | Theo Q02, chỉ kết thúc nghĩa vụ khi đã bồi thường và trả đủ phạt tính đến ngày tất toán. |
| UC03/UC07 · Người được giữ chỗ đến nhận | Cuốn `DangGiuCho`, lượt `DangCoQuyen` | Ghi dòng mượn, chuyển cuốn sang `DangMuon` và lượt nhận sang `DaNhan` trong cùng giao dịch. | Kiểm tra người nhận, hạn nhận và điều kiện mượn tại thời điểm giao. Nếu là người thứ nhất, xét lại đăng ký của người thứ hai theo dòng nguồn mới. |

## 5. Service và ánh xạ Use Case

Service thuộc `Application/Services` kiểm tra quyền, tải dữ liệu hiện tại, thực thi quy tắc liên đối tượng, gọi hành vi thực thể rồi lưu nhất quán.

| UC | Service và phương thức | Trách nhiệm |
| --- | --- | --- |
| UC01 | `TheService.CapThe()`, `CapLaiThe()` | Xác minh tư cách, cấp đúng hồ sơ, khóa thẻ cũ khi cấp lại. |
| UC02 | `TheService.GiaHanThe()`, `KhoiPhucThe()` | Thẻ còn hạn gia hạn trong cửa sổ; thẻ hết hạn khôi phục đúng điều kiện. |
| UC03 | `MuonTraService.KiemTraDieuKienMuon()`, `TinhHanTra()`, `MuonNhieuCuon()` | Kiểm tra toàn bộ danh sách; một cuốn lỗi thì từ chối cả yêu cầu. |
| UC04 | `GiaHanService.GiaHanDongMuon()` | Gia hạn từng dòng một lần; gọi ChiTietMuon.GiaHanMotLan(). |
| UC05 | `MuonTraService.TraTungCuon()`, `TinhPhatQuaHan()`; `ThanhToanService.GhiNhanThucThu()`, `DoiSoatKhoan()` | Nhận trả và tiền thực thu riêng; thanh toán sau nhận trả không tạo lần trả mới. |
| UC06 | `HangChoService.DangKyCho()`, `HuyDangKy()`, `KetThucKhiMatQuyen()` | Một đăng ký hiệu lực, bảo toàn thứ tự/lịch sử. |
| UC07 | `HangChoService.XetDotGiuCho()`, `ChuyenLuot()`, `XuLyHetLuot()`, `XetLaiNguoiHai()`, `TamDungDot()`, `ChuyenSangBanBoSung()` | Tối đa hai lượt, quyền tuần tự; người thứ hai không được nhận cuốn đang cho mượn. |
| UC08 | `SuCoService.BaoMat()`, `GhiNhanHuHong()`, `ChotBoiThuong()`, `GhiNhanTimLai()`, `TatToanMatSach()` | Giá có căn cứ, phạt riêng; tìm lại phân nhánh trước/sau tất toán Q02. |
| UC09 | `SuCoService.PheDuyetBanThayThe()`, `NhanBanThayThe()`; `SachService.PheDuyetThanhLy()`, `ThanhLyCuon()` | Nhận bản mới có duyệt; thanh lý có căn cứ, mã cuốn mới khác cũ. |
| UC10 | `HoSoService.XacNhanNghiaVu()`, `DongHoSo()`, `HanCheKhiBuocRoiTruong()` | Đối soát sách/nợ, dừng quyền hoặc đóng đúng điều kiện, giữ lịch sử. |
| UC11 | `SachService.ThemDauSach()`, `CapNhatDauSach()`, `NhapCuon()`, `ChuyenKe()`, `QuanLyDanhMuc()`, `PheDuyetDinhGia()` | Quản lý ấn bản, tác giả, thể loại, NXB, kệ và bản vật lý; không sửa trực tiếp trạng thái để bỏ qua mượn/trả. |
| UC12 | `TraCuuThongKeService.TimSach()`, `XemNghiaVu()`, `ThongKe()` | Chỉ đọc dữ liệu đúng quyền; phân biệt dự kiến, phải thu và thực thu. |

### 5.1. Hai service hỗ trợ

| Service | Phương thức | Phạm vi |
| --- | --- | --- |
| `TaiKhoanService` | `DangNhap()`, `KiemTraQuyen()`, `VoHieuHoa()` | Xác thực, vai trò và quyền trên đối tượng ở mọi UC; không chỉ ẩn nút. |
| `KhoSachService` | `TimCuon()`, `DanhDauMuon()`, `DanhDauTra()` | Hiện thực IQuanLyKhoSach; truy cập trạng thái cuốn theo giao dịch đã kiểm tra. Không tự bỏ qua hàng chờ hoặc phê duyệt. |

Tổng cộng 11 service đúng tên file: TheService, MuonTraService, GiaHanService, HangChoService, SuCoService, ThanhToanService, HoSoService, TaiKhoanService, SachService, KhoSachService, TraCuuThongKeService. Không tạo thêm service riêng chỉ để có một service cho mỗi UC.

### 5.2. Hợp đồng gọi chung

- Đầu vào nghiệp vụ theo từng UC, kèm ngữ cảnh tài khoản đã xác thực. Lệnh thay đổi nhận MaYeuCau; không tin vai trò hoặc mã chủ hồ sơ do người dùng tùy ý nhập.
- Ngày hiện tại lấy từ một nguồn thời gian dùng chung hoặc được truyền có kiểm soát khi kiểm thử. Người dùng không tự đổi ngày tất toán để giảm phạt.
- Kết quả gồm mã giao dịch/đối tượng, trạng thái và lỗi nghiệp vụ; service không in Console hoặc mở MessageBox. Kiểm tra thất bại dùng NghiepVuException hoặc kết quả lỗi thống nhất.
- Sau khi nhận thật sách/tiền mà lưu lỗi, giữ căn cứ để đối soát; rollback dữ liệu không phải trả lại sách/tiền trong thực tế.
- UC03 cập nhật phiếu, dòng, cuốn và quyền nhận nhất quán; UC05 gọi xét hàng chờ trước khi cuốn mở cho mượn tiếp; UC09 gọi HangChoService.ChuyenSangBanBoSung() giữ thứ tự và nguồn. UC10 đọc nghĩa vụ cả hai phân hệ trước xác nhận.
- Mã yêu cầu, nội dung và kết quả được lưu bền vững; cùng mã/cùng nội dung trả kết quả cũ, khác nội dung báo xung đột. Không chỉ lưu mã ở RAM hoặc riêng phiếu mượn/phiếu thu.

### 5.3. Đầu vào và đầu ra của các nhóm phương thức

Mọi lệnh thay đổi nhận thêm ngữ cảnh người thực hiện đã xác thực và mã yêu cầu. Ngữ cảnh do chương trình cấp, không lấy vai trò tự khai từ Form. Lỗi nghiệp vụ trả bằng NghiepVuException với mã/lý do; khi thất bại trước giao nhận, không trả đối tượng có vẻ đã lưu thành công.

| Nhóm phương thức | Dữ liệu nghiệp vụ đầu vào | Kết quả |
| --- | --- | --- |
| TheService.CapThe / CapLaiThe | Mã trường/độc giả, loại độc giả, thông tin định danh; cấp lại có mã thẻ và lý do | TheThuVien được cấp; cấp lại vẫn cùng độc giả, hạn cũ và lịch sử. |
| TheService.GiaHanThe / KhoiPhucThe | Mã thẻ; ngày xử lý từ nguồn thời gian | TheThuVien với hạn mới đúng nhánh. |
| MuonTraService.KiemTraDieuKienMuon | DocGia, thẻ, các cuốn, số nghĩa vụ hiện tại, các khoản chưa xong và quyền nhận hiện tại | Không lỗi khi toàn bộ điều kiện đạt; không thay đổi dữ liệu. |
| MuonTraService.TinhHanTra | DocGia, ngày mượn, lịch | DateOnly là hạn trả. |
| MuonTraService.MuonNhieuCuon | Mã độc giả; danh sách mã cuốn không trùng và mô tả tình trạng lúc giao từng cuốn | PhieuMuon cùng các ChiTietMuon đã lưu trong một giao dịch. |
| GiaHanService.GiaHanDongMuon | Mã dòng, số ngày 3/5/7, ngày xử lý | ChiTietMuon với hạn mới, DaGiaHan = true. |
| MuonTraService.TraTungCuon | Mã dòng, ngày nhận thật, tình trạng vật lý và mô tả đối chiếu | ChiTietMuon sau nhận trả; các khoản phát sinh được truy vấn theo MaDongMuon. |
| MuonTraService.TinhPhatQuaHan | ChiTietMuon, ngày chốt/tạm tính, lịch và chính sách phạt | decimal; truy vấn tạm tính không tự chốt hoặc ghi đã thu. |
| ThanhToanService.GhiNhanThucThu | Mã độc giả, khoản cần trả, số phân bổ, tiền thực nhận, mã giao dịch, hình thức | PhieuThu và các ChiTietThu; cập nhật khoản tương ứng nhất quán. |
| ThanhToanService.DoiSoatKhoan | Mã khoản, chứng từ/số tiền đã nhận và kết luận có căn cứ | KhoanThu cùng dấu vết đối soát; không tự hoàn tiền ngoài phạm vi. |
| HangChoService.DangKyCho / HuyDangKy / KetThucKhiMatQuyen | Mã độc giả, mã cuốn hoặc mã đăng ký; lý do kết thúc | DangKyCho ở trạng thái tương ứng, giữ lịch sử. |
| HangChoService.XetDotGiuCho | Mã cuốn, mã dòng nguồn, mã sự kiện cuốn sẵn sàng, thời điểm | DotGiuCho?; null khi không có ứng viên hợp lệ. |
| HangChoService.ChuyenLuot / XuLyHetLuot / XetLaiNguoiHai / TamDungDot | Mã đợt, mã lượt hoặc dòng nguồn mới; thời điểm và lý do/sự kiện | Đợt, lượt và đăng ký liên quan được cập nhật cùng nhau. |
| HangChoService.ChuyenSangBanBoSung | Mã cuốn cũ/mới, sự cố, danh sách đăng ký hiệu lực đã đồng ý tiếp tục | Các đăng ký chuyển cuốn, giữ thứ tự ban đầu và dòng nguồn; không chuyển người đã hủy. |
| SuCoService.BaoMat / GhiNhanHuHong | Mã dòng, ngày báo/nhận, loại/mức độ, minh chứng | BienBanSuCo; chưa có căn cứ giá vẫn có thể lưu chờ xử lý. |
| SuCoService.ChotBoiThuong | Mã sự cố, mức độ, giá và phê duyệt giá khi cần | BienBanSuCo với giá/tỷ lệ chốt và KhoanThu bồi thường khi phát sinh. |
| SuCoService.GhiNhanTimLai / TatToanMatSach | Mã sự cố, ngày thực nhận hoặc ngày xử lý, tình trạng và chứng từ liên quan | Trạng thái sự cố, dòng, cuốn, khoản thu đúng BR33–BR35; không tất toán thiếu nghĩa vụ. |
| SuCoService.PheDuyetBanThayThe / NhanBanThayThe | Mã sự cố; thông tin bản tương đương, người/căn cứ duyệt; nhận bản có mã cuốn mới | PheDuyet hoặc BienNhanThayThe, CuonSach mới và cập nhật phần bồi thường. |
| SachService.PheDuyetThanhLy / ThanhLyCuon | Mã cuốn, mã sự cố nếu có, lý do và phê duyệt | PheDuyet hoặc BienBanThanhLy; cuốn rời lưu thông và bỏ kệ hiện hành. |
| HoSoService.XacNhanNghiaVu / DongHoSo / HanCheKhiBuocRoiTruong | Mã độc giả, loại thao tác, căn cứ/lý do | Kết quả đối soát và trạng thái hồ sơ; lưu người xác nhận/thời điểm. |
| SachService.ThemDauSach / CapNhatDauSach / NhapCuon / ChuyenKe / QuanLyDanhMuc | Thông tin ấn bản/cuốn/tác giả/thể loại/NXB/kệ và mã đối tượng | Đối tượng hợp lệ đã lưu; QuanLyDanhMuc là nhóm thao tác, tách hàm theo loại dữ liệu khi cài đặt. |
| SachService.PheDuyetDinhGia | Mã cuốn, giá định lại, căn cứ, người có quyền | PheDuyet và giá trên cuốn; không đổi ngược giá sự cố đã chốt. |
| TraCuuThongKeService.TimSach / XemNghiaVu / ThongKe | Từ khóa, bộ lọc, mã độc giả trong phạm vi quyền, khoảng ngày | Danh sách/bảng kết quả chỉ đọc; số dự kiến, phải thu và thực thu tách riêng. |
| TaiKhoanService.DangNhap / KiemTraQuyen / VoHieuHoa | Thông tin đăng nhập hoặc tài khoản phiên; chức năng, đối tượng và lý do khi vô hiệu hóa | Phiên xác thực, kết quả kiểm tra hoặc trạng thái tài khoản; không trả mật khẩu/băm ra giao diện. |
| KhoSachService.TimCuon / DanhDauMuon / DanhDauTra | Mã cuốn, trạng thái/phiên bản hiện tại; tình trạng nhận khi trả | CuonSach? khi tìm; cập nhật khi nằm trong giao dịch nghiệp vụ hợp lệ. |

Các thao tác nhiều cuốn gọi lặp phương thức xử lý từng cuốn khi quy tắc cho phép trả từng phần; riêng MuonNhieuCuon phải kiểm tra toàn bộ trước khi ghi. Trả sách không bắt buộc gọi thu tiền thành công mới được lưu ngày nhận thật.


## 6. Ánh xạ quy tắc nghiệp vụ

| Quy tắc | Phân hệ | Phương thức thực thi | Điều kiện cần bảo đảm |
| --- | --- | --- | --- |
| BR01 | PH-A | `TheService.CapThe / CapLaiThe / KhoiPhucThe` | Xác minh sinh viên/giảng viên thuộc trường. |
| BR02 | PH-A | `TheService.CapLaiThe; TheThuVien.KhoaThe` | Một thẻ hiện hành, giữ hạn/nợ/lịch sử. |
| BR03 | PH-A | `TheService.GiaHanThe; TheThuVien.GiaHanThe` | Cửa sổ 30 ngày, cộng 6 tháng từ hạn cũ. |
| BR04 | PH-A | `TheService.KhoiPhucThe; TheThuVien.KhoiPhucThe` | Thẻ hết hạn, còn tư cách, không nợ; 6 tháng từ khôi phục. |
| BR05 | PH-A+B | `QuyTacMuon.KiemTraMuon / KiemTraUngVien` | Tổng nghĩa vụ mọi phiếu, gồm mất chưa tất toán; hạn mức 3/5. |
| BR06 | PH-A+B | `MuonTraService.TinhHanTra; TheService.GiaHanThe` | Hạn mượn không rút theo hạn thẻ; đổi thẻ không đổi hạn mượn. |
| BR07 | PH-A | `HoSoService.XacNhanNghiaVu` | Hết sách/nghĩa vụ và nợ mới xác nhận. |
| BR08 | PH-A+B | `HoSoService.HanCheKhiBuocRoiTruong; QuyTacMuon` | Chặn quyền mới, giữ hạn cũ và quyền trả/thanh toán. |
| BR09 | PH-A | `HoSoService.DongHoSo; TaiKhoanService.VoHieuHoa` | Chỉ đóng đủ điều kiện; giữ lịch sử theo Q04. |
| BR10 | PH-B | `MuonTraService.TinhHanTra; DocGia.SoNgayMuon` | 14/30 ngày mở cửa, không tính ngày giao. |
| BR11 | PH-B | `MuonTraService.MuonNhieuCuon; QuyTacMuon.KiemTraMuon` | Thẻ, quyền, nợ, hạn mức và quyền nhận từng cuốn. |
| BR12 | PH-A+B | `KhoSachService.TimCuon; MuonTraService.MuonNhieuCuon` | Kiểm tra từng bản, không dùng tổng số sách thay thế. |
| BR13 | PH-B | `MuonTraService.MuonNhieuCuon; CuonSach.DanhDauMuon` | Không giao trùng; dòng/cuốn lưu nhất quán. |
| BR14 | PH-B | `GiaHanService.GiaHanDongMuon; ChiTietMuon.GiaHanMotLan` | Một lần 3/5/7, chưa quá hạn, đủ quyền/thẻ/nợ/hàng chờ. |
| BR15 | PH-B | `ChiTietMuon.GiaHanMotLan; LichMoCua.CongNgayMoCua` | Cộng từ hạn cũ; chọn 3 ngày không cho thêm lần hai. |
| BR16 | PH-A+B | `MuonTraService.TraTungCuon; HangChoService.XetDotGiuCho` | Xét ưu tiên trước khi giao lại; không trả/mượn để vượt hàng. |
| BR17 | PH-B | `MuonTraService.TraTungCuon; ChiTietMuon.NhanTra; PhieuMuon.DaHoanTat` | Xử lý từng dòng, phiếu chỉ hoàn tất khi mọi nghĩa vụ đủ điều kiện. |
| BR18 | PH-B | `MuonTraService.MuonNhieuCuon / TraTungCuon; SuCoService.ChotBoiThuong` | Ghi tình trạng giao, đối chiếu nhận; không tính bồi thường hao mòn. |
| BR19 | PH-B | `ThanhToanService.GhiNhanThucThu / DoiSoatKhoan` | Thu đủ khoản chốt, tách nhận trả/thanh toán Q01. |
| BR20 | PH-B và hỗ trợ chung | `ThongBaoObserver.TaoNhacQuaHan` | Nhắc quá hạn một ngày mở cửa, không đổi hạn trả. |
| BR21 | PH-A | `HangChoService.DangKyCho` | Tối đa một đăng ký hiệu lực toàn thư viện, kể cả đồng thời. |
| BR22 | PH-A | `HangChoService.DangKyCho / HuyDangKy / XetDotGiuCho` | Thời điểm và thứ tự; hủy giữ thứ tự người khác. |
| BR23 | PH-A | `HangChoService.XetDotGiuCho; DotGiuCho.GhiNhanUngVien` | Chọn tối đa hai người đủ điều kiện. |
| BR24 | PH-A+B | `HangChoService.XetDotGiuCho / XetLaiNguoiHai` | 21/37 theo người mượn nguồn; sau giao dùng dòng mới. |
| BR25 | PH-A | `HangChoService.ChuyenLuot; LuotGiuCho.KichHoat` | Lượt tuần tự, thời hạn riêng theo Q03. |
| BR26 | PH-A | `HangChoService.XuLyHetLuot; DotGiuCho.KetThuc` | Không chọn người thứ ba hoặc mở đợt lách cùng lần sẵn sàng. |
| BR27 | PH-A | `HangChoService.KetThucKhiMatQuyen; HoSoService.HanCheKhiBuocRoiTruong` | Hủy đúng lý do; không đồng nhất khóa với hết hạn thẻ. |
| BR28 | PH-A+B | `HangChoService.TamDungDot / ChuyenSangBanBoSung; ThongBaoObserver` | Hỏng thì báo toàn hàng, chỉ chuyển người còn hiệu lực đã chọn tiếp tục. |
| BR29 | PH-B | `MuonTraService.TinhPhatQuaHan; PhatTheoNgay.TinhPhat` | 5.000 đồng/cuốn/ngày mở cửa, không trần; ngày chốt Q01/Q02. |
| BR30 | PH-A+B | `SuCoService.ChotBoiThuong; SachService.PheDuyetDinhGia; IChinhSachDinhGia` | 0/50/100%, giá có căn cứ và duyệt đúng quyền. |
| BR31 | PH-B | `SuCoService.PheDuyetBanThayThe / NhanBanThayThe` | Mất/hư nặng tiền hoặc bản tương đương; hư nhẹ chỉ tiền. |
| BR32 | PH-B | `ThanhToanService.GhiNhanThucThu; SuCoService.NhanBanThayThe` | Tách phạt/bồi thường; không thu tiền sách rồi nhận thêm bản cho cùng khoản. |
| BR33 | PH-B | `SuCoService.GhiNhanTimLai; MuonTraService.TraTungCuon` | Trước tất toán: nhận thật, hủy nghĩa vụ mất, đối soát chứng từ đã nhận. |
| BR34 | PH-B | `SuCoService.GhiNhanTimLai` | Sau tất toán: không hoàn bồi thường, cuốn cũ thuộc người đọc. |
| BR35 | PH-B | `SuCoService.TatToanMatSach; PhatTheoNgay.TinhPhat` | Báo mất không dừng phạt; khác ngày tính đến tất toán, trừ phần đã thu. |
| BR36 | PH-B | `SachService.ThanhLyCuon; SuCoService.NhanBanThayThe` | Hư nặng thanh lý; hư nhẹ sửa; bản mới mã mới. |

Kiểm tra quyền, dữ liệu, chống yêu cầu lặp và lưu nhất quán áp dụng cho mọi service theo G01–G06 và NFR01–NFR09. Bất biến một đối tượng đặt trong phương thức của thực thể; các quy tắc liên nhiều đối tượng thực thi trong service/QuyTacMuon. Không chỉ kiểm tra ở giao diện.