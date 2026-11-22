# Phân tích yêu cầu hệ thống quản lý thư viện

Tài liệu mô tả hoạt động của thư viện đại học, đối tượng sử dụng, phạm vi hệ thống, chức năng và quy tắc nghiệp vụ. Đây là cơ sở chung để xây dựng mô hình lớp, sơ đồ, chương trình C# và các ca kiểm thử. 

## 1. Bối cảnh và mục tiêu

### 1.1. Bối cảnh

Thư viện phục vụ sinh viên và giảng viên thuộc trường. Thủ thư quản lý thẻ, danh mục sách và hoạt động mượn trả. Người quản lý thư viện phê duyệt các trường hợp cần thẩm quyền như định giá sách, nhận bản thay thế, thanh lý và xác nhận hoàn tất nghĩa vụ.

Hệ thống quản lý sách vật lý theo từng cuốn. Các cuốn cùng một ấn bản có thể dùng chung thông tin đầu sách nhưng phải có mã, tình trạng và lịch sử lưu thông riêng. Hoạt động mượn, trả, gia hạn, xếp hàng và xử lý sự cố đều phải xác định đúng cuốn liên quan.

### 1.2. Mục tiêu

- Tập trung thông tin độc giả, thẻ, đầu sách, cuốn sách và giao dịch.
- Kiểm soát điều kiện mượn và quyền nhận sách, tránh giao trùng một cuốn hoặc vượt hạn mức.
- Theo dõi hạn trả, gia hạn, hàng chờ, tiền phạt và bồi thường theo cùng một bộ quy tắc.
- Bảo toàn lịch sử khi cấp lại thẻ, thay thế sách hoặc đóng hồ sơ độc giả.
- Phân quyền rõ ràng giữa độc giả, thủ thư và người quản lý.
- Cung cấp căn cứ để áp dụng lập trình hướng đối tượng và phân công công việc cho nhóm hai người.

## 2. Mô tả hoạt động thư viện

### 2.1. Tiếp nhận độc giả và quản lý thẻ

Thủ thư xác minh mã trường và tư cách sinh viên hoặc giảng viên trước khi cấp quyền sử dụng thư viện. Mỗi độc giả có một hồ sơ và tối đa một thẻ đang hoạt động. Khi cấp lại thẻ, hệ thống khóa thẻ cũ và giữ nguyên hồ sơ, hạn thẻ, công nợ và lịch sử.

Thẻ được gia hạn trong thời gian cho phép hoặc khôi phục khi đã hết hạn và độc giả vẫn thuộc đối tượng phục vụ. Việc thay đổi hiệu lực thẻ không tự thay đổi hạn trả của sách đã mượn.

### 2.2. Quản lý và tra cứu sách

Thư viện lưu thông tin chung của đầu sách và quản lý riêng từng cuốn, gồm mã cuốn, tình trạng, trạng thái lưu thông, giá trị và vị trí kệ. Độc giả tra cứu sách và xem khả năng mượn. Thủ thư cập nhật danh mục theo quyền được cấp.

Số lượng sách khả dụng phải phản ánh những cuốn thực sự có thể giao. Cuốn đang mượn, đang giữ cho người khác, bị mất, đang sửa chữa hoặc đã thanh lý không được coi là sẵn sàng cho mọi độc giả.

### 2.3. Mượn sách

Độc giả chọn sách và xuất trình thông tin nhận diện. Thủ thư kiểm tra tư cách sử dụng, hiệu lực thẻ, công nợ, số cuốn đang có nghĩa vụ và quyền nhận từng cuốn.

Nếu hợp lệ, hệ thống lập phiếu mượn và các dòng chi tiết, ghi tình trạng lúc giao, xác định hạn trả và cập nhật trạng thái cuốn. Một phiếu có thể gồm nhiều cuốn nhưng từng cuốn phải được theo dõi độc lập. Hạn mức được tính trên tất cả phiếu của độc giả.

### 2.4. Gia hạn mượn

Độc giả yêu cầu gia hạn cho một cuốn đang mượn hoặc nhờ thủ thư thực hiện theo quyền. Hệ thống kiểm tra cuốn chưa quá hạn, lượt mượn chưa được gia hạn, thẻ còn hạn, không có nợ và không có đăng ký chờ hiệu lực.

Gia hạn chỉ được thực hiện một lần trong mỗi lượt mượn, chọn thêm 3, 5 hoặc 7 ngày mở cửa tính từ hạn trả cũ.

### 2.5. Trả sách

Độc giả có thể trả một hoặc nhiều cuốn trong phiếu. Thủ thư kiểm tra sách thực nhận, đối chiếu tình trạng lúc giao, ghi ngày nhận và xác định khoản phạt hoặc bồi thường nếu có.

Mỗi dòng mượn được xử lý riêng. Sách tốt được xét quyền ưu tiên của hàng chờ trước khi cho mượn tiếp; sách hỏng được chuyển sang xử lý sự cố. Không cho phép trả rồi mượn lại ngay để bỏ qua người đang có quyền ưu tiên.

