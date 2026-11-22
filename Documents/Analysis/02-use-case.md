# Đặc tả Use Case hệ thống quản lý thư viện

Tài liệu đặc tả 12 Use Case theo mã và phạm vi đã nêu trong tài liệu yêu cầu. Mỗi Use Case có tác nhân, điều kiện trước, dữ liệu đầu vào, luồng chính, luồng thay thế, trường hợp lỗi và kết quả sau thực hiện. Các mã BR, NFR và Q tham chiếu tài liệu yêu cầu, không tạo lại một bộ quy tắc khác.

## 1. Quy ước chung

### 1.1. Tác nhân và cách đọc

- **Độc giả:** sinh viên hoặc giảng viên; chỉ thao tác trên dữ liệu của mình.
- **Thủ thư:** nhân viên được cấp quyền thực hiện giao dịch tại thư viện.
- **Người quản lý:** người có quyền phê duyệt định giá, thay thế, thanh lý và đóng hồ sơ.
- **Nguồn dữ liệu nhà trường:** thành phần hỗ trợ xác minh tư cách độc giả; nếu mô phỏng thì phải ghi rõ là dữ liệu bài tập.
- **Tác vụ nội bộ:** xử lý hết lượt, nhắc quá hạn và thông báo; là cơ chế kích hoạt của hệ thống, không phải một người sử dụng mới.

Tác nhân là người thực hiện nghiệp vụ; người phụ trách là thành viên nhóm lập trình chức năng. Hai khái niệm này không thay thế cho nhau.

Luồng chính mô tả trường hợp thành công thông thường. Luồng thay thế là lựa chọn hoặc tình huống nghiệp vụ hợp lệ. Trường hợp lỗi phải nêu cách xử lý và dữ liệu được giữ lại. Điều kiện nghiệp vụ được kiểm tra lại tại bước xử lý để tránh sử dụng dữ liệu đã thay đổi.

### 1.2. Yêu cầu áp dụng cho mọi Use Case

| Mã | Tình huống | Cách xử lý chung |
| --- | --- | --- |
| G01 | Chưa đăng nhập, phiên không hợp lệ hoặc thiếu quyền | Từ chối thao tác; yêu cầu xác thực lại hoặc thông báo không đủ quyền. Không tiết lộ hồ sơ ngoài phạm vi được phép. |
| G02 | Thiếu dữ liệu, mã không tồn tại hoặc định dạng sai | Chỉ rõ trường cần sửa; không ghi giao dịch dở dang. |
| G03 | Dữ liệu thay đổi trong lúc xử lý | Đọc lại và kiểm tra điều kiện; không để hai giao dịch cùng nhận một cuốn hoặc cùng làm vượt hạn mức. |
| G04 | Gửi lại yêu cầu đã hoàn tất | Trả lại kết quả cũ, không tạo thêm phiếu, cộng hạn hoặc thu tiền lần nữa. Cùng mã yêu cầu nhưng nội dung khác phải báo xung đột. |
| G05 | Lỗi lưu trước khi giao dịch dữ liệu hoàn tất | Không ghi nhận thành công; bảo toàn trạng thái nhất quán, không để phiếu và cuốn lệch nhau. Nếu đã nhận sách hoặc tiền thật, phải đối soát thực tế, không coi hoàn tác dữ liệu là hoàn tác giao nhận vật lý. |
| G06 | Gửi thông báo lỗi sau khi giao dịch đã lưu thành công | Giữ giao dịch, ghi trạng thái gửi lỗi và cho phép gửi lại không trùng. Nếu việc xét quyền nhận chưa xong thì không giao cuốn vượt hàng chờ. |

Các quy tắc chung thực hiện NFR02–NFR06. Mỗi thao tác thay đổi dữ liệu phải lưu được người thực hiện, thời điểm, đối tượng và lý do phù hợp.

### 1.3. Quy ước thời gian và xử lý nghiệp vụ

- **Lịch xử lý:** thư viện mở thứ Hai đến thứ Bảy, đóng cửa lúc 17:00 theo giờ Việt Nam; nghỉ Chủ nhật và các ngày nghỉ đặc biệt đã khai báo.
- **Cộng ngày mở cửa:** không tính ngày bắt đầu và bỏ qua ngày thư viện đóng cửa.
- **Hiệu lực thẻ:** 6 tháng và cửa sổ gia hạn 30 ngày tính theo lịch.
- **Thời hạn giữ chỗ:** kết thúc lúc 17:00 của ngày mở cửa thứ hai sau ngày cấp quyền. Người thứ hai được tính thời hạn riêng khi chuyển quyền.
- **Q01 — Nhận trả và thanh toán:** ghi ngày nhận sách thực tế, dừng tính thêm phạt quá hạn của cuốn sau ngày đó; khoản chưa thu vẫn được theo dõi riêng và chặn quyền sử dụng mới theo quy định. Nếu thu hoặc lưu lỗi, phải đối soát trước khi thu lại.
- **Q02 — Mất sách:** chỉ kết thúc nghĩa vụ sách và giải phóng hạn mức khi đã hoàn tất bồi thường và thanh toán đủ tiền phạt đến ngày tất toán. Báo mất hoặc chỉ nộp bản thay thế chưa đủ để tất toán.
- **Q03 — Thay đổi lịch:** áp dụng lịch mới cho các phép tính mới; không tự sửa hạn trả hoặc hạn nhận đã chốt. Không tính phạt vào ngày thư viện thực tế đóng cửa theo lịch đã xác nhận.
- **Q04 — Đóng hồ sơ:** vô hiệu hóa quyền sử dụng nhưng giữ hồ sơ và lịch sử; không tự động xóa hoặc ẩn danh trong phiên bản bài tập.

**Quy ước xử lý yêu cầu mượn nhiều cuốn tại UC03:**

1. Hệ thống kiểm tra toàn bộ danh sách cuốn trong một yêu cầu mượn.
2. Chỉ ghi nhận thành công khi tất cả cuốn và điều kiện của độc giả đều hợp lệ.
3. Nếu có cuốn không hợp lệ, hệ thống thông báo cuốn đó và lý do; chưa lập phiếu mượn cho yêu cầu.
4. Nếu độc giả muốn mượn những cuốn còn lại, thủ thư sửa danh sách, xác nhận lại và gửi yêu cầu mới.
5. Hệ thống kiểm tra lại toàn bộ điều kiện trước khi lưu.
6. Không tạo phiếu hoặc cập nhật trạng thái một phần khiến kết quả khác với danh sách đã xác nhận.

## 2. Danh sách Use Case và phân công

| Mã | Use Case | Tác nhân trực tiếp | Phụ trách |
| --- | --- | --- | --- |
| UC01 | Cấp mới và cấp lại thẻ | Thủ thư | Lan |
| UC02 | Gia hạn và khôi phục thẻ | Thủ thư | Lan |
| UC03 | Mượn sách | Thủ thư | Quân |
| UC04 | Gia hạn mượn | Độc giả hoặc thủ thư được phép | Quân |
| UC05 | Trả sách và thu tiền | Thủ thư | Quân |
| UC06 | Đăng ký và hủy hàng chờ | Độc giả hoặc thủ thư được phép | Lan |
| UC07 | Xét và chuyển lượt giữ chỗ | Thủ thư; có thể được kích hoạt bởi tác vụ nội bộ | Lan |
| UC08 | Xử lý mất, hỏng và bồi thường | Thủ thư, người quản lý | Quân |
| UC09 | Thanh lý và nhập bản thay thế | Người quản lý, thủ thư | Quân |
| UC10 | Xác nhận nghĩa vụ và đóng hồ sơ | Người quản lý | Lan |
| UC11 | Quản lý đầu sách, cuốn sách và vị trí | Thủ thư, người quản lý theo quyền | Lan |
| UC12 | Tra cứu và thống kê | Độc giả, thủ thư, người quản lý | Quân |

## 3. UC01 — Cấp mới và cấp lại thẻ

### 3.1. Mục tiêu và tác nhân

- **Mục tiêu:** cấp thẻ cho độc giả hợp lệ hoặc thay thẻ cũ mà không làm mất hồ sơ và nghĩa vụ.
- **Tác nhân chính:** thủ thư.
- **Phối hợp:** độc giả cung cấp thông tin; nguồn dữ liệu nhà trường hỗ trợ xác minh.
- **Kích hoạt:** độc giả đề nghị cấp mới hoặc cấp lại thẻ.
- **Đầu vào:** mã trường, thông tin định danh, loại độc giả, loại yêu cầu; thông tin thẻ cũ và lý do nếu cấp lại.
- **Quy tắc:** BR01–BR04, NFR01, NFR04–NFR05.

### 3.2. Điều kiện trước

1. Thủ thư đã đăng nhập và có quyền quản lý thẻ.
2. Có thông tin đủ để xác minh độc giả với nhà trường.
3. Với cấp lại, phải xác định được hồ sơ và thẻ cần thay.

### 3.3. Luồng chính — Cấp mới

