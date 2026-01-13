-- =====================================================
-- Script SQL để thêm dữ liệu mẫu cho CPhoneS
-- Database: CPhoneS
-- =====================================================

USE [CPhoneS]
GO

-- Xóa dữ liệu cũ (tùy chọn - chỉ dùng khi cần reset dữ liệu)
-- DELETE FROM ChiTietDonHang
-- DELETE FROM OrderDetails
-- DELETE FROM DonHang
-- DELETE FROM Orders
-- DELETE FROM SanPham
-- DELETE FROM DanhMucSP
-- DELETE FROM KhachHang
-- DELETE FROM ApplicationUser
-- DELETE FROM Account
-- GO

-- =====================================================
-- 1. INSERT DANH MỤC SẢN PHẨM (DanhMucSP)
-- =====================================================
SET IDENTITY_INSERT DanhMucSP ON
GO

INSERT INTO DanhMucSP (MaDM, TenDM, AnhDM, MoTaDM, TrangThai)
VALUES
    (1, N'Điện thoại thông minh', N'/images/category/smartphone.jpg', N'Các dòng điện thoại thông minh hiện đại', 1),
    (2, N'iPhone', N'/images/category/iphone.jpg', N'Điện thoại iPhone chính hãng Apple', 1),
    (3, N'Samsung', N'/images/category/samsung.jpg', N'Điện thoại Samsung Galaxy', 1),
    (4, N'Xiaomi', N'/images/category/xiaomi.jpg', N'Điện thoại Xiaomi giá rẻ', 1),
    (5, N'Oppo', N'/images/category/oppo.jpg', N'Điện thoại Oppo camera đẹp', 1),
    (6, N'Phụ kiện', N'/images/category/accessories.jpg', N'Các phụ kiện điện thoại', 1)

SET IDENTITY_INSERT DanhMucSP OFF
GO

-- =====================================================
-- 2. INSERT SẢN PHẨM (SanPham)
-- =====================================================
SET IDENTITY_INSERT SanPham ON
GO

INSERT INTO SanPham (MaSP, MaDM, TenSP, AnhSP, VideoSP, ThuongHieu, GiaSP, TrangThai, BestSeller, CreateDate, MotaSP)
VALUES
    -- iPhone
    (1, 2, N'iPhone 15 Pro Max 256GB', N'/images/products/iphone-15-pro-max.jpg', N'/videos/iphone-15-pro-max.mp4', N'Apple', 29990000, 1, 1, '2024-01-15', N'iPhone 15 Pro Max với chip A17 Pro, camera 48MP, pin lâu bền'),
    (2, 2, N'iPhone 15 Pro 128GB', N'/images/products/iphone-15-pro.jpg', NULL, N'Apple', 25990000, 1, 1, '2024-01-15', N'iPhone 15 Pro thiết kế titanium, hiệu năng mạnh mẽ'),
    (3, 2, N'iPhone 14 128GB', N'/images/products/iphone-14.jpg', NULL, N'Apple', 19990000, 1, 0, '2023-09-20', N'iPhone 14 với chip A15 Bionic, camera 12MP'),
    
    -- Samsung
    (4, 3, N'Samsung Galaxy S24 Ultra 512GB', N'/images/products/galaxy-s24-ultra.jpg', NULL, N'Samsung', 27990000, 1, 1, '2024-01-20', N'Galaxy S24 Ultra với S Pen, camera 200MP'),
    (5, 3, N'Samsung Galaxy S23 256GB', N'/images/products/galaxy-s23.jpg', NULL, N'Samsung', 18990000, 1, 0, '2023-02-15', N'Galaxy S23 thiết kế đẹp, hiệu năng ổn định'),
    (6, 3, N'Samsung Galaxy A54 128GB', N'/images/products/galaxy-a54.jpg', NULL, N'Samsung', 8990000, 1, 1, '2023-03-10', N'Galaxy A54 giá rẻ, camera 50MP'),
    
    -- Xiaomi
    (7, 4, N'Xiaomi 14 Pro 512GB', N'/images/products/xiaomi-14-pro.jpg', NULL, N'Xiaomi', 21990000, 1, 1, '2024-01-10', N'Xiaomi 14 Pro camera Leica, Snapdragon 8 Gen 3'),
    (8, 4, N'Xiaomi Redmi Note 13 Pro 256GB', N'/images/products/redmi-note-13-pro.jpg', NULL, N'Xiaomi', 6990000, 1, 1, '2024-01-05', N'Redmi Note 13 Pro pin 5100mAh, sạc nhanh 67W'),
    (9, 4, N'Xiaomi 13T 256GB', N'/images/products/xiaomi-13t.jpg', NULL, N'Xiaomi', 12990000, 1, 0, '2023-10-15', N'Xiaomi 13T camera 50MP, chip Dimensity 8200'),
    
    -- Oppo
    (10, 5, N'Oppo Find X7 Ultra 512GB', N'/images/products/oppo-find-x7.jpg', NULL, N'Oppo', 24990000, 1, 0, '2024-01-25', N'Oppo Find X7 Ultra camera 50MP, thiết kế cao cấp'),
    (11, 5, N'Oppo Reno 11 Pro 256GB', N'/images/products/oppo-reno-11.jpg', NULL, N'Oppo', 13990000, 1, 1, '2024-01-12', N'Reno 11 Pro camera selfie 32MP, sạc nhanh 80W'),
    (12, 5, N'Oppo A98 256GB', N'/images/products/oppo-a98.jpg', NULL, N'Oppo', 6990000, 1, 0, '2023-11-20', N'Oppo A98 pin 5000mAh, màn hình 120Hz'),
    
    -- Điện thoại thông minh khác
    (13, 1, N'Google Pixel 8 Pro 256GB', N'/images/products/pixel-8-pro.jpg', NULL, N'Google', 23990000, 1, 0, '2023-10-10', N'Pixel 8 Pro camera AI, Android thuần'),
    (14, 1, N'OnePlus 12 256GB', N'/images/products/oneplus-12.jpg', NULL, N'OnePlus', 19990000, 1, 1, '2024-01-18', N'OnePlus 12 sạc nhanh 100W, màn hình AMOLED'),
    (15, 1, N'Nothing Phone 2 256GB', N'/images/products/nothing-phone-2.jpg', NULL, N'Nothing', 15990000, 1, 0, '2023-07-25', N'Nothing Phone 2 thiết kế độc đáo, Glyph Interface')