### 2.6. Đăng ký chờ và giữ chỗ

Độc giả có thể đăng ký chờ một cuốn cụ thể chưa sẵn sàng để mượn trực tiếp. Hàng chờ có thể có nhiều người, nhưng mỗi độc giả chỉ có một đăng ký hiệu lực trên toàn thư viện.

Khi cuốn đã được kiểm tra và có thể giao, thư viện xét một đợt giữ chỗ, chọn tối đa hai người đủ điều kiện theo thứ tự. Người thứ nhất có quyền nhận trước. Nếu hết lượt mà chưa nhận, người thứ hai được kiểm tra lại và có thời hạn nhận riêng.

Nếu người thứ nhất đã mượn thành công, người thứ hai không được đồng thời có quyền nhận cuốn đang cho mượn. Đăng ký của người thứ hai chỉ được tiếp tục chờ khi đáp ứng điều kiện kiểm tra lại. Nếu cả hai lượt đều không giao được, cuốn trở về trạng thái sẵn sàng; không chọn người thứ ba trong cùng đợt.

### 2.7. Mất, hỏng và bồi thường

Thủ thư lập biên bản khi sách bị báo mất hoặc có hư hỏng. Việc xác định trách nhiệm phải đối chiếu dấu vết có sẵn lúc giao; hao mòn thông thường không bị tính là hư hỏng phải bồi thường.

Khoản bồi thường được xác định theo mức độ và giá trị sách có căn cứ. Tiền phạt quá hạn và bồi thường là hai nghĩa vụ riêng. Với mất hoặc hư nặng, người đọc có thể nộp tiền hoặc bản sách thay thế được phê duyệt. Bản thay thế có mã mới và giữ liên kết với sự cố, cuốn cũ và hàng chờ có liên quan.

Việc báo mất không tự kết thúc lượt mượn, giải phóng hạn mức hoặc dừng tiền phạt. Trường hợp tìm lại sách phải phân biệt trước và sau khi tất toán nghĩa vụ mất sách theo Q02; hoàn tất riêng phần bồi thường chưa đồng nghĩa đã tất toán.

### 2.8. Kết thúc sử dụng thư viện

Khi độc giả rời trường hoặc yêu cầu đóng hồ sơ, thư viện đối soát sách và công nợ. Chỉ xác nhận hoàn tất nghĩa vụ khi không còn nghĩa vụ sách và khoản phải thanh toán.

Nếu độc giả bị buộc rời trường, hệ thống dừng quyền sử dụng mới nhưng giữ hạn trả và nghĩa vụ đã phát sinh. Đóng hồ sơ không xóa lịch sử mượn trả, sự cố hoặc thu tiền.

## 3. Đối tượng sử dụng và phân quyền

### 3.1. Các nhóm người sử dụng

| Đối tượng | Nhu cầu và quyền chính | Giới hạn |
| --- | --- | --- |
| Sinh viên | Tra cứu sách, xem hồ sơ và nghĩa vụ của mình, yêu cầu gia hạn mượn, đăng ký hoặc hủy chờ | Tối đa 3 cuốn; thời hạn mượn 14 ngày mở cửa; không tự xác nhận giao sách hoặc thu tiền |
| Giảng viên | Các chức năng dành cho độc giả, với chính sách mượn của giảng viên | Tối đa 5 cuốn; thời hạn mượn 30 ngày mở cửa; không xem trái phép hồ sơ người khác |
| Thủ thư | Xác minh độc giả, xử lý thẻ, cập nhật danh mục, giao nhận sách, xử lý hàng chờ, lập biên bản và ghi nhận thu tiền | Chỉ thực hiện trong phạm vi quyền được cấp; không tự phê duyệt việc thuộc thẩm quyền quản lý |
| Người quản lý thư viện | Duyệt định giá, bản thay thế, thanh lý; xác nhận nghĩa vụ và đóng hồ sơ; xem thống kê theo quyền | Không được bỏ qua quy tắc nghiệp vụ hoặc xóa lịch sử để làm mất nghĩa vụ |

Sinh viên và giảng viên được gọi chung là **độc giả**. Tài khoản nhân viên không mặc nhiên là hồ sơ độc giả.

### 3.2. Thành phần phối hợp

- **Nguồn dữ liệu của nhà trường:** cung cấp căn cứ xác minh mã trường, loại độc giả và tư cách thuộc trường. Khi chưa tích hợp hệ thống thật, dữ liệu mô phỏng phải được ghi rõ là dữ liệu dùng cho bài tập.
- **Tác vụ nội bộ:** kiểm tra quá hạn, hết lượt giữ chỗ và xử lý thông báo. Đây là hoạt động của hệ thống, không phải một nhóm người sử dụng.

### 3.3. Nguyên tắc phân quyền