1. Thủ thư chọn cấp mới và nhập thông tin độc giả.
2. Hệ thống xác minh mã trường, tư cách thuộc trường và loại sinh viên/giảng viên.
3. Hệ thống tìm hồ sơ hiện có theo mã trường và kiểm tra thẻ đang hoạt động.
4. Nếu chưa có hồ sơ, hệ thống tạo hồ sơ độc giả; nếu đã có hồ sơ hợp lệ thì sử dụng hồ sơ đó, không tạo trùng.
5. Hệ thống tạo số thẻ riêng, xác định hiệu lực 6 tháng từ ngày cấp.
6. Thủ thư kiểm tra và xác nhận thông tin.
7. Hệ thống lưu thẻ, liên kết hồ sơ và nhật ký; bảo đảm tối đa một thẻ hoạt động.
8. Hệ thống hiển thị kết quả cấp thẻ và hạn sử dụng.

### 3.4. Luồng thay thế — Cấp lại

1. Thủ thư chọn cấp lại và tìm hồ sơ/thẻ cũ.
2. Hệ thống xác minh độc giả và hiển thị hạn thẻ, lịch sử, công nợ.
3. Thủ thư nhập lý do và xác nhận cấp lại.
4. Hệ thống khóa mã thẻ cũ, tạo mã mới gắn cùng hồ sơ, giữ hạn cũ và liên kết lịch sử cấp lại.
5. Hệ thống lưu đồng thời các thay đổi và thông báo kết quả.

Cấp lại khi còn nợ không xóa nợ hoặc tự khôi phục quyền mượn. Nếu thẻ đã hết hạn, cấp lại không làm thẻ có hạn mới; muốn khôi phục phải thực hiện UC02 theo điều kiện tương ứng.

### 3.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC01-E01 | Không thuộc đối tượng phục vụ hoặc chưa xác minh được | Không cấp thẻ; nêu nguyên nhân hoặc yêu cầu bổ sung xác minh. |
| UC01-E02 | Xin cấp mới nhưng đã có thẻ hoạt động | Từ chối cấp thêm; hướng dẫn dùng thẻ hiện có hoặc chọn cấp lại khi phù hợp. |
| UC01-E03 | Hồ sơ có thẻ hết hạn nhưng chọn cấp mới để thay đổi hạn | Hướng sang UC02; không tạo hồ sơ mới để bỏ qua điều kiện khôi phục. |
| UC01-E04 | Thẻ cần cấp lại không thuộc hồ sơ đã chọn | Từ chối, giữ nguyên thẻ và hồ sơ. |
| UC01-E05 | Mã thẻ trùng hoặc lỗi lưu | Không để hai thẻ hoạt động; áp dụng G03–G05. |

### 3.6. Kết quả sau thực hiện

- **Thành công:** có thẻ mới hoặc thẻ cấp lại gắn đúng hồ sơ; tối đa một thẻ hoạt động; cấp lại giữ hạn cũ và toàn bộ nghĩa vụ.
- **Thất bại:** không có thẻ mới hợp lệ được ghi nhận; không khóa mất thẻ cũ chỉ vì giao dịch cấp lại chưa hoàn tất; lịch sử không bị xóa.

## 4. UC02 — Gia hạn và khôi phục thẻ

### 4.1. Mục tiêu và tác nhân

- **Mục tiêu:** thay đổi hiệu lực thẻ đúng trường hợp còn hạn hoặc đã hết hạn.
- **Tác nhân chính:** thủ thư; độc giả đề nghị thực hiện.
- **Phối hợp:** nguồn dữ liệu nhà trường để kiểm tra tư cách.
- **Đầu vào:** mã độc giả/thẻ, loại yêu cầu và ngày thực hiện do hệ thống xác định.
- **Quy tắc:** BR01, BR03–BR04, BR06, BR08 và điều kiện chung tại mục 6.6 của tài liệu yêu cầu.

### 4.2. Điều kiện trước

1. Thủ thư có quyền xử lý thẻ và xác định được hồ sơ cần xử lý.
2. Hồ sơ chưa đóng; có dữ liệu về thẻ, tư cách sử dụng và công nợ để kiểm tra.
3. Yêu cầu phân biệt rõ gia hạn thẻ còn hạn hay khôi phục thẻ hết hạn.

### 4.3. Luồng chính — Gia hạn thẻ còn hạn

1. Thủ thư chọn thẻ và yêu cầu gia hạn.
2. Hệ thống xác minh độc giả còn thuộc đối tượng phục vụ, chưa mất quyền và không còn nợ.
3. Hệ thống kiểm tra thẻ còn hạn và ngày hiện tại nằm trong 30 ngày cuối trước hoặc đúng ngày hết hạn.
4. Hệ thống tính hạn mới bằng hạn cũ cộng 6 tháng lịch.
5. Thủ thư xác nhận hạn mới.
6. Hệ thống lưu thay đổi và nhật ký, hiển thị kết quả.

### 4.4. Luồng thay thế — Khôi phục thẻ hết hạn

1. Thủ thư chọn khôi phục.
2. Hệ thống kiểm tra thẻ đã hết hạn, độc giả còn hợp lệ, chưa mất quyền và không còn nợ.
3. Hệ thống tính hiệu lực mới 6 tháng từ ngày khôi phục.
4. Thủ thư xác nhận; hệ thống lưu và thông báo kết quả.

### 4.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC02-E01 | Gia hạn ngoài cửa sổ 30 ngày | Từ chối, không thay đổi hạn thẻ. |
| UC02-E02 | Chọn gia hạn cho thẻ đã hết hạn hoặc khôi phục cho thẻ còn hạn | Thông báo sai loại yêu cầu; yêu cầu chọn đúng nghiệp vụ. |
| UC02-E03 | Còn nợ, mất tư cách hoặc bị dừng quyền | Từ chối gia hạn/khôi phục; không chặn việc xem và thanh toán nghĩa vụ. |
| UC02-E04 | Gửi lặp sau khi đã gia hạn | Cùng yêu cầu trả kết quả cũ; yêu cầu mới phải xét cửa sổ trên hạn mới, không cộng liên tiếp theo hạn cũ. |
| UC02-E05 | Thẻ đã bị thay hoặc hồ sơ đã đóng trong lúc xử lý | Kiểm tra lại và từ chối cập nhật trên bản không còn hợp lệ. |

### 4.6. Kết quả sau thực hiện

- **Thành công:** hạn thẻ cập nhật đúng nhánh; không tạo khoản phạt chỉ vì hết hạn thẻ; hạn trả sách đã mượn giữ nguyên.
- **Thất bại:** hạn thẻ và nghĩa vụ đang có không thay đổi.

## 5. UC03 — Mượn sách

### 5.1. Mục tiêu và tác nhân

- **Mục tiêu:** giao đúng cuốn cho độc giả đủ điều kiện và ghi nhận đầy đủ lượt mượn.
- **Tác nhân chính:** thủ thư; độc giả là người nhận sách.
- **Đầu vào:** mã độc giả/thẻ, danh sách mã cuốn không trùng, tình trạng lúc giao, mã yêu cầu.
- **Quy tắc:** BR05–BR06, BR10–BR13, BR16, BR23–BR25, NFR02–NFR04.

### 5.2. Điều kiện trước

1. Thủ thư đã đăng nhập và có quyền cho mượn.
2. Đã nhận diện người nhận sách và có danh sách cuốn cần kiểm tra.
3. Có lịch mở cửa để tính hạn và dữ liệu thẻ, công nợ, lượt mượn, giữ chỗ hiện tại.

### 5.3. Luồng chính

1. Thủ thư chọn độc giả, quét/nhập mã từng cuốn và ghi tình trạng lúc giao.
2. Hệ thống kiểm tra yêu cầu đã được xử lý hay chưa.
3. Hệ thống đọc lại thẻ, quyền sử dụng, các khoản chưa thanh toán và tổng số cuốn chưa hoàn tất nghĩa vụ trên mọi phiếu.
4. Hệ thống kiểm tra thẻ còn hạn, còn quyền, không nợ và tổng số cuốn sau yêu cầu không vượt 3 đối với sinh viên hoặc 5 đối với giảng viên.
5. Với từng cuốn, hệ thống kiểm tra khả dụng và quyền giữ chỗ; không cho giao cuốn đang mượn, mất, sửa, thanh lý hoặc giữ cho người khác.
6. Hệ thống tính hạn trả: 14 ngày mở cửa với sinh viên, 30 ngày với giảng viên, không tính ngày giao.
7. Thủ thư kiểm tra danh sách và xác nhận giao dịch.
8. Hệ thống lưu phiếu mượn, từng dòng, hạn trả, tình trạng giao và trạng thái cuốn trong một lần xử lý nhất quán. Nếu nhận từ giữ chỗ, đồng thời ghi nhận lượt đã nhận và kết thúc đợt tương ứng.
9. Hệ thống lưu kết quả yêu cầu, nhật ký và thông tin cần xử lý tiếp cho hàng chờ; trả biên nhận.
10. Sau khi giao dịch được xác nhận thành công, thủ thư giao sách cho độc giả.