SET IDENTITY_INSERT SanPham OFF
GO

-- =====================================================
-- 3. INSERT APPLICATION USER (Người dùng - dùng BCrypt)
-- Lưu ý: PasswordHash đã được hash bằng BCrypt từ mật khẩu "123456"
-- =====================================================
SET IDENTITY_INSERT ApplicationUser ON
GO

INSERT INTO ApplicationUser (Name, UserName, Email, PasswordHash, FullName)
VALUES
    (N'Nguyễn Văn Admin', N'admin', N'admin@cphones.com', N'$$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu', N'Nguyễn Văn Admin'),
    (N'Trần Thị Hương', N'huong', N'huong@example.com', N'$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu', N'Trần Thị Hương'),
    (N'Lê Văn Nam', N'nam', N'nam@example.com', N'$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu', N'Lê Văn Nam'),
    (N'Phạm Thị Mai', N'mai', N'mai@example.com', N'$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu', N'Phạm Thị Mai'),
    (N'Hoàng Văn Đức', N'duc', N'duc@example.com', N'$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu', N'Hoàng Văn Đức')

SET IDENTITY_INSERT ApplicationUser OFF
GO

INSERT INTO ApplicationUser
(UserName, PasswordHash)
VALUES
('Hoàng Văn Đức', '$2a$11$JCdvYuhM/NWGGr.sZsB3y.ZGaqHpDhhIz04YYlDekmzIt22.MszEu');

-- LƯU Ý QUAN TRỌNG VỀ BCrypt Hash:
-- ApplicationUser sử dụng BCrypt để hash mật khẩu
-- Hash trong file này chỉ là placeholder - KHÔNG SỬ DỤNG TRỰC TIẾP
-- 
-- CÁCH 1: Đăng ký tài khoản qua giao diện web (ĐƯỢC KHUYẾN NGHỊ)
--        Truy cập /Account/Register để tạo tài khoản mới
--
-- CÁCH 2: Tạo hash bằng C# code (nếu cần insert trực tiếp vào DB)
--        string hash = BCrypt.Net.BCrypt.HashPassword("123456");
--        Sau đó copy hash vào PasswordHash
--
-- CÁCH 3: Comment phần INSERT ApplicationUser và chỉ insert dữ liệu khác
--        Người dùng sẽ tự đăng ký qua web

-- =====================================================
-- 4. INSERT ACCOUNT (Tài khoản hệ thống)
-- =====================================================
SET IDENTITY_INSERT Account ON
GO

INSERT INTO Account (AccountID, TaiKhoan, MatKhau, Phone, Email, FullName, CreateDate)
VALUES
    (1, N'admin01', N'admin123', 901234567, N'admin01@cphones.com', N'Quản trị viên', '2024-01-01'),
    (2, N'user01', N'user123', 902345678, N'user01@example.com', N'Người dùng 01', '2024-01-05'),
    (3, N'user02', N'user123', 903456789, N'user02@example.com', N'Người dùng 02', '2024-01-10')

SET IDENTITY_INSERT Account OFF
GO

-- =====================================================
-- 5. INSERT KHÁCH HÀNG (KhachHang)
-- =====================================================
SET IDENTITY_INSERT KhachHang ON
GO