- Độc giả chỉ xem và thao tác trên hồ sơ, lượt mượn, hàng chờ và nghĩa vụ của mình.
- Giao sách, xác nhận nhận trả và ghi nhận tiền thực thu do nhân viên có quyền thực hiện.
- Các thao tác phê duyệt phải lưu người duyệt và căn cứ liên quan.
- Kiểm tra quyền phải được thực hiện khi xử lý nghiệp vụ, không chỉ bằng cách ẩn nút trên giao diện.

## 4. Phạm vi hệ thống

### 4.1. Trong phạm vi

- Thư viện đại học phục vụ sinh viên và giảng viên thuộc trường.
- Quản lý sách vật lý theo đầu sách và từng cuốn.
- Quản lý hồ sơ độc giả, tài khoản, hiệu lực và lịch sử thẻ.
- Mượn, trả từng cuốn và gia hạn mượn.
- Hàng chờ theo cuốn, đợt giữ chỗ và lượt nhận sách.
- Phạt quá hạn, xử lý mất/hỏng, bồi thường và bản thay thế.
- Ghi nhận tiền thực thu, nghĩa vụ chưa hoàn tất và lịch sử thao tác.
- Thông báo trong hệ thống, tra cứu và thống kê cơ bản.
- Xác nhận nghĩa vụ, dừng quyền sử dụng và đóng hồ sơ.

### 4.2. Ngoài phạm vi phiên bản bài tập

- Sách điện tử, bản quyền số và đọc sách trực tuyến.
- Mượn liên thư viện, vận chuyển và giao nhận tại nhà.
- Mua sắm, nhà cung cấp, kế toán tổng hợp hoặc quản lý ngân sách thư viện.
- Cổng thanh toán trực tuyến, gửi email hoặc SMS thật; có thể xem xét như phần mở rộng.
- Tự động kết nối dữ liệu nhà trường khi chưa có hệ thống hoặc quyền truy cập phù hợp.
- Ra quyết định tốt nghiệp, thôi học, nghỉ việc; hệ thống chỉ xác nhận nghĩa vụ với thư viện.

### 4.3. Giới hạn triển khai

Chương trình dự kiến dùng C# và phát triển bằng VS Code. Loại giao diện, công nghệ lưu trữ và phiên bản môi trường được xác định ở bước thiết kế, khởi tạo dự án. 

## 5. Yêu cầu chức năng

Danh sách gồm **12 Use Case**: 

| Mã | Chức năng | Tác nhân chính | Yêu cầu xử lý và kết quả |
| --- | --- | --- | --- |
| UC01 | Cấp mới và cấp lại thẻ | Thủ thư | Xác minh đối tượng; cấp thẻ mới hoặc thay thẻ cũ; bảo đảm tối đa một thẻ hoạt động, giữ hồ sơ và lịch sử |
| UC02 | Gia hạn và khôi phục thẻ | Thủ thư | Kiểm tra tư cách, công nợ và thời điểm; xác định hạn thẻ mới đúng trường hợp |
| UC03 | Mượn sách | Thủ thư | Kiểm tra thẻ, quyền, nợ, hạn mức và từng cuốn; tạo phiếu, dòng mượn, hạn trả và tình trạng lúc giao |
| UC04 | Gia hạn mượn | Độc giả hoặc thủ thư được phép | Kiểm tra điều kiện; gia hạn từng cuốn một lần, thêm 3/5/7 ngày mở cửa từ hạn cũ |
| UC05 | Trả sách và thu tiền | Thủ thư | Nhận từng cuốn, tính phạt, kiểm tra hỏng, ghi nhận khoản phải thu và tiền thực thu; cập nhật dòng mượn và khả dụng |
| UC06 | Đăng ký và hủy hàng chờ | Độc giả hoặc thủ thư được phép | Tạo đăng ký theo cuốn; chặn đăng ký hiệu lực thứ hai; hủy có lưu lịch sử và bảo toàn thứ tự |
| UC07 | Xét và chuyển lượt giữ chỗ | Thủ thư, tác vụ nội bộ | Chọn tối đa hai người hợp lệ; cấp quyền nhận tuần tự, xử lý hết lượt và kiểm tra lại điều kiện |
| UC08 | Xử lý mất, hỏng và bồi thường | Thủ thư, người quản lý | Lập biên bản, xác định giá và mức độ; tách phạt/bồi thường; xử lý tìm lại trước hoặc sau tất toán nghĩa vụ mất sách theo Q02 |
| UC09 | Thanh lý và nhập bản thay thế | Người quản lý, thủ thư | Duyệt phương án, xử lý cuốn cũ, nhận bản mới có mã riêng; giữ lịch sử và liên kết hàng chờ hợp lệ |
| UC10 | Xác nhận nghĩa vụ và đóng hồ sơ | Người quản lý | Đối soát sách/nợ; dừng quyền hoặc đóng hồ sơ đúng điều kiện; giữ giao dịch và nhật ký |
| UC11 | Quản lý đầu sách, cuốn sách và vị trí | Thủ thư, người quản lý theo quyền | Thêm/sửa thông tin ấn bản, nhập từng cuốn, quản lý vị trí; kiểm tra mã trùng và dữ liệu không hợp lệ |
| UC12 | Tra cứu và thống kê | Độc giả, thủ thư, người quản lý | Tìm theo tên/tác giả/mã; xem khả dụng, lịch sử và nghĩa vụ theo quyền; thống kê đang mượn, quá hạn, chưa thu |