### 5.4. Luồng thay thế

- **Nhận cuốn đang giữ chỗ:** chỉ người có quyền nhận còn hiệu lực và đáp ứng lại điều kiện mượn mới được nhận. Khi người thứ nhất nhận, UC07 kiểm tra lại người thứ hai theo dòng mượn mới, không cấp quyền nhận cuốn đang mượn.
- **Thẻ sắp hết hạn:** vẫn tính đủ 14/30 ngày; không rút ngắn hạn trả theo hạn thẻ.
- **Muốn mượn một phần danh sách không hợp lệ:** thủ thư loại cuốn không hợp lệ, cho độc giả kiểm tra danh sách còn lại và gửi yêu cầu mới theo quy ước mục 1.3.

### 5.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC03-E01 | Hết hạn thẻ, mất quyền hoặc còn nợ | Từ chối yêu cầu, nêu điều kiện chưa đạt. |
| UC03-E02 | Vượt hạn mức sau khi cộng các cuốn mới | Từ chối; tính cả sách đã báo mất nhưng chưa hoàn tất nghĩa vụ. |
| UC03-E03 | Mã cuốn trùng trong yêu cầu hoặc cuốn không thể giao | Không lập phiếu; yêu cầu sửa danh sách. |
| UC03-E04 | Có quyền ưu tiên của người khác hoặc việc xét hàng chờ chưa xong | Không giao cuốn để vượt hàng chờ. |
| UC03-E05 | Hai yêu cầu cạnh tranh cùng cuốn hoặc cùng hạn mức | Chỉ chấp nhận kết quả đáp ứng ràng buộc; yêu cầu còn lại đọc lại dữ liệu và báo lý do. |
| UC03-E06 | Lỗi lưu giữa phiếu, dòng và trạng thái cuốn | Không lưu một phần và không giao sách khi chưa xác nhận thành công. |

### 5.6. Kết quả sau thực hiện

- **Thành công:** các cuốn thuộc đúng phiếu/dòng của người nhận; hạn trả và tình trạng đã chốt; không cuốn nào có hai lượt cùng chiếm quyền mượn; tổng nghĩa vụ không vượt hạn mức.
- **Thất bại trước khi giao sách:** không có phiếu mới hoàn tất và không thay đổi quyền sở hữu lượt mượn; sách vẫn ở thư viện.

## 6. UC04 — Gia hạn mượn

### 6.1. Mục tiêu và tác nhân

- **Mục tiêu:** kéo dài hạn trả của một cuốn đúng điều kiện.
- **Tác nhân chính:** độc giả hoặc thủ thư được phép thao tác thay.
- **Đầu vào:** mã dòng mượn, số ngày muốn thêm trong tập 3/5/7, mã yêu cầu.
- **Quy tắc:** BR08, BR14–BR15, NFR03, NFR05.

### 6.2. Điều kiện trước

1. Người thao tác đã xác thực và được phép xử lý dòng mượn.
2. Có dòng mượn để kiểm tra; độc giả tự thực hiện phải là chủ của dòng đó.
3. Hệ thống có lịch mở cửa và dữ liệu thẻ, nợ, đăng ký chờ liên quan.

### 6.3. Luồng chính

1. Người dùng chọn cuốn đang mượn và số ngày cần gia hạn.
2. Hệ thống kiểm tra quyền và yêu cầu lặp.
3. Hệ thống kiểm tra dòng chưa trả, chưa kết thúc theo xử lý mất, chưa quá hạn và chưa gia hạn lần nào.
4. Hệ thống kiểm tra độc giả còn quyền, thẻ còn hạn, không nợ và cuốn không có đăng ký chờ hiệu lực.
5. Hệ thống tính hạn mới bằng hạn cũ cộng số ngày mở cửa đã chọn, không tính ngày hạn cũ.
6. Người dùng xác nhận hạn mới.
7. Hệ thống lưu hạn mới, đánh dấu đã gia hạn, lưu nhật ký và trả kết quả.

### 6.4. Luồng thay thế

- Thủ thư thực hiện thay độc giả vẫn phải kiểm tra cùng điều kiện; quyền nhân viên không cho phép tự bỏ qua hàng chờ hoặc công nợ.
- Gửi lại cùng yêu cầu đã thành công trả hạn mới đã lưu, không cộng thêm ngày.

### 6.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC04-E01 | Số ngày không phải 3, 5 hoặc 7 | Yêu cầu chọn lại, không đổi hạn. |
| UC04-E02 | Dòng đã trả, đã kết thúc, quá hạn hoặc từng gia hạn | Từ chối. Gia hạn trước đó 3 ngày cũng không cho thêm lần thứ hai. |
| UC04-E03 | Hết thẻ, mất quyền hoặc còn nợ | Từ chối; giữ nguyên hạn đang có. |
| UC04-E04 | Có đăng ký chờ hiệu lực hoặc phát sinh chờ trước khi lưu | Kiểm tra lại và từ chối, bảo vệ quyền người chờ. |
| UC04-E05 | Độc giả yêu cầu gia hạn dòng của người khác | Từ chối truy cập theo G01. |

### 6.6. Kết quả sau thực hiện

- **Thành công:** chỉ hạn trả của dòng được chọn và trạng thái gia hạn thay đổi; không tạo lượt mượn mới.
- **Thất bại:** hạn trả và số lần gia hạn giữ nguyên.

## 7. UC05 — Trả sách và thu tiền

### 7.1. Mục tiêu và tác nhân

- **Mục tiêu:** ghi nhận đúng sách thực nhận, nghĩa vụ phát sinh và tiền thực thu, sau đó xử lý khả dụng.
- **Tác nhân chính:** thủ thư; độc giả trả sách và thanh toán.
- **Phối hợp:** người quản lý nếu cần duyệt sự cố/định giá ở UC08.
- **Đầu vào:** dòng mượn cần trả, cuốn thực nhận, ngày nhận, tình trạng, căn cứ thu và xác nhận tiền thực tế, mã yêu cầu.
- **Quy tắc:** BR16–BR19, BR28–BR32, NFR02–NFR06; nhánh lỗi sau tiếp nhận phụ thuộc Q01.

### 7.2. Điều kiện trước

1. Thủ thư có quyền nhận trả và ghi nhận thu tiền theo phân quyền.
2. Với nhận trả: có sách vật lý để đối chiếu và xác định đúng dòng mượn. Với thanh toán công nợ đã ghi nhận: có mã khoản phải thu và chứng từ liên quan, không yêu cầu nhận lại sách lần nữa.
3. Hệ thống có hạn trả, tình trạng lúc giao và lịch mở cửa.

Thẻ hết hạn, còn nợ hoặc đã bị dừng quyền sử dụng mới không phải lý do từ chối tiếp nhận trả sách.

### 7.3. Luồng chính — Nhận trả và thanh toán thành công

1. Thủ thư xác định độc giả và chọn các dòng tương ứng với cuốn thực nhận.
2. Hệ thống kiểm tra đúng cuốn, đúng dòng và tình trạng xử lý trước đó.
3. Thủ thư ghi ngày nhận thật, kiểm tra tình trạng, đối chiếu dấu vết lúc giao.
4. Hệ thống tính phạt từng cuốn: 5.000 đồng nhân số ngày mở cửa sau hạn đến hết ngày nhận; không trễ thì không phát sinh phạt.
5. Nếu có hư hỏng cần bồi thường, thực hiện nhánh đánh giá tại UC08 trước khi xác nhận khoản liên quan; không tự quy hao mòn thành lỗi của người đọc.
6. Hệ thống hiển thị riêng tiền phạt, bồi thường và tổng khoản phải thanh toán của lần xử lý.
7. Độc giả thanh toán đủ; thủ thư xác nhận số tiền thực nhận. Nếu không có khoản phải thu thì bỏ bước thu tiền.
8. Hệ thống lưu nhận trả, tình trạng, các nghĩa vụ, phiếu thu/chi tiết thu khi có tiền và nhật ký. Chỉ kết thúc các dòng đủ điều kiện hoàn tất.
9. Cuốn dùng được chuyển sang xét quyền ưu tiên bằng UC07; cuốn hỏng chuyển xử lý sửa chữa/thanh lý, không cho mượn ngay.
10. Hệ thống trả kết quả nhận sách, thu tiền và nghĩa vụ còn lại của phiếu.

### 7.4. Luồng thay thế

- **Trả một phần phiếu:** chỉ các dòng được chọn thay đổi; dòng chưa trả giữ nguyên hạn và nghĩa vụ.
- **Thanh toán công nợ sau nhận trả:** thủ thư chọn khoản chưa hoàn tất, đối soát tiền đã nhận, thu đủ phần còn phải thu theo Q01 và xác nhận chứng từ. Không ghi thêm lần nhận trả, không sửa ngày nhận thực tế và không tăng phạt sau ngày nhận sách. Với mất sách chưa nhận lại, việc tính lại phạt và tất toán thuộc UC08/Q02.
- **Hao mòn hoặc dấu vết cũ:** ghi nhận tình trạng nhưng không phát sinh bồi thường cho phần đó.
- **Phát hiện mất thay vì có cuốn để nhận:** xử lý báo mất bằng UC08; không ghi ngày trả vật lý giả.
- **Đã nhận sách nhưng chưa thu được đủ hoặc có lỗi thu:** đi theo nhánh Q01 ở mục lỗi, chưa coi toàn bộ nghĩa vụ đã hoàn tất.