INSERT INTO KhachHang (MaKH, TenKH, Diachi, Ngaysinh, Phone, Email, CreateDate)
VALUES
    (1, N'Nguyễn Văn An', N'123 Đường Nguyễn Văn Linh, Quận 7, TP.HCM', '1990-05-15', 912345678, N'nguyenvanan@email.com', '2024-01-10'),
    (2, N'Trần Thị Bình', N'456 Đường Lê Lợi, Quận 1, TP.HCM', '1995-08-20', 923456789, N'tranthibinh@email.com', '2024-01-15'),
    (3, N'Lê Văn Cường', N'789 Đường Võ Văn Tần, Quận 3, TP.HCM', '1988-12-03', 934567890, N'levancuong@email.com', '2024-01-20'),
    (4, N'Phạm Thị Dung', N'321 Đường Cách Mạng Tháng 8, Quận 10, TP.HCM', '1992-03-25', 945678901, N'phamthidung@email.com', '2024-02-01'),
    (5, N'Hoàng Văn Em', N'654 Đường Nguyễn Trãi, Quận 5, TP.HCM', '1997-07-10', 956789012, N'hoangvanem@email.com', '2024-02-05')

SET IDENTITY_INSERT KhachHang OFF
GO

-- =====================================================
-- 6. INSERT ĐƠN HÀNG (Orders - Hệ thống mới)
-- =====================================================
SET IDENTITY_INSERT Orders ON
GO

INSERT INTO Orders (OrderId, CustomerName, CustomerAddress, TotalAmount, OrderDate, PaymentMethod, Status)
VALUES
    (1, N'Nguyễn Văn An', N'123 Đường Nguyễn Văn Linh, Quận 7, TP.HCM', 6990000, '2024-02-10 10:30:00', N'Thanh toán khi nhận hàng', N'Pending'),
    (2, N'Trần Thị Bình', N'456 Đường Lê Lợi, Quận 1, TP.HCM', 25990000, '2024-02-12 14:20:00', N'Chuyển khoản', N'Completed'),
    (3, N'Lê Văn Cường', N'789 Đường Võ Văn Tần, Quận 3, TP.HCM', 13990000, '2024-02-15 09:15:00', N'Thanh toán khi nhận hàng', N'Pending')

SET IDENTITY_INSERT Orders OFF
GO

-- =====================================================
-- 7. INSERT CHI TIẾT ĐƠN HÀNG (OrderDetails)
-- =====================================================
SET IDENTITY_INSERT OrderDetails ON
GO

INSERT INTO OrderDetails (OrderDetailId, OrderId, ProductId, Quantity, Price)
VALUES
    -- Đơn hàng 1
    (1, 1, 8, 1, 6990000),  -- Xiaomi Redmi Note 13 Pro
    
    -- Đơn hàng 2
    (2, 2, 2, 1, 25990000), -- iPhone 15 Pro
    
    -- Đơn hàng 3
    (3, 3, 11, 1, 13990000) -- Oppo Reno 11 Pro

SET IDENTITY_INSERT OrderDetails OFF
GO

-- =====================================================
-- 8. INSERT ĐƠN HÀNG CŨ (DonHang - Hệ thống cũ) - Tùy chọn
-- =====================================================
-- SET IDENTITY_INSERT DonHang ON
-- GO

-- INSERT INTO DonHang (MaDH, MaKH, NgayTao, TrangThaiHuyDon, ThanhToan, NgayThanhToan, Note)
-- VALUES
--     (1, 1, '2024-01-20', 0, 1, '2024-01-21', N'Giao hàng nhanh'),
--     (2, 2, '2024-01-25', 0, 0, '2024-01-01', N'Đang chờ xử lý')

-- SET IDENTITY_INSERT DonHang OFF
-- GO

-- =====================================================
-- 9. INSERT CHI TIẾT ĐƠN HÀNG CŨ (ChiTietDonHang) - Tùy chọn
-- =====================================================
-- SET IDENTITY_INSERT ChiTietDonHang ON
-- GO

-- INSERT INTO ChiTietDonHang (MaCTDH, MaDH, MaSP, TongTien, Ngaygiao)
-- VALUES
--     (1, 1, 1, 29990000, 3),
--     (2, 2, 4, 27990000, 5)

-- SET IDENTITY_INSERT ChiTietDonHang OFF
-- GO

-- =====================================================
-- HOÀN TẤT
-- =====================================================
PRINT N'Đã thêm dữ liệu mẫu thành công!'
PRINT N'Tổng số danh mục: ' + CAST((SELECT COUNT(*) FROM DanhMucSP) AS NVARCHAR(10))
PRINT N'Tổng số sản phẩm: ' + CAST((SELECT COUNT(*) FROM SanPham) AS NVARCHAR(10))
PRINT N'Tổng số người dùng: ' + CAST((SELECT COUNT(*) FROM ApplicationUser) AS NVARCHAR(10))
PRINT N'Tổng số khách hàng: ' + CAST((SELECT COUNT(*) FROM KhachHang) AS NVARCHAR(10))
PRINT N'Tổng số đơn hàng: ' + CAST((SELECT COUNT(*) FROM Orders) AS NVARCHAR(10))
GO