Các hoạt động dùng chung gồm xác thực tài khoản, kiểm tra quyền, lưu lịch sử và thông báo lỗi rõ ràng. 

### 5.1. Đầu ra cần có

- Hồ sơ độc giả và thông tin hiệu lực thẻ.
- Danh mục đầu sách, danh sách cuốn và khả dụng của từng cuốn.
- Phiếu mượn với hạn trả và tình trạng lúc giao của từng dòng.
- Thông tin nhận trả, gia hạn và lịch sử mượn.
- Danh sách chờ, quyền nhận hiện tại và thời hạn của lượt giữ chỗ.
- Biên bản mất/hỏng, khoản phạt, bồi thường, phiếu thu hoặc biên nhận thay thế.
- Thông báo nhắc trả, quyền nhận hoặc sự cố ảnh hưởng hàng chờ.
- Kết quả tra cứu, thống kê và xác nhận nghĩa vụ theo quyền.

## 6. Quy tắc nghiệp vụ

### 6.1. Thuật ngữ và cách tính thời gian

| Thuật ngữ | Ý nghĩa |
| --- | --- |
| Đầu sách | Thông tin chung của một ấn bản; không đại diện cho một bản vật lý duy nhất |
| Cuốn sách | Bản vật lý có mã và lịch sử riêng |
| Dòng mượn | Một cuốn trong một lượt mượn, có hạn trả và trạng thái xử lý riêng |
| Nghĩa vụ sách chưa hoàn tất | Nghĩa vụ của một dòng mượn chưa được kết thúc theo quy trình trả hoặc xử lý mất; báo mất chưa đủ để giải phóng hạn mức |
| Đăng ký chờ | Yêu cầu của độc giả chờ một cuốn cụ thể |
| Đợt giữ chỗ | Một lần xét tối đa hai người đủ điều kiện khi cuốn có thể giao |
| Lượt giữ chỗ | Quyền nhận có thời hạn của một người trong đợt |
| Dòng mượn nguồn | Lượt mượn được dùng làm căn cứ xác định mốc kiểm tra thẻ của người chờ |
| Ngày mở cửa | Mặc định thứ Hai đến thứ Bảy; không tính Chủ nhật và ngày thư viện đóng cửa theo lịch được xác nhận |

Khi cộng ngày mở cửa, không tính ngày bắt đầu. Thời hạn thẻ 6 tháng và cửa sổ gia hạn thẻ 30 ngày được tính theo lịch, không đổi thành ngày mở cửa. Thời hạn mượn, gia hạn mượn, mốc 21/37 ngày và ngày phạt được tính theo lịch mở cửa.

### 6.2. Độc giả và thẻ

| Mã | Quy tắc |
| --- | --- |
| BR01 | Chỉ sinh viên và giảng viên thuộc trường được cấp quyền mượn. Phải xác minh mã trường trước khi cấp mới, cấp lại hoặc khôi phục thẻ. |
| BR02 | Mỗi độc giả chỉ có một thẻ hoạt động. Khi cấp lại, khóa thẻ cũ, giữ hạn thẻ, hồ sơ, nợ, lịch sử và số lần gia hạn của các lượt mượn. |
| BR03 | Thẻ có hiệu lực 6 tháng. Chỉ gia hạn trong 30 ngày cuối khi còn hạn; cộng 6 tháng từ hạn cũ, kiểm tra theo hạn mới để tránh gia hạn lặp trong cùng cửa sổ. |
| BR04 | Thẻ hết hạn được khôi phục khi độc giả còn đúng đối tượng và đáp ứng điều kiện. Hiệu lực mới là 6 tháng từ ngày khôi phục; không phạt chỉ vì thẻ hết hạn. |
| BR05 | Sinh viên tối đa 3 cuốn, giảng viên tối đa 5 cuốn chưa hoàn tất nghĩa vụ sách. Tính trên tất cả phiếu, không tính theo số đầu sách. |
| BR06 | Thẻ sắp hết hạn vẫn được mượn đủ thời gian chính sách. Hạn trả có thể sau hạn thẻ; thẻ hết hạn không tự thay đổi hạn trả đã giao. |
| BR07 | Độc giả chủ động rời trường, tốt nghiệp hoặc nghỉ việc phải trả hết sách và thanh toán đủ trước khi được xác nhận hoàn tất nghĩa vụ thư viện. |
| BR08 | Độc giả bị buộc thôi học hoặc đuổi việc giữ hạn trả cũ nhưng bị dừng mượn, gia hạn và xếp hàng mới. Vẫn được thông báo, trả sách và xử lý công nợ. |
| BR09 | Chỉ hủy thẻ và đóng hồ sơ khi hết nghĩa vụ sách và nợ. Vô hiệu hóa tài khoản nhưng giữ lịch sử; việc ẩn danh chỉ thực hiện theo chính sách lưu trữ được phê duyệt. |