### 7.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC05-E01 | Không khớp cuốn/dòng hoặc dòng đã xử lý | Không tạo thêm lần trả; nếu gửi lặp đúng yêu cầu thì trả kết quả cũ. |
| UC05-E02 | Ngày nhận không hợp lệ hoặc tình trạng thiếu thông tin | Yêu cầu sửa/đối chiếu; không tính khoản thu trên dữ liệu sai. |
| UC05-E03 | Thu thiếu hoặc chưa xác nhận nhận tiền | Không đánh dấu khoản đã thu đủ; không tạo phiếu thu thành công cho số tiền chưa nhận. |
| UC05-E04 | Đã nhận sách vật lý rồi thanh toán/lưu giao dịch lỗi | Áp dụng Q01: lưu ngày nhận thực tế và trạng thái nhận trả riêng với thanh toán; giữ khoản chưa thu, hạn chế quyền sử dụng mới và đối soát. Nếu chưa lưu được, giữ bằng chứng giao nhận để cập nhật đúng ngày nhận. Không tiếp tục tăng phạt sau ngày nhận thật hoặc báo rằng sách đã tự quay lại độc giả. |
| UC05-E05 | Đã nhận tiền thật nhưng chưa xác nhận được dữ liệu lưu | Đối soát giao dịch trước khi thu lại; không yêu cầu nộp lần hai chỉ vì ứng dụng báo lỗi. |
| UC05-E06 | Thiếu căn cứ định giá hoặc chưa được duyệt | Chuyển xử lý UC08; không tự đặt giá và không tuyên bố đã hoàn tất bồi thường. |
| UC05-E07 | Xét hàng chờ hoặc gửi thông báo lỗi sau khi đã lưu nhận trả | Giữ ngày nhận và chứng từ; xử lý lại phần lỗi. Không mở cuốn cho mượn vượt quyền ưu tiên. |

### 7.6. Kết quả sau thực hiện

- **Thành công:** lưu đúng ngày nhận, tình trạng, khoản phát sinh và tiền thực thu; dòng đủ điều kiện kết thúc; phiếu chỉ hoàn tất khi mọi dòng đã xử lý xong.
- **Thất bại trước giao nhận thực tế:** không có giao dịch thành công mới; dữ liệu không bị ghi dở.
- **Ngoại lệ sau giao nhận thực tế:** phải giữ/đối soát bằng chứng nhận sách, nhận tiền; không đánh dấu hết nghĩa vụ khi còn khoản chưa giải quyết. Theo Q01, trạng thái nhận trả và thanh toán được theo dõi riêng; khoản chưa thu vẫn tồn tại và phạt quá hạn của cuốn đã nhận dừng tại ngày nhận thực tế.

## 8. UC06 — Đăng ký và hủy hàng chờ

### 8.1. Mục tiêu và tác nhân

- **Mục tiêu:** quản lý đăng ký chờ đúng cuốn, đúng người và đúng thứ tự.
- **Tác nhân chính:** độc giả hoặc thủ thư được phép thao tác thay.
- **Đầu vào:** mã độc giả, mã cuốn hoặc mã đăng ký, loại thao tác; lý do khi hủy.
- **Quy tắc:** BR21–BR22, BR27–BR28 và điều kiện chung mục 6.6 của tài liệu yêu cầu.

### 8.2. Điều kiện trước

1. Người thao tác được xác thực và có quyền trên hồ sơ/đăng ký.
2. Đăng ký mới phải xác định được cuốn muốn chờ và độc giả cần kiểm tra điều kiện.
3. Hủy phải xác định được đăng ký đang hiệu lực; không yêu cầu độc giả hết nợ mới được hủy.

### 8.3. Luồng chính — Đăng ký chờ

1. Người dùng chọn một cuốn cụ thể chưa sẵn sàng cho mượn trực tiếp.
2. Hệ thống kiểm tra quyền sử dụng, hiệu lực thẻ và công nợ của độc giả.
3. Hệ thống kiểm tra người này chưa có đăng ký chờ hiệu lực ở bất kỳ cuốn nào.
4. Hệ thống hiển thị cuốn và điều kiện chờ để người dùng xác nhận.
5. Hệ thống tạo đăng ký, ghi thời điểm và số thứ tự tăng để phân xử cùng thời điểm.
6. Hệ thống lưu nhật ký, trả mã đăng ký và trạng thái đang chờ.

Đăng ký thành công chưa có nghĩa độc giả đã có quyền nhận. Điều kiện còn hạn mức và mốc thẻ 21/37 ngày được xét khi chọn người ở UC07, sau đó kiểm tra lại khi giao ở UC03.

### 8.4. Luồng thay thế — Hủy đăng ký

1. Người dùng chọn đăng ký của mình hoặc đăng ký được phép quản lý.
2. Hệ thống kiểm tra đăng ký còn hiệu lực và hiển thị thông tin.
3. Người dùng xác nhận hủy; hệ thống chuyển trạng thái, lưu thời điểm/lý do, không xóa bản ghi.
4. Nếu đăng ký đang có lượt nhận, hệ thống yêu cầu UC07 xử lý phần còn lại của đợt; không tự thêm người thứ ba.
5. Hệ thống trả kết quả hủy và giữ thứ tự tương đối của các đăng ký khác.

### 8.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC06-E01 | Đã có đăng ký hiệu lực ở cuốn khác hoặc cùng cuốn | Từ chối tạo mới; hiển thị đăng ký hiện có trong phạm vi được phép. |
| UC06-E02 | Mất quyền, thẻ hết hạn hoặc còn nợ khi đăng ký mới | Từ chối tạo đăng ký; vẫn cho phép hủy đăng ký hiện có. |
| UC06-E03 | Cuốn đang sẵn sàng cho mượn trực tiếp | Hướng sang UC03; không tạo hàng chờ giả hoặc tự suy diễn dòng mượn nguồn. |
| UC06-E04 | Không tìm thấy cuốn hoặc yêu cầu truy cập đăng ký của người khác | Từ chối theo G01–G02. |
| UC06-E05 | Đăng ký đã hủy/kết thúc hoặc bị thay đổi trước khi xác nhận | Trả trạng thái hiện tại; không hủy lần hai hoặc đảo thứ tự. |
| UC06-E06 | Hai yêu cầu đăng ký mới đồng thời của cùng độc giả | Chỉ tối đa một đăng ký hiệu lực được lưu. |

### 8.6. Kết quả sau thực hiện

- **Đăng ký thành công:** có một đăng ký hiệu lực, chưa tự cấp quyền nhận và chưa phát sinh phiếu mượn.
- **Hủy thành công:** đăng ký mất hiệu lực nhưng còn lịch sử; quyền giữ chỗ liên quan được xử lý phù hợp.
- **Thất bại:** không tạo đăng ký trùng, không xóa hoặc thay đổi đăng ký không thuộc quyền.

## 9. UC07 — Xét và chuyển lượt giữ chỗ

### 9.1. Mục tiêu và tác nhân

- **Mục tiêu:** cấp quyền nhận theo thứ tự, tối đa hai người trong một đợt và tối đa một người có quyền nhận tại một thời điểm.
- **Tác nhân trực tiếp:** thủ thư khi xử lý thủ công; hệ thống cũng tự kích hoạt theo sự kiện hoặc tác vụ hết lượt.
- **Bên nhận kết quả:** độc giả trong hàng chờ.
- **Đầu vào:** cuốn, dòng mượn nguồn, hàng chờ, thời điểm hiện tại và sự kiện bắt đầu/kết thúc lượt.
- **Quy tắc:** BR16, BR21–BR28; thời hạn phụ thuộc lịch Q03.

### 9.2. Điều kiện trước

1. Lệnh xử lý đến từ thủ thư có quyền hoặc tác vụ nội bộ hợp lệ.
2. Khi mở đợt mới, cuốn đã được kiểm tra, thực sự có thể giao và không đang cho mượn; có dòng mượn nguồn để tính mốc thẻ.
3. Khi chuyển lượt hoặc xử lý đã giao, phải xác định đúng đợt/lượt hiện có và sự kiện liên quan.
4. Lịch mở cửa và giờ kết thúc lượt phải có căn cứ được xác nhận; áp dụng lịch mặc định và ngày nghỉ bổ sung theo Q03; dữ liệu demo phải ghi rõ các ngày nghỉ đã khai báo.

### 9.3. Luồng chính — Mở đợt và cấp lượt đầu

1. Hệ thống đọc lại cuốn và kiểm tra chưa có đợt mở khác cho cùng lần sẵn sàng này.
2. Hệ thống lấy hàng chờ hiệu lực theo thời điểm và thứ tự đã ghi.
3. Hệ thống xác định ngày bắt đầu và loại người mượn của dòng nguồn.
4. Hệ thống tính mốc thẻ: cộng 21 ngày mở cửa nếu người mượn nguồn là sinh viên, 37 ngày nếu là giảng viên. Không lấy loại của người đang chờ để chọn mốc.
5. Hệ thống duyệt từ đầu: kiểm tra quyền, công nợ, hạn mức và thẻ còn hiệu lực đến mốc; đăng ký thiếu điều kiện bị kết thúc có lý do.
6. Hệ thống chọn tối đa hai người hợp lệ. Sau khi đủ hai người thì không chọn thêm người thứ ba vào đợt.
7. Hệ thống tạo đợt và các lượt đã chọn; chỉ kích hoạt lượt người thứ nhất. Lượt người thứ hai chưa chạy thời hạn.
8. Hệ thống tính hạn nhận của người thứ nhất đến giờ đóng cửa ngày mở cửa thứ hai sau ngày cấp quyền.
9. Hệ thống lưu quyền giữ chỗ, nhật ký và thông báo cho người được nhận.

### 9.4. Luồng thay thế

**A1 — Không có hoặc chỉ có một người hợp lệ**

- Không có người hợp lệ: không tạo quyền nhận, cuốn trở về sẵn sàng sau khi hoàn tất xét hàng chờ.
- Chỉ có một người hợp lệ: tạo một lượt; nếu lượt đó hết mà không nhận thì kết thúc đợt, không kéo người mới vào đợt đã chốt.

**A2 — Người thứ nhất hết lượt, hủy hoặc mất điều kiện**

1. Hệ thống kết thúc lượt và đăng ký tương ứng, lưu lý do.
2. Hệ thống kiểm tra lại người thứ hai đã được chọn và cuốn vẫn có thể giao.
3. Nếu người thứ hai hợp lệ, kích hoạt lượt này và tính hai ngày mở cửa riêng từ thời điểm chuyển quyền.
4. Nếu không hợp lệ hoặc không có người thứ hai, kết thúc đợt; không bổ sung người thứ ba.

**A3 — Người thứ nhất đã nhận và mượn thành công qua UC03**

1. Ghi nhận lượt đã nhận và kết thúc đợt hiện tại; cuốn ở trạng thái đang mượn.
2. Dùng dòng mượn mới làm nguồn để kiểm tra lại mốc thẻ của người thứ hai.
3. Nếu người thứ hai đủ điều kiện, giữ đăng ký ở trạng thái tiếp tục chờ; nếu không, kết thúc đăng ký có lý do.
4. Không kích hoạt thời hạn nhận hoặc gửi thông báo mời nhận cuốn đang cho mượn.

**A4 — Người thứ hai hết lượt mà không nhận**

- Kết thúc lượt và đợt, đưa cuốn về sẵn sàng cho người đủ điều kiện. Các đăng ký chưa đến lượt được giữ theo trạng thái thực tế, nhưng không mở ngay một đợt mới chỉ để xét người thứ ba trong cùng lần sẵn sàng.

**A5 — Sách bị hỏng hoặc không thể giao**

- Tạm dừng đợt, thông báo toàn hàng, không chạy thời hạn nhận khi chưa có sách dùng được. Người chờ chọn tiếp tục hoặc hủy; việc chuyển sang bản thay thế thực hiện qua UC09.

### 9.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC07-E01 | Không xác định được dòng nguồn cần thiết | Không tự chọn mốc 21/37. Nếu cuốn sẵn sàng và không thuộc đợt cần xét thì xử lý mượn trực tiếp; nếu dữ liệu liên quan thiếu, yêu cầu đối soát. |
| UC07-E02 | Sách đã được giao hoặc không còn dùng được | Không cấp quyền nhận mới; xử lý đúng nhánh đã giao/tạm dừng. |
| UC07-E03 | Sự kiện/tác vụ chạy lặp hoặc hai xử lý cùng mở đợt | Chỉ lưu một kết quả hợp lệ; không tạo hai đợt hoặc hai quyền nhận đồng thời. |
| UC07-E04 | Giờ kết thúc lượt hoặc lịch cần thiết chưa được xác định | Không gửi thông báo có hạn nhận tùy ý; khôi phục cấu hình lịch theo Q03 rồi tính lại trước khi cấp quyền. Không có ngày nghỉ đặc biệt thì dùng lịch mặc định thứ Hai đến thứ Bảy, đóng cửa lúc 17:00. |
| UC07-E05 | Lỗi lưu hoặc thông báo | Giữ tính nhất quán của quyền ưu tiên; xử lý lại theo G05–G06, không cho giao vượt hàng. |

### 9.6. Kết quả sau thực hiện

- **Thành công:** cuốn được giữ cho đúng một người, đang tiếp tục chờ do đã được mượn, bị tạm dừng hoặc trở về sẵn sàng theo đúng nhánh; một đợt tối đa hai lượt.
- **Thất bại:** không phát sinh quyền nhận không có căn cứ; dữ liệu không cho phép hai người cùng được nhận một cuốn.

## 10. UC08 — Xử lý mất, hỏng và bồi thường

### 10.1. Mục tiêu và tác nhân

- **Mục tiêu:** ghi nhận sự cố, xác định đúng nghĩa vụ và theo dõi xử lý đến khi đủ điều kiện hoàn tất.
- **Tác nhân chính:** thủ thư lập và xử lý biên bản; người quản lý phê duyệt khi cần.
- **Phối hợp:** độc giả báo mất, cung cấp thông tin và lựa chọn phương án hợp lệ.
- **Đầu vào:** dòng mượn, loại sự cố, ngày báo/nhận, tình trạng, minh chứng, giá trị có căn cứ và hình thức bồi thường.
- **Quy tắc:** BR18, BR29–BR35; Q01 và Q02 ảnh hưởng nhánh thanh toán/tất toán.

### 10.2. Điều kiện trước

1. Nhân viên có quyền tương ứng và xác định được dòng mượn/cuốn xảy ra sự cố.
2. Có thông tin báo mất hoặc kiểm tra hư hỏng thực tế để ghi nhận.
3. Việc xác nhận khoản bồi thường phải có căn cứ giá và phê duyệt cần thiết; thiếu căn cứ vẫn có thể ghi nhận sự cố đang chờ xử lý, không tự chốt tiền.

### 10.3. Luồng chính — Ghi nhận và xử lý sự cố

1. Thủ thư chọn dòng mượn, kiểm tra sự cố và nghĩa vụ đã có để tránh ghi trùng.
2. Thủ thư ghi loại sự cố, thời điểm, mô tả và minh chứng; với hỏng, đối chiếu tình trạng lúc giao.
3. Hệ thống ghi nhận sự cố, loại cuốn khỏi khả năng giao khi không dùng được. Báo mất chưa kết thúc nghĩa vụ sách.
4. Nhân viên xác định giá trị theo giá bìa; nếu thiếu thì yêu cầu định giá được duyệt và lưu căn cứ.
5. Hệ thống tính mức bồi thường: hao mòn 0%, hư nhẹ 50%, hư nặng hoặc mất 100% giá trị; lưu mức độ và giá trị đã chốt.
6. Hệ thống xác định tiền phạt riêng theo lịch, ngày thực nhận nếu đã trả hoặc quy tắc tất toán mất theo Q02.
7. Thủ thư trình bày các khoản riêng biệt và phương án hợp lệ: hư nhẹ thu tiền; mất/hư nặng nhận tiền hoặc bản thay thế được duyệt.
8. Nếu thanh toán tiền, thủ thư xác nhận thực thu đủ và hệ thống lưu chứng từ; nếu thay sách, chuyển UC09 để kiểm tra và nhận bản được duyệt.
9. Hệ thống kiểm tra các nghĩa vụ đã được giải quyết; chỉ đánh dấu hoàn tất khi đủ điều kiện, với mất sách, áp dụng Q02: chỉ tất toán khi bồi thường đã hoàn tất và tiền phạt đến ngày tất toán đã được thanh toán đủ; khấu trừ phần phạt đã thu.
10. Hệ thống lưu nhật ký và hiển thị kết quả, khoản còn phải xử lý và trạng thái cuốn/dòng mượn.

### 10.4. Luồng thay thế

- **Chỉ báo mất, chưa bồi thường:** lưu sự cố; nghĩa vụ sách và hạn mức vẫn còn, tiền phạt tiếp tục theo quy tắc. Không ghi ngày trả giả.
- **Hao mòn hoặc hỏng đã có lúc giao:** không tính bồi thường cho phần đó; phạt quá hạn nếu có vẫn xét độc lập.
- **Tìm lại trước tất toán nghĩa vụ mất sách theo Q02:** nhận qua UC05, hủy nghĩa vụ bồi thường mất, tính phạt đến ngày thực nhận; nếu hỏng thì đánh giá hỏng. Không tự hoàn một khoản đã thu khi chưa có căn cứ xử lý; các khoản đã nhận nhưng chưa tất toán phải được quản lý đối soát theo Q02 và mục 8.1 của tài liệu yêu cầu, giữ nguyên lịch sử chứng từ. Hoàn tất riêng phần bồi thường không chuyển tình huống này sang nhánh sau tất toán.
- **Tìm lại sau tất toán nghĩa vụ mất sách theo Q02:** không hoàn tiền hoặc sách đã nộp; cuốn tìm lại thuộc người đọc, hồ sơ cuốn cũ vẫn lưu và không đưa vào lưu thông.
- **Mất/hư nặng và chọn bản thay thế:** UC09 ghi biên nhận sách, không ghi thành tiền thực thu và không tự xóa khoản phạt.

### 10.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC08-E01 | Sự cố không khớp cuốn/dòng hoặc đã được xử lý | Không tạo khoản trùng; hiển thị hồ sơ đã có theo quyền. |
| UC08-E02 | Thiếu căn cứ giá, giá không hợp lệ hoặc chưa có quyền duyệt | Giữ sự cố chờ xử lý, không tự chốt bồi thường. |
| UC08-E03 | Hư nhẹ nhưng đề nghị bồi thường bằng bản sách | Từ chối phương án này, áp dụng hình thức tiền theo BR31. |
| UC08-E04 | Đã bồi thường tiền sách nhưng tiếp tục nhận bản thay thế cho cùng nghĩa vụ | Ngăn thu trùng; đối soát chứng từ hiện có. |
| UC08-E05 | Yêu cầu tất toán khi mới hoàn tất bồi thường hoặc tiền phạt chưa đủ đến ngày xử lý | Từ chối tất toán; áp dụng Q02, tính lại phạt đến ngày xử lý và trừ phần đã thu. Chỉ tất toán và giải phóng hạn mức khi đủ cả bồi thường và phạt; không từ chối chỉ vì hai nghĩa vụ được thanh toán khác ngày. |
| UC08-E06 | Thu hoặc lưu lỗi sau giao nhận thực tế | Giữ căn cứ và đối soát như UC05; không thu lại khi chưa rõ kết quả trước. |

### 10.6. Kết quả sau thực hiện

- **Ghi nhận thành công:** có biên bản, minh chứng và trạng thái xử lý rõ; các khoản có thể vẫn chưa hoàn tất.
- **Giải quyết thành công:** bồi thường có căn cứ và chứng từ; phạt được xử lý riêng; chỉ đóng nghĩa vụ sách khi đáp ứng điều kiện tất toán đã chốt.
- **Thất bại hoặc chờ duyệt:** không phát sinh khoản thu giả, không mất công nợ hoặc lịch sử; không coi việc báo mất là đã trả sách.

## 11. UC09 — Thanh lý và nhập bản thay thế

### 11.1. Mục tiêu và tác nhân

- **Mục tiêu:** xử lý cuốn cũ và ghi nhận bản thay thế đúng phê duyệt, bảo toàn lịch sử và đăng ký chờ hợp lệ.
- **Tác nhân chính:** người quản lý phê duyệt; thủ thư nhận sách và cập nhật.
- **Phối hợp:** độc giả nộp bản thay thế, người trong hàng chờ chọn tiếp tục hoặc hủy.
- **Đầu vào:** sự cố, mã cuốn cũ, phương án duyệt, thông tin bản thay thế, vị trí và lựa chọn của người chờ.
- **Quy tắc:** BR28, BR31–BR32, BR36, NFR01–NFR04; hoàn tất nghĩa vụ mất phụ thuộc Q02.

### 11.2. Điều kiện trước

1. Có hồ sơ sự cố hoặc căn cứ xử lý cuốn cũ được xác định.
2. Người phê duyệt và người cập nhật có quyền tương ứng.
3. Bản thay thế phải được kiểm tra thực tế trước khi xác nhận đã nhận.

### 11.3. Luồng chính — Nhận bản thay thế đã duyệt

1. Thủ thư mở hồ sơ sự cố và kiểm tra phương án bồi thường chưa hoàn tất bằng hình thức khác.
2. Thủ thư đối chiếu bản nộp với tên sách, tác giả, ngôn ngữ, phiên bản và tình trạng có thể sử dụng.
3. Người quản lý phê duyệt tính tương đương và lưu căn cứ.
4. Nếu cuốn cũ hư nặng, ghi thanh lý và bỏ vị trí lưu thông. Nếu cuốn cũ bị mất, giữ đúng lịch sử mất, không giả nhận sách để thanh lý vật lý.
5. Thủ thư nhập bản mới với mã riêng, liên kết đầu sách phù hợp và vị trí kệ còn hợp lệ; không tái sử dụng mã cuốn cũ.
6. Hệ thống lập biên nhận thay thế, liên kết sự cố, cuốn cũ, cuốn mới, người duyệt và người nhận.
7. Hệ thống chuyển các đăng ký còn hiệu lực của người đã chọn tiếp tục sang cuốn mới, giữ thứ tự và liên kết lịch sử/dòng nguồn; không chuyển người đã hủy hoặc đã kết thúc đăng ký.
8. Khi bản mới đã kiểm tra và có thể giao, yêu cầu UC07 xét quyền nhận; không tự cấp quyền trước khi xét điều kiện.
9. Hệ thống cập nhật tiến độ bồi thường, giữ khoản phạt độc lập và trả kết quả.

### 11.4. Luồng thay thế

- **Chỉ thanh lý cuốn hư nặng, chưa có bản mới:** ghi quyết định và đưa cuốn khỏi lưu thông; hàng chờ được thông báo/tạm dừng theo BR28, không tự tạo cuốn thay thế.
- **Cuốn chỉ hư nhẹ:** chuyển sửa chữa, không tự thanh lý hoặc yêu cầu bản thay thế.
- **Người chờ chọn hủy:** xử lý UC06, giữ lịch sử và không chuyển đăng ký đó.
- **Vị trí cũ không còn phù hợp:** chọn vị trí khác hợp lệ; không coi việc đổi kệ là đổi danh tính cuốn.

### 11.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC09-E01 | Bản không tương đương, không dùng được hoặc chưa được duyệt | Không xác nhận biên nhận thay thế thành công; yêu cầu bổ sung/đổi phương án hợp lệ. |
| UC09-E02 | Mã bản mới trùng mã đã có hoặc tái sử dụng mã cũ | Từ chối nhập với mã đó. |
| UC09-E03 | Nghĩa vụ đã được bồi thường đủ bằng tiền | Không nhận thêm bản như một nghĩa vụ bắt buộc của cùng sự cố. |
| UC09-E04 | Chuyển nhầm đăng ký đã hủy/kết thúc hoặc đảo thứ tự | Từ chối thay đổi không hợp lệ và đọc lại dữ liệu hàng chờ. |
| UC09-E05 | Lỗi lưu sau khi đã nhận bản vật lý | Đối soát và lưu bằng chứng giao nhận; không tự nhận lần hai. Không ghi dữ liệu một phần làm mất liên kết. |

### 11.6. Kết quả sau thực hiện

- **Thành công:** cuốn cũ không lưu thông khi đã thanh lý/mất; bản mới có mã riêng và biên nhận; đăng ký hợp lệ được chuyển đúng thứ tự; phạt không bị xóa.
- **Chưa hoàn tất tài chính:** biên nhận thay thế không đồng nghĩa mọi nghĩa vụ đã tất toán; xử lý phần còn lại tại UC08 theo Q02.
- **Thất bại:** không nhập bản không được duyệt, không tạo chứng từ trùng hoặc xóa lịch sử cuốn cũ.

## 12. UC10 — Xác nhận nghĩa vụ và đóng hồ sơ

### 12.1. Mục tiêu và tác nhân

- **Mục tiêu:** xác nhận nghĩa vụ thư viện và kết thúc quyền sử dụng đúng điều kiện.
- **Tác nhân chính:** người quản lý; thủ thư có thể cung cấp dữ liệu đối soát.
- **Phối hợp:** độc giả hoặc thông tin rời trường có căn cứ từ nhà trường.
- **Đầu vào:** mã độc giả, lý do, loại yêu cầu xác nhận/đóng hồ sơ/dừng quyền do buộc rời trường.
- **Quy tắc:** BR07–BR09, BR27, NFR04–NFR05; việc ẩn danh phụ thuộc Q04.

### 12.2. Điều kiện trước

1. Người quản lý có quyền xử lý và xác định đúng hồ sơ.
2. Có yêu cầu kết thúc sử dụng hoặc căn cứ thay đổi tư cách.
3. Có thể đối soát tất cả phiếu, nghĩa vụ sách, sự cố và khoản chưa thanh toán của độc giả.

### 12.3. Luồng chính — Xác nhận và đóng hồ sơ khi đủ điều kiện

1. Người quản lý chọn độc giả và loại yêu cầu.
2. Hệ thống tổng hợp nghĩa vụ sách trên mọi phiếu, sự cố chưa xử lý xong và khoản phải thu chưa thanh toán.
3. Hệ thống kiểm tra không còn nghĩa vụ sách và nợ; không coi báo mất đơn thuần là hết sách.
4. Người quản lý xem kết quả đối soát và xác nhận hoàn tất nghĩa vụ thư viện.
5. Nếu có yêu cầu đóng hồ sơ, hệ thống vô hiệu hóa thẻ/tài khoản, kết thúc quyền sử dụng mới và xử lý đăng ký chờ theo trạng thái phù hợp.
6. Hệ thống lưu lý do, người xác nhận, thời điểm và lịch sử; trả kết quả.

### 12.4. Luồng thay thế

- **Chỉ xác nhận nghĩa vụ:** trả xác nhận, không tự đóng hồ sơ nếu không có yêu cầu tương ứng.
- **Còn sách hoặc nợ:** trả danh sách cần giải quyết, chưa xác nhận hoàn tất; độc giả tiếp tục trả/thanh toán qua UC05 hoặc UC08.
- **Bị buộc rời trường:** dừng quyền mượn, gia hạn, xếp hàng mới và hủy đăng ký phù hợp ngay theo căn cứ; giữ hạn trả cũ, lịch sử và nghĩa vụ. Chưa đóng hồ sơ như đã hết nghĩa vụ nếu vẫn còn sách/nợ.
- **Hồ sơ đã đóng:** trả trạng thái hiện tại; không tạo lần đóng mới làm mất lịch sử.

### 12.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC10-E01 | Người thao tác không có quyền hoặc căn cứ rời trường không rõ | Không thay đổi tư cách; yêu cầu kiểm tra căn cứ. |
| UC10-E02 | Phát sinh nghĩa vụ mới trước khi xác nhận | Đối soát lại; không xác nhận hết nghĩa vụ trên dữ liệu cũ. |
| UC10-E03 | Yêu cầu xóa phiếu, khoản thu hoặc lịch sử để đóng hồ sơ | Từ chối; bảo toàn chứng từ và nghĩa vụ. |
| UC10-E04 | Yêu cầu xóa hoặc ẩn danh hồ sơ trong phiên bản bài tập | Từ chối vì ngoài phạm vi theo Q04; vẫn có thể vô hiệu hóa quyền và đóng hồ sơ đúng điều kiện, giữ lịch sử. |

### 12.6. Kết quả sau thực hiện

- **Thành công:** có xác nhận nghĩa vụ và/hoặc hồ sơ bị đóng theo đúng yêu cầu; lịch sử được giữ.
- **Nhánh dừng quyền:** độc giả không có quyền sử dụng mới nhưng hạn trả và khoản đang có vẫn tồn tại.
- **Chưa đủ điều kiện:** không cấp xác nhận hoàn tất sai và không xóa công nợ; có danh sách việc cần giải quyết.

## 13. UC11 — Quản lý đầu sách, cuốn sách và vị trí

### 13.1. Mục tiêu và tác nhân

- **Mục tiêu:** tạo và duy trì danh mục đủ chính xác để phục vụ tra cứu và giao nhận từng cuốn.
- **Tác nhân chính:** thủ thư; người quản lý phê duyệt định giá hoặc thao tác thuộc quyền quản lý.
- **Đầu vào:** thông tin ấn bản, mã cuốn, tình trạng, giá bìa/định giá và vị trí kệ.
- **Quy tắc:** BR12, BR30, BR36, NFR01, NFR04–NFR05. Đây là Use Case bổ sung đã có trong tài liệu yêu cầu.

### 13.2. Điều kiện trước

1. Người thao tác được xác thực và có quyền quản lý phần danh mục tương ứng.
2. Có thông tin sách/bản vật lý cần nhập hoặc đối tượng hiện có cần sửa.
3. Định giá thay cho giá bìa phải có căn cứ và quyền phê duyệt.

### 13.3. Luồng chính — Thêm đầu sách và cuốn

1. Thủ thư tra cứu đầu sách/ấn bản hiện có trước khi tạo mới.
2. Nếu chưa có ấn bản phù hợp, nhập tên, tác giả, ngôn ngữ, phiên bản và các thông tin thư mục cần thiết; hệ thống kiểm tra dữ liệu.
3. Thủ thư chọn đầu sách rồi nhập từng cuốn vật lý với mã riêng, tình trạng và giá trị có căn cứ nếu có.
4. Thủ thư chọn vị trí kệ hợp lệ; có thể chưa gán vị trí nếu cuốn chưa được xếp kệ, nhưng phải thể hiện rõ trạng thái này.
5. Hệ thống kiểm tra mã cuốn không trùng, liên kết đầu sách/vị trí hợp lệ và giá không âm. Nếu thiếu giá bìa thì không tự đặt giá; đánh dấu cần định giá khi nghiệp vụ yêu cầu.
6. Thủ thư kiểm tra và xác nhận.
7. Hệ thống lưu đầu sách, cuốn, vị trí và nhật ký; cuốn chỉ được coi là sẵn sàng khi đã kiểm tra và đủ điều kiện giao.
8. Hệ thống hiển thị thông tin vừa cập nhật để tra cứu.

### 13.4. Luồng thay thế

- **Thêm bản cho đầu sách đã có:** dùng đầu sách hiện tại, tạo mã cuốn mới; không gộp lịch sử giữa các bản.
- **Sửa thông tin thư mục:** cập nhật thông tin được phép, giữ định danh và lịch sử giao dịch. Thay giá danh mục không sửa ngược giá bồi thường đã chốt trong biên bản.
- **Chuyển kệ hoặc quản lý vị trí:** kiểm tra vị trí mới; bỏ vị trí không xóa cuốn và phải xử lý liên kết còn sử dụng.
- **Thanh lý hoặc nhập bản bồi thường:** thực hiện UC09 để có phê duyệt/chứng từ, không dùng sửa danh mục để bỏ qua quy trình.

### 13.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC11-E01 | Thiếu thông tin bắt buộc, mã trùng, giá âm hoặc liên kết không hợp lệ | Yêu cầu sửa; không lưu bản ghi dở. |
| UC11-E02 | Tự chuyển cuốn đang mượn/giữ chỗ thành sẵn sàng bằng sửa danh mục | Từ chối; trạng thái lưu thông phải thay qua nghiệp vụ tương ứng. |
| UC11-E03 | Sửa giá nhằm thay đổi nghĩa vụ đã chốt hoặc xóa cuốn có lịch sử | Không cho làm mất căn cứ và lịch sử; chuyển đúng quy trình xử lý. |
| UC11-E04 | Định giá không có căn cứ hoặc chưa được người có quyền duyệt | Không công nhận giá định lại làm căn cứ bồi thường. |

### 13.6. Kết quả sau thực hiện

- **Thành công:** danh mục có đầu sách, cuốn và vị trí đúng liên kết; mỗi cuốn có mã riêng; dữ liệu tra cứu phản ánh khả dụng thực tế.
- **Thất bại:** danh mục không có bản ghi dở hoặc liên kết sai; lịch sử lưu thông và các khoản đã chốt không bị thay đổi.

## 14. UC12 — Tra cứu và thống kê

### 14.1. Mục tiêu và tác nhân

- **Mục tiêu:** cung cấp thông tin sách, nghĩa vụ và thống kê đúng phạm vi truy cập.
- **Tác nhân chính:** độc giả, thủ thư, người quản lý.
- **Đầu vào:** từ khóa tên/tác giả/mã, bộ lọc, mã độc giả khi được phép, khoảng thời gian báo cáo.
- **Quy tắc:** BR12, BR20, BR29–BR32, NFR04–NFR05. Đây là Use Case bổ sung đã có trong tài liệu yêu cầu.

### 14.2. Điều kiện trước

1. Người dùng đã được xác thực theo phạm vi của phiên bản bài tập.
2. Hệ thống xác định được quyền xem danh mục, hồ sơ cá nhân hoặc thống kê tổng hợp.
3. Có thể đọc dữ liệu danh mục và giao dịch liên quan; bộ lọc đầu vào hợp lệ.

### 14.3. Luồng chính — Tra cứu sách

1. Người dùng chọn tra cứu, nhập từ khóa hoặc bộ lọc.
2. Hệ thống kiểm tra dữ liệu tìm kiếm và phạm vi quyền.
3. Hệ thống tìm đầu sách/cuốn phù hợp theo tên, tác giả hoặc mã.
4. Hệ thống tổng hợp khả dụng từ trạng thái từng cuốn; không tính cuốn đang mượn, sửa, mất, thanh lý hoặc giữ cho người khác là sẵn sàng cho mọi người.
5. Hệ thống hiển thị danh sách và chi tiết được phép: thông tin đầu sách, cuốn, vị trí, tình trạng khả dụng.
6. Người dùng có thể chọn xem chi tiết hoặc bắt đầu yêu cầu khác; tra cứu tự nó không tạo đăng ký chờ hay phiếu mượn.

### 14.4. Luồng thay thế

- **Xem nghĩa vụ cá nhân:** hiển thị thẻ, dòng đang mượn, hạn trả, lịch sử, đăng ký chờ và khoản của chính độc giả; thẻ hết hạn/còn nợ không chặn việc xem nghĩa vụ.
- **Nhân viên xem nghĩa vụ độc giả:** phải có quyền tương ứng, sử dụng dữ liệu để đối soát và hỗ trợ xử lý.
- **Thống kê:** nhân viên được phép chọn khoảng ngày/bộ lọc, xem đang mượn, quá hạn, khoản chưa thu và số thực thu. Mỗi chỉ tiêu phải phân biệt dự kiến, phải thu và đã thu, truy vết được về dữ liệu gốc.
- **Không có kết quả:** trả danh sách rỗng hoặc số tổng hợp bằng 0 phù hợp, không coi là lỗi hệ thống.

### 14.5. Trường hợp lỗi

| Mã | Tình huống | Xử lý |
| --- | --- | --- |
| UC12-E01 | Độc giả truy cập hồ sơ người khác hoặc báo cáo ngoài quyền | Từ chối, không trả dữ liệu nhạy cảm. |
| UC12-E02 | Bộ lọc sai hoặc ngày bắt đầu sau ngày kết thúc | Yêu cầu sửa trước khi truy vấn. |
| UC12-E03 | Không đọc được dữ liệu | Báo lỗi rõ; không hiển thị dữ liệu lỗi như một kết quả rỗng thành công. |
| UC12-E04 | Dữ liệu khả dụng thay đổi sau khi hiển thị | Khi người dùng chuyển sang mượn/chờ, Use Case đích kiểm tra lại; kết quả tra cứu không bảo đảm quyền nhận. |

### 14.6. Kết quả sau thực hiện

- **Thành công:** người dùng nhận thông tin đúng quyền và bộ lọc; khoản chưa chốt không bị ghi là đã thu. Tiền phạt dự kiến phải được ghi rõ là dự kiến.
- **Thất bại:** không tiết lộ dữ liệu ngoài quyền và không làm thay đổi giao dịch nghiệp vụ.
- Đây là Use Case đọc dữ liệu; không tự gia hạn, thu tiền, tạo hàng chờ hoặc đổi trạng thái cuốn.

## 15. Liên hệ giữa các Use Case

Các liên hệ dưới đây mô tả phối hợp nghiệp vụ:

| Use Case | Liên hệ |
| --- | --- |
| UC01 và UC02 | Dùng chung hồ sơ/thẻ; cấp lại không thay thế quy trình khôi phục hiệu lực. |
| UC03 và UC07 | Giao sách phải tôn trọng quyền nhận; giao thành công cập nhật đợt và kiểm tra lại người tiếp tục chờ. |
| UC04 và UC06 | Đăng ký chờ hiệu lực là điều kiện chặn gia hạn mượn. |
| UC05 và UC07 | Sách trả dùng được được xét ưu tiên trước khi cho mượn tiếp. |
| UC05 và UC08 | Hư hỏng khi trả được đánh giá theo sự cố; ngày nhận thật là căn cứ phạt cho sách đã nhận. |
| UC06 và UC07 | Đăng ký/hủy ảnh hưởng lượt giữ chỗ; đăng ký thành công chưa phải quyền nhận. |
| UC08 và UC09 | Phương án bồi thường bằng sách cần phê duyệt và biên nhận; tiền phạt vẫn độc lập. |
| UC09 và UC07 | Bản thay thế chỉ được xét giữ chỗ khi có thể giao, với đăng ký được chuyển hợp lệ. |
| UC10 với UC05 và UC08 | Chỉ xác nhận hết nghĩa vụ sau khi sách và các khoản đã giải quyết đúng điều kiện. |
| UC11 và UC12 | Danh mục là nguồn tra cứu; sửa danh mục không được bỏ qua giao dịch lưu thông. |

## 16. Quy ước nghiệp vụ và phạm vi kiểm thử

| Mã | Quy ước đã thống nhất | Use Case liên quan | Nội dung cần kiểm thử |
| --- | --- | --- | --- |
| Q01 | Ghi nhận ngày nhận sách thực tế, tách việc nhận trả và việc thanh toán. Tiền phạt quá hạn của cuốn đã trả dừng tại ngày nhận thực tế; khoản chưa thanh toán tiếp tục được theo dõi. | UC05, UC08; kiểm tra hạn chế quyền tại UC02, UC03, UC04, UC06 | Nhận sách thành công nhưng thu tiền thất bại; không tăng phạt quá hạn sau ngày nhận sách; không ghi nhận đã thanh toán khi chưa xác nhận thu tiền; khoản nợ chặn gia hạn hoặc khôi phục thẻ, mượn mới, gia hạn mượn và đăng ký hàng chờ mới. Nếu đã nhận tiền nhưng lưu thất bại, phải đối soát trước khi thu lại. |
| Q02 | Nghĩa vụ mất sách chỉ được tất toán khi hoàn thành bồi thường và thanh toán đủ tiền phạt quá hạn đến ngày tất toán. Báo mất chưa giải phóng hạn mức mượn. | UC08, UC09, UC10; kiểm tra hạn mức tại UC03 | Bồi thường và trả tiền phạt cùng ngày hoặc khác ngày; tính phạt đến ngày hoàn thành đủ nghĩa vụ; trừ số tiền phạt đã thu để tránh thu trùng; chỉ giải phóng hạn mức khi tất toán; xử lý tìm lại sách trước và sau tất toán theo BR33–BR34. |
| Q03 | Thư viện mở từ thứ Hai đến thứ Bảy, đóng cửa lúc 17:00 theo giờ Việt Nam; nghỉ Chủ nhật và các ngày nghỉ bổ sung trong lịch thư viện. | UC01, UC02, UC03, UC04, UC05, UC06, UC07, UC08 | Cộng ngày mở cửa không tính ngày bắt đầu và ngày nghỉ; lượt giữ chỗ hết hạn lúc 17:00 ngày mở cửa thứ hai sau ngày cấp lượt; lượt tiếp theo có thời hạn riêng. Hiệu lực thẻ 6 tháng và cửa sổ gia hạn 30 ngày tính theo lịch. Cập nhật lịch không tự thay đổi hạn trả hoặc hạn giữ chỗ đã lưu; tính phạt loại trừ ngày đóng cửa theo lịch đã xác nhận. |
| Q04 | Trong phạm vi bài tập, không tự động xóa hoặc ẩn danh hồ sơ. Đóng hồ sơ phải vô hiệu hóa quyền sử dụng và giữ lại lịch sử nghiệp vụ. | UC10; kiểm tra quyền truy cập tại các Use Case liên quan | Không cho đóng hồ sơ khi còn nghĩa vụ; sau khi đóng không được thực hiện giao dịch mới; vẫn giữ lịch sử mượn, trả, gia hạn, hàng chờ, sự cố và thanh toán; chỉ người có quyền được xem dữ liệu. |

### 16.1. Kiểm thử yêu cầu mượn nhiều cuốn

Áp dụng quy ước xử lý thống nhất của UC03:

- Khi tất cả cuốn sách và điều kiện của độc giả đều hợp lệ, tạo phiếu mượn cho toàn bộ danh sách đã xác nhận.
- Khi có ít nhất một cuốn không hợp lệ, từ chối toàn bộ yêu cầu, thông báo rõ cuốn sách và lý do; không tạo phiếu hoặc cập nhật trạng thái mượn cho một phần danh sách.
- Khi thủ thư sửa danh sách và gửi lại, kiểm tra lại toàn bộ điều kiện trước khi lưu.
- Khi lưu thất bại hoặc gửi lặp yêu cầu, không để phát sinh phiếu trùng, tăng hạn mức đã sử dụng sai hoặc lệch trạng thái sách.

### 16.2. Yêu cầu chung đối với kiểm thử

- Mỗi Use Case có ít nhất một trường hợp thành công và một trường hợp bị từ chối phù hợp.
- Kiểm tra các luồng thay thế, trường hợp lỗi và điều kiện biên được mô tả trong từng Use Case.
- Kiểm tra quyền của tác nhân, tính hợp lệ của dữ liệu và trạng thái trước, sau giao dịch.
- Kiểm tra gửi lặp yêu cầu, thay đổi dữ liệu đồng thời và lỗi lưu dữ liệu đối với các giao dịch có liên quan.
- Các ca kiểm thử liên quan đến Q01–Q04 phải xác định kết quả mong đợi theo bảng quy ước trên.
- Mỗi ca kiểm thử ghi rõ dữ liệu đầu vào, điều kiện ban đầu, các bước thực hiện, kết quả mong đợi, kết quả thực tế và trạng thái đạt hoặc không đạt.
- Các ca chưa chạy phải ghi “Chưa thực hiện”. Chỉ ghi “Đạt” sau khi chạy chương trình và đối chiếu kết quả, dữ liệu thực tế với kết quả mong đợi.