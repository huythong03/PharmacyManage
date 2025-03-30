# PharmacyWeb
**PharmacyWeb** là một nền tảng quản lý y tế trực tuyến, được thiết kế để cung cấp trải nghiệm người dùng hiện đại và thân thiện. Dự án sử dụng ASP.NET Core để xây dựng một giao diện web đẹp mắt, phù hợp cho các ứng dụng liên quan đến y tế và dược phẩm.

## 📋 Tổng quan

PharmacyWeb được phát triển với mục tiêu tạo ra một trang web quản lý y tế trực tuyến, nơi người dùng có thể đăng nhập, đăng ký, quản lý thông tin tài khoản, và tương tác với hệ thống thông qua giao diện quản lý (Manage Pages). Dự án sử dụng cơ sở dữ liệu SQL Server để lưu trữ thông tin người dùng, và hỗ trợ các chức năng quên mật khẩu, xác nhận email, và đổi mật khẩu.

### Các tính năng chính
- **Giao diện người dùng hiện đại**: Sử dụng gradient màu xanh dương (`#4f46e5` đến `#3b82f6`) và font `Inter` để tạo cảm giác chuyên nghiệp.
- **Hiệu ứng Particles**: Nền động với các chấm kết nối thành mạng lưới, có hiệu ứng xoáy nhẹ (orbit) nhờ `particles-config.js`.
- **Quản lý tài khoản**: Hỗ trợ đăng nhập, đăng ký, quên mật khẩu, và xác nhận quên mật khẩu.
- **Sidebar quản lý**: Giao diện sidebar đẹp mắt để quản lý các tác vụ (Manage Pages).
- **Widget chat**: Tích hợp widget chat cố định ở góc dưới bên phải, giúp người dùng liên hệ nhanh.
- **Responsive**: Tối ưu giao diện cho cả desktop và mobile.

## 🛠️ Công nghệ sử dụng
- **ASP.NET Core**: Framework chính để xây dựng backend và giao diện.
- **SQL Server**: Cơ sở dữ liệu để lưu trữ và quản lý dữ liệu người dùng.
- **Bootstrap**: Framework CSS để tạo giao diện responsive.
- **Particles.js**: Thư viện tạo hiệu ứng particles động.
- **CSS/SCSS**: Tùy chỉnh giao diện với `site.css`.
- **JavaScript**: Tùy chỉnh particles với `particles-config.js`.

## 🚀 Hướng dẫn cài đặt

### Yêu cầu
- **.NET SDK** (phiên bản 8.0 hoặc mới hơn)
- **SQL Server** (phiên bản 2019 hoặc mới hơn) và **SQL Server Management Studio (SSMS)** (để quản lý cơ sở dữ liệu)
- **Visual Studio** (hoặc bất kỳ IDE hỗ trợ ASP.NET Core)

### Các bước cài đặt
1. **Clone repository**:
   ```bash
   git clone https://github.com/huythong03/PharmacyManage.git
   cd PharmacyManage