### 6.3. Mượn, gia hạn và trả

| Mã | Quy tắc |
| --- | --- |
| BR10 | Sinh viên được mượn tối đa 14 ngày mở cửa, giảng viên 30 ngày mở cửa. Hạn trả được chốt khi giao sách. |
| BR11 | Muốn mượn phải có thẻ còn hạn, còn quyền sử dụng, không còn khoản phải thu chưa thanh toán và không vượt hạn mức. Cuốn phải sẵn sàng hoặc đang giữ đúng cho người nhận. |
| BR12 | Phải có cuốn khả dụng và bản được chọn phải đủ điều kiện giao. Không dùng tổng số lượng thay cho kiểm tra từng cuốn. |
| BR13 | 	Một cuốn chỉ có tối đa một lượt đang chiếm quyền mượn tại một thời điểm. Dòng đã nhận trả nhưng còn nghĩa vụ tài chính được theo dõi riêng theo Q01, không tạo quyền giữ cuốn cho người mượn cũ. Việc ghi nhận dòng mượn và cập nhật trạng thái cuốn phải nhất quán. |
| BR14 | Gia hạn theo từng cuốn, tối đa một lần trong mỗi lượt mượn, chọn 3/5/7 ngày. Cuốn chưa quá hạn, thẻ còn hạn, không nợ và không có đăng ký chờ hiệu lực. |
| BR15 | Ngày gia hạn được cộng từ hạn trả cũ theo ngày mở cửa, không tính ngày hạn cũ. Chọn 3 ngày không cho phép gia hạn thêm lần nữa. |
| BR16 | Không cho trả rồi mượn lại ngay để vượt hàng chờ. Phải xử lý quyền ưu tiên trước khi giao lại. |
| BR17 | Cho trả một hoặc nhiều cuốn trong phiếu. Từng dòng có ngày trả, tình trạng và phạt riêng; phiếu chỉ hoàn tất khi mọi dòng đã xử lý xong nghĩa vụ. |
| BR18 | Phải ghi dấu vết có sẵn lúc giao và đối chiếu khi nhận. Hao mòn thông thường không bị coi là hư hỏng phải bồi thường. |
| BR19 | Khi trả sách, thư viện yêu cầu thanh toán đủ khoản phát sinh, không thanh toán một phần. Ngoại lệ chưa hoàn tất thu tiền sau tiếp nhận vật lý được xử lý theo Q01; không ghi đã thu khi chưa xác nhận tiền thực nhận. |
| BR20 | Khi quá hạn một ngày, gửi thông báo nhắc trả. Thông báo không thay đổi hạn trả và không thay thế nghĩa vụ tài chính. |

### 6.4. Hàng chờ và giữ chỗ

| Mã | Quy tắc |
| --- | --- |
| BR21 | Hàng chờ gắn mã cuốn cụ thể và có thể có nhiều người. Mỗi độc giả chỉ một đăng ký hiệu lực trên toàn thư viện. |
| BR22 | Xếp theo thời điểm đăng ký; dùng thứ tự tăng để phân xử các đăng ký cùng thời điểm. Hủy không thay đổi thứ tự tương đối của những người còn lại. |
| BR23 | Khi xét đợt, duyệt từ trên xuống và chọn tối đa hai người đủ quyền, không nợ, còn hạn mức và đạt điều kiện hạn thẻ. |
| BR24 | Mốc kiểm tra thẻ bằng ngày bắt đầu dòng mượn nguồn cộng 21 ngày mở cửa nếu người mượn nguồn là sinh viên, hoặc 37 ngày nếu là giảng viên. Thẻ người chờ phải còn hiệu lực đến mốc này. Khi người thứ nhất mượn thành công, kiểm tra lại người thứ hai theo dòng mượn mới. |
| BR25 | Người thứ nhất nhận trước; quyền nhận kết thúc vào giờ đóng cửa ngày mở cửa thứ hai sau ngày cấp quyền khi sách đã được kiểm tra và có thể giao. Người thứ hai được thời hạn riêng khi thực sự chuyển quyền. |
| BR26 | Nếu hết hai lượt vẫn không giao được, đưa cuốn về sẵn sàng cho người đủ điều kiện; không chọn người thứ ba trong cùng đợt. |
| BR27 | Độc giả bị khóa theo nghiệp vụ hoặc mất quyền do rời trường được hủy đăng ký phù hợp. Không đồng nhất khóa tài khoản với việc thẻ tạm hết hạn. |
| BR28 | Nếu sách trả bị hỏng, thông báo toàn hàng chờ và tạm dừng đợt. Người đọc chọn tiếp tục chờ bản bổ sung hoặc hủy; không chạy thời hạn nhận khi chưa có sách dùng được. |

Quy ước xử lý: đăng ký bị kết thúc vì thiếu điều kiện hoặc hết lượt phải lưu lý do và không tự phục hồi ở đợt sau. Những đăng ký chưa đến lượt, hoặc người thứ hai được tiếp tục chờ sau kiểm tra lại hợp lệ, được giữ theo trạng thái thực tế. Người thứ hai không được kích hoạt quyền nhận khi người thứ nhất đang mượn cuốn đó.

### 6.5. Phạt, bồi thường và thay thế

| Mã | Quy tắc |
| --- | --- |
| BR29 | Phạt 5.000 đồng mỗi cuốn cho mỗi ngày mở cửa quá hạn, không có trần. Không tính ngày thư viện đóng cửa. |
| BR30 | Hao mòn thông thường: 0%; hư nhẹ sửa được: 50%; hư nặng không dùng được: 100% giá trị. Căn cứ là giá bìa; nếu thiếu thì dùng định giá được duyệt và lưu lý do. |
| BR31 | Mất hoặc hư nặng: bồi thường tiền hoặc bản thay thế được duyệt, đúng tên, tác giả, ngôn ngữ và phiên bản tương đương được chấp thuận. Hư nhẹ chỉ thu tiền. |
| BR32 | Phạt quá hạn độc lập với bồi thường. Không thu đủ tiền sách rồi tiếp tục thu bản thay thế cho cùng nghĩa vụ. |
| BR33 | Tìm lại trước tất toán nghĩa vụ mất sách theo Q02: nhận sách, hủy nghĩa vụ bồi thường mất và tính phạt đến ngày trả thực tế; nếu hỏng thì xử lý theo mức độ hỏng. |
| BR34 | Tìm lại sau tất toán nghĩa vụ mất sách theo Q02: không hoàn tiền hoặc bản đã nộp; cuốn tìm lại thuộc người đọc. Hồ sơ cuốn cũ vẫn lưu và không được đưa lại vào lưu thông. |
| BR35 | Báo mất không dừng phạt. Nếu quá hạn, phạt tiếp tục theo mốc tất toán đã thống nhất; nếu tiền phạt và bồi thường hoàn tất khác thời điểm, tính phạt đến ngày tất toán theo Q02 và khấu trừ phần đã thu. |
| BR36 | Cuốn hư nặng được thanh lý và rời vị trí lưu thông; bản thay thế có mã mới, có thể dùng kệ cũ nếu phù hợp. Hư nhẹ chuyển sửa chữa, không tự thanh lý. |

Tiền phạt cho một cuốn được tính bằng **5.000 đồng × số ngày mở cửa sau hạn trả đến hết ngày chốt**. Với cuốn trả thực tế, ngày chốt để tính phạt là ngày thư viện thực nhận; không thay ngày này bằng ngày thu tiền muộn hơn. Với cuốn mất chưa trả, ngày chốt là ngày tất toán theo Q02; khi chưa tất toán, số phạt tính đến ngày tra cứu chỉ là số tạm tính tại thời điểm đó.

### 6.6. Điều kiện áp dụng chung

- Còn nợ thì chặn mượn, gia hạn mượn, gia hạn hoặc khôi phục thẻ và đăng ký chờ mới.
- Vẫn cho phép trả sách, báo mất, hủy chờ, xem và thanh toán nghĩa vụ khi bị chặn quyền sử dụng mới.
- Cấp lại thẻ không tạo độc giả thứ hai hoặc xóa nợ.
- Cuốn thay thế có mã mới; chỉ chuyển đăng ký còn hiệu lực của người đã chọn tiếp tục, giữ thứ tự và liên kết lịch sử.
- Chưa có dòng mượn nguồn thì không tự suy diễn mốc 21/37 ngày; cuốn đang sẵn sàng được xử lý theo luồng mượn trực tiếp.

## 7. Yêu cầu chất lượng và toàn vẹn dữ liệu

| Mã | Yêu cầu |
| --- | --- |
| NFR01 | Mã định danh phải phân biệt được từng độc giả, thẻ, cuốn, giao dịch và chứng từ; không tái sử dụng mã cuốn cũ cho bản thay thế. |
| NFR02 | Lỗi khi xử lý giao dịch không được để lại phiếu dở, cuốn bị mượn trùng hoặc số nghĩa vụ sai. Hai yêu cầu đồng thời không được vượt hạn mức hoặc cùng nhận một cuốn. |
| NFR03 | Gửi lại cùng một yêu cầu đã thành công không được tạo thêm phiếu, cộng hạn hoặc thu tiền lần nữa. |
| NFR04 | Mọi thay đổi quan trọng phải truy vết được người thực hiện, thời điểm, đối tượng và lý do; không xóa lịch sử để che mất nghĩa vụ. |
| NFR05 | Kiểm tra dữ liệu đầu vào và quyền sử dụng; không lưu mật khẩu dạng rõ hoặc đưa mật khẩu vào nhật ký. |
| NFR06 | Thông báo lỗi phải nêu nguyên nhân phù hợp, không báo thành công khi thao tác chưa hoàn tất. Lỗi gửi thông báo không làm mất giao dịch đã lưu thành công. |
| NFR07 | Dữ liệu phải được lưu và đọc lại giữa các lần chạy; bảo toàn liên kết giữa hồ sơ, cuốn, phiếu, sự cố và khoản thu. |
| NFR08 | Dùng thống nhất lịch mở cửa và cách tính thời gian; dữ liệu kiểm thử có ngày cố định để kiểm tra được kết quả. |
| NFR09 | Chương trình phải có hướng dẫn cài đặt, chạy và kiểm thử; tránh đường dẫn chỉ tồn tại trên máy của một thành viên. |

## 8. Quy ước xử lý nghiệp vụ

Nhóm thống nhất các quy ước dưới đây để áp dụng đồng bộ trong thiết kế, lập trình và kiểm thử.

### Q01 — Đã nhận sách nhưng chưa hoàn tất thu tiền

- Khi thủ thư đã nhận sách vật lý, hệ thống ghi nhận ngày nhận thực tế và tình trạng sách.
- Tiền phạt quá hạn của cuốn được tính đến hết ngày nhận thực tế. Không tiếp tục tính thêm phạt quá hạn cho cuốn đó chỉ vì việc thanh toán chưa hoàn tất.
- Nếu chưa thu được tiền hoặc chưa xác nhận được kết quả thu, khoản phải thanh toán vẫn được giữ ở trạng thái chưa hoàn tất.
- Không đánh dấu khoản đã thanh toán và không tạo chứng từ thu thành công khi chưa xác nhận tiền thực nhận.
- Độc giả còn nợ bị chặn mượn mới, gia hạn mượn, gia hạn hoặc khôi phục thẻ và đăng ký chờ mới. Vẫn được trả sách, báo mất, hủy chờ, xem và thanh toán nghĩa vụ.
- Nếu đã nhận tiền nhưng thao tác lưu bị lỗi, thủ thư phải đối soát trước khi yêu cầu thanh toán lại, tránh thu hai lần.
- Trạng thái nhận trả sách và trạng thái thanh toán được theo dõi riêng. Không coi việc hoàn tác dữ liệu là trả lại sách vật lý cho độc giả.
- Nếu hệ thống chưa lưu được thông tin tiếp nhận, thủ thư phải giữ bằng chứng giao nhận để đối soát và cập nhật lại đúng ngày nhận thực tế.

### Q02 — Tất toán nghĩa vụ mất sách

- Báo mất không tự dừng tiền phạt và không tự giải phóng hạn mức mượn.
- Nghĩa vụ mất sách chỉ hoàn tất khi đồng thời đáp ứng:
  1. Đã bồi thường đủ bằng tiền hoặc đã được xác nhận nhận bản sách thay thế được duyệt.
  2. Đã thanh toán đủ tiền phạt quá hạn tính đến ngày tất toán.
- Ngày tất toán là ngày cả hai điều kiện trên được đáp ứng.
- Nếu bồi thường và thanh toán phạt diễn ra khác ngày, tiền phạt tiếp tục được tính theo ngày mở cửa đến ngày tất toán.
- Khoản phạt đã thu trước đó được ghi nhận và khấu trừ khi xác định phần còn phải thanh toán; không thu lại phần đã thu.
- Phạt quá hạn và bồi thường là hai nghĩa vụ riêng. Không chấp nhận trả góp một khoản đã chốt; việc hoàn tất hai nghĩa vụ ở hai thời điểm khác nhau không làm chúng trở thành một khoản chung.
- Chỉ khi tất toán mới kết thúc nghĩa vụ sách của dòng mượn mất và loại cuốn đó khỏi số cuốn tính vào hạn mức.
- Nếu tìm lại sách trước khi tất toán, áp dụng BR33 và đối soát các khoản đã nhận trước khi điều chỉnh nghĩa vụ.
- Nếu tìm lại sau khi tất toán, áp dụng BR34: không hoàn tiền hoặc bản thay thế đã nộp; cuốn tìm lại thuộc người đọc.

### Q03 — Lịch mở cửa và thời hạn xử lý

- Trong phạm vi bài tập, thư viện mở cửa từ thứ Hai đến thứ Bảy và đóng cửa lúc 17:00 theo giờ Việt Nam.
- Chủ nhật là ngày nghỉ.
- Ngày nghỉ đặc biệt được khai báo trong lịch thư viện. Nếu không có ngày nghỉ đặc biệt được khai báo, áp dụng lịch mặc định trên.
- Khi cộng ngày mở cửa, không tính ngày bắt đầu; bỏ qua Chủ nhật và ngày nghỉ đặc biệt.
- Hạn nhận sách giữ chỗ kết thúc lúc 17:00 của ngày mở cửa thứ hai sau ngày cấp quyền nhận.
- Người thứ hai có thời hạn nhận riêng, tính từ ngày được chuyển quyền, không dùng lại thời hạn của người thứ nhất.
- Hiệu lực thẻ 6 tháng và cửa sổ gia hạn thẻ 30 ngày được tính theo lịch, không tính theo ngày mở cửa.
- Lịch thay đổi được áp dụng cho các phép tính mới; không tự sửa hạn trả hoặc hạn nhận đã chốt. Tiền phạt phải loại trừ những ngày thư viện thực tế đóng cửa theo lịch đã xác nhận.

### Q04 — Lưu trữ hồ sơ và lịch sử

- Trong phạm vi bài tập, hệ thống không tự động xóa hoặc ẩn danh hồ sơ.
- Khi đóng hồ sơ, hệ thống vô hiệu hóa quyền sử dụng, thẻ và tài khoản theo quy trình.
- Giữ lại lịch sử mượn trả, gia hạn, hàng chờ, sự cố, khoản thu và chứng từ liên quan.
- Việc đóng hồ sơ không làm mất hoặc thay đổi nghĩa vụ đã phát sinh.
- Chỉ người có quyền mới được truy cập thông tin lưu trữ.
- Chức năng xóa hoặc ẩn danh theo thời hạn lưu trữ nằm ngoài phạm vi phiên bản bài tập.

### 8.1. Tìm lại sách trước tất toán khi đã nhận bồi thường

Mốc phân nhánh của BR33–BR34 là ngày tất toán nghĩa vụ mất sách theo Q02. Hoàn tất riêng phần bồi thường chưa đồng nghĩa đã tất toán.

Nếu đã nhận tiền sách hoặc bản thay thế nhưng còn tiền phạt, sau đó tìm lại cuốn cũ: nhận cuốn cũ qua UC05, dừng phạt tại ngày thực nhận, hủy nghĩa vụ bồi thường mất theo BR33 và đối soát các chứng từ đã nhận. Nếu sách hỏng thì đánh giá nghĩa vụ hỏng theo tình trạng thực tế.

Không xóa phiếu thu/biên nhận và không tự hoàn tiền hoặc trả bản thay thế khi chưa có quyết định của người quản lý. Kết quả đối soát phải lưu căn cứ, người xử lý và thời điểm. Khoản đang chờ đối soát chưa được coi là đã giải quyết toàn bộ; hoàn tiền tự động không thuộc phạm vi phiên bản bài tập.

### 8.2. Phân biệt lượt mượn, nghĩa vụ sách và công nợ

Ngày nhận sách vật lý được lưu độc lập với ngày thanh toán. Cuốn đã nhận phải qua kiểm tra tình trạng và xét ưu tiên trước khi được giao tiếp.

BR13 ngăn hai lượt cùng chiếm quyền mượn một cuốn; dòng lịch sử đã nhận sách nhưng còn công nợ không đồng nghĩa cuốn vẫn đang ở người mượn cũ.

Điều kiện kết thúc nghĩa vụ của dòng và phiếu vẫn tuân BR17/Q01; không dùng trạng thái khả dụng của cuốn để xóa công nợ. Riêng sách mất chỉ được giải phóng hạn mức khi tất toán Q02.

## 9. Phân công và sử dụng tài liệu

| Thành viên | Use Case chịu trách nhiệm chính |
| --- | --- |
| Trần Thị Hà Lan | UC01, UC02, UC06, UC07, UC10, UC11 |
| Hoàng Văn Quân | UC03, UC04, UC05, UC08, UC09, UC12 |
| Cả hai | Rà soát quy tắc, thống nhất dữ liệu và hợp đồng phối hợp, tích hợp và kiểm thử các luồng giao nhau. |

## 10. Phạm vi hai phân hệ

| Phân hệ | Phạm vi chức năng | Use Case | Phụ trách |
| --- | --- | --- | --- |
| PH-A — Độc giả, danh mục và hàng chờ | Thẻ, hồ sơ, danh mục sách, đăng ký chờ và quyền nhận sách | UC01, UC02, UC06, UC07, UC10, UC11 | Trần Thị Hà Lan |
| PH-B — Lưu thông, sự cố và thống kê | Mượn, gia hạn, trả, thu tiền, mất/hỏng, thanh lý, nhận bản thay thế và tra cứu/thống kê | UC03, UC04, UC05, UC08, UC09, UC12 | Hoàng Văn Quân |

Hai phân hệ cùng sử dụng dữ liệu độc giả, thẻ, cuốn, phiếu, hàng chờ, khoản thu và lịch sử. Không tạo hai hồ sơ độc giả hoặc hai bản ghi cuốn độc lập cho từng phân hệ.

Mượn/trả và hàng chờ phải phối hợp để bảo toàn quyền nhận; đóng hồ sơ phải đối soát nghĩa vụ của cả hai phân hệ. Một service có thể hỗ trợ nhiều Use Case; phân công UC không buộc mọi file phải thuộc riêng một phân hệ.