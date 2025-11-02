-- ============================================================================
-- SIMPLCOMMERCE - SEED DATA SCRIPT
-- Thêm dữ liệu mẫu để test và demo
-- ============================================================================

USE SimplCommerce;
GO

-- ============================================================================
-- 1. THÊM CATEGORIES (Danh mục sản phẩm)
-- ============================================================================

SET IDENTITY_INSERT Catalog_Category ON;

INSERT INTO Catalog_Category (Id, Name, Slug, Description, DisplayOrder, IsPublished, IncludeInMenu, IsDeleted, ParentId, ThumbnailImageId, CreatedOn, LatestUpdatedOn)
VALUES 
    (100, N'Điện thoại', 'dien-thoai', N'Điện thoại thông minh các loại', 1, 1, 1, 0, NULL, NULL, GETDATE(), GETDATE()),
    (101, N'Laptop', 'laptop', N'Máy tính xách tay', 2, 1, 1, 0, NULL, NULL, GETDATE(), GETDATE()),
    (102, N'Tablet', 'tablet', N'Máy tính bảng', 3, 1, 1, 0, NULL, NULL, GETDATE(), GETDATE()),
    (103, N'Phụ kiện', 'phu-kien', N'Phụ kiện điện tử', 4, 1, 1, 0, NULL, NULL, GETDATE(), GETDATE()),
    (104, N'Đồng hồ thông minh', 'dong-ho-thong-minh', N'Smart watches', 5, 1, 1, 0, NULL, NULL, GETDATE(), GETDATE()),
    (105, N'Tai nghe', 'tai-nghe', N'Tai nghe các loại', 6, 1, 1, 0, 103, NULL, GETDATE(), GETDATE()),
    (106, N'Sạc dự phòng', 'sac-du-phong', N'Pin sạc dự phòng', 7, 1, 1, 0, 103, NULL, GETDATE(), GETDATE());

SET IDENTITY_INSERT Catalog_Category OFF;
GO

-- ============================================================================
-- 2. THÊM BRANDS (Thương hiệu)
-- ============================================================================

SET IDENTITY_INSERT Catalog_Brand ON;

INSERT INTO Catalog_Brand (Id, Name, Slug, IsPublished, IsDeleted, CreatedOn, LatestUpdatedOn)
VALUES 
    (100, N'Apple', 'apple', 1, 0, GETDATE(), GETDATE()),
    (101, N'Samsung', 'samsung', 1, 0, GETDATE(), GETDATE()),
    (102, N'Xiaomi', 'xiaomi', 1, 0, GETDATE(), GETDATE()),
    (103, N'OPPO', 'oppo', 1, 0, GETDATE(), GETDATE()),
    (104, N'Dell', 'dell', 1, 0, GETDATE(), GETDATE()),
    (105, N'HP', 'hp', 1, 0, GETDATE(), GETDATE()),
    (106, N'Asus', 'asus', 1, 0, GETDATE(), GETDATE()),
    (107, N'Sony', 'sony', 1, 0, GETDATE(), GETDATE()),
    (108, N'Huawei', 'huawei', 1, 0, GETDATE(), GETDATE());

SET IDENTITY_INSERT Catalog_Brand OFF;
GO

-- ============================================================================
-- 3. THÊM PRODUCTS (Sản phẩm)
-- ============================================================================

SET IDENTITY_INSERT Catalog_Product ON;

INSERT INTO Catalog_Product (
    Id, Name, Slug, ShortDescription, Description, Specification,
    Price, OldPrice, SpecialPrice, SpecialPriceStart, SpecialPriceEnd,
    HasOptions, IsVisibleIndividually, IsFeatured, IsCallForPricing, IsAllowToOrder,
    StockQuantity, Sku, Gtin, NormalizedName, DisplayOrder,
    VendorId, TaxClassId, StockTrackingIsEnabled, IsPublished, PublishedOn,
    IsDeleted, CreatedOn, LatestUpdatedOn, CreatedById, LatestUpdatedById,
    BrandId, ReviewsCount, RatingAverage
)
VALUES 
    -- iPhone
    (1000, N'iPhone 15 Pro Max 256GB', 'iphone-15-pro-max-256gb', 
     N'iPhone 15 Pro Max - Titanium. Camera 48MP. Chip A17 Pro', 
     N'<h2>iPhone 15 Pro Max</h2><p>iPhone mới nhất với chip A17 Pro mạnh mẽ</p>',
     N'Screen: 6.7" - CPU: A17 Pro - RAM: 8GB - Camera: 48MP - Pin: 4422 mAh',
     34990000, 36990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 50, 'IP15PM256', NULL, 'IPHONE 15 PRO MAX 256GB', 1,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 100, 0, 0),

    (1001, N'iPhone 14 Pro 128GB', 'iphone-14-pro-128gb',
     N'iPhone 14 Pro - Dynamic Island. Camera 48MP',
     N'<h2>iPhone 14 Pro</h2><p>Thiết kế đẹp, hiệu năng mạnh mẽ</p>',
     N'Screen: 6.1" - CPU: A16 Bionic - RAM: 6GB - Camera: 48MP',
     27990000, 29990000, 26990000, GETDATE(), DATEADD(day, 30, GETDATE()),
     0, 1, 1, 0, 1, 30, 'IP14P128', NULL, 'IPHONE 14 PRO 128GB', 2,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 100, 2, 4.5),

    -- Samsung
    (1002, N'Samsung Galaxy S24 Ultra', 'samsung-galaxy-s24-ultra',
     N'Galaxy S24 Ultra - S Pen. Camera 200MP. Snapdragon 8 Gen 3',
     N'<h2>Samsung Galaxy S24 Ultra</h2><p>Flagship Android đỉnh cao</p>',
     N'Screen: 6.8" - CPU: Snapdragon 8 Gen 3 - RAM: 12GB - Camera: 200MP',
     33990000, 35990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 45, 'S24U512', NULL, 'SAMSUNG GALAXY S24 ULTRA', 3,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 101, 5, 4.8),

    (1003, N'Samsung Galaxy Z Fold5', 'samsung-galaxy-z-fold5',
     N'Galaxy Z Fold5 - Màn hình gập. Chip Snapdragon 8 Gen 2',
     N'<h2>Samsung Galaxy Z Fold5</h2><p>Điện thoại gập tiên tiến</p>',
     N'Screen: 7.6" (main) - CPU: Snapdragon 8 Gen 2 - RAM: 12GB',
     43990000, 45990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 20, 'ZF5-512', NULL, 'SAMSUNG GALAXY Z FOLD5', 4,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 101, 1, 4.3),

    -- Xiaomi
    (1004, N'Xiaomi 14 Ultra', 'xiaomi-14-ultra',
     N'Xiaomi 14 Ultra - Camera Leica. Chip Snapdragon 8 Gen 3',
     N'<h2>Xiaomi 14 Ultra</h2><p>Camera flagship với Leica</p>',
     N'Screen: 6.73" - CPU: Snapdragon 8 Gen 3 - RAM: 16GB - Camera: 50MP',
     29990000, 31990000, 28990000, GETDATE(), DATEADD(day, 15, GETDATE()),
     0, 1, 1, 0, 1, 35, 'MI14U16', NULL, 'XIAOMI 14 ULTRA', 5,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 102, 3, 4.6),

    -- Laptop Dell
    (1005, N'Dell XPS 15 9530 i7', 'dell-xps-15-9530-i7',
     N'Dell XPS 15 - Intel Core i7-13700H. RTX 4050. 16GB RAM',
     N'<h2>Dell XPS 15</h2><p>Laptop cao cấp cho dân sáng tạo</p>',
     N'CPU: Intel Core i7-13700H - RAM: 16GB - SSD: 512GB - VGA: RTX 4050 - Screen: 15.6" FHD+',
     52990000, 54990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 15, 'XPS15-I7', NULL, 'DELL XPS 15 9530 I7', 6,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 104, 2, 4.7),

    (1006, N'Dell Inspiron 15 3520', 'dell-inspiron-15-3520',
     N'Dell Inspiron 15 - Intel Core i5. 8GB RAM. Giá tốt',
     N'<h2>Dell Inspiron 15</h2><p>Laptop văn phòng, học tập</p>',
     N'CPU: Intel Core i5-1235U - RAM: 8GB - SSD: 256GB - Screen: 15.6" FHD',
     14990000, 15990000, 13990000, GETDATE(), DATEADD(day, 20, GETDATE()),
     0, 1, 0, 0, 1, 25, 'INS15-I5', NULL, 'DELL INSPIRON 15 3520', 7,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 104, 0, 0),

    -- Asus Laptop
    (1007, N'Asus ROG Strix G16 RTX 4070', 'asus-rog-strix-g16-rtx4070',
     N'Asus ROG Strix G16 - Gaming. Intel i9. RTX 4070',
     N'<h2>Asus ROG Strix G16</h2><p>Laptop gaming mạnh mẽ</p>',
     N'CPU: Intel Core i9-13980HX - RAM: 32GB - SSD: 1TB - VGA: RTX 4070 - Screen: 16" QHD 240Hz',
     65990000, 68990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 10, 'ROG-G16', NULL, 'ASUS ROG STRIX G16 RTX 4070', 8,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 106, 1, 5.0),

    -- iPad
    (1008, N'iPad Pro M2 12.9 inch 256GB', 'ipad-pro-m2-129-256gb',
     N'iPad Pro M2 - Chip M2. Màn hình Liquid Retina XDR',
     N'<h2>iPad Pro M2</h2><p>Tablet cao cấp nhất của Apple</p>',
     N'Screen: 12.9" Liquid Retina XDR - CPU: Apple M2 - RAM: 8GB - Storage: 256GB',
     32990000, 34990000, NULL, NULL, NULL,
     0, 1, 1, 0, 1, 20, 'IPADPRO-M2', NULL, 'IPAD PRO M2 12.9 INCH 256GB', 9,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 100, 4, 4.9),

    -- Phụ kiện
    (1009, N'AirPods Pro 2 (USB-C)', 'airpods-pro-2-usbc',
     N'AirPods Pro 2 - Chống ồn chủ động. Cổng USB-C',
     N'<h2>AirPods Pro 2</h2><p>Tai nghe True Wireless cao cấp</p>',
     N'Bluetooth 5.3 - Active Noise Cancellation - Spatial Audio - MagSafe Charging',
     6990000, 7490000, 6490000, GETDATE(), DATEADD(day, 10, GETDATE()),
     0, 1, 1, 0, 1, 100, 'APP2-USBC', NULL, 'AIRPODS PRO 2 (USB-C)', 10,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 100, 10, 4.8),

    (1010, N'Samsung Galaxy Buds2 Pro', 'samsung-galaxy-buds2-pro',
     N'Galaxy Buds2 Pro - Chống ồn. Hi-Fi 24bit',
     N'<h2>Galaxy Buds2 Pro</h2><p>Tai nghe True Wireless Samsung</p>',
     N'Bluetooth 5.3 - Active Noise Cancellation - IPX7 - Wireless Charging',
     4490000, 4990000, NULL, NULL, NULL,
     0, 1, 0, 0, 1, 80, 'BUDS2PRO', NULL, 'SAMSUNG GALAXY BUDS2 PRO', 11,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, 101, 7, 4.5),

    -- Sạc dự phòng
    (1011, N'Anker PowerCore 20000mAh PD', 'anker-powercore-20000-pd',
     N'Sạc dự phòng Anker 20000mAh - Sạc nhanh PD 18W',
     N'<h2>Anker PowerCore</h2><p>Pin dự phòng dung lượng cao</p>',
     N'Capacity: 20000mAh - Output: USB-C PD 18W, USB-A QC 18W - Input: USB-C PD',
     1290000, 1490000, 1190000, GETDATE(), DATEADD(day, 30, GETDATE()),
     0, 1, 0, 0, 1, 150, 'ANK-PC20K', NULL, 'ANKER POWERCORE 20000MAH PD', 12,
     NULL, NULL, 1, 1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, NULL, 15, 4.7);

SET IDENTITY_INSERT Catalog_Product OFF;
GO

-- ============================================================================
-- 4. LIÊN KẾT PRODUCTS VỚI CATEGORIES
-- ============================================================================

INSERT INTO Catalog_ProductCategory (ProductId, CategoryId, IsFeaturedProduct, DisplayOrder)
VALUES 
    -- iPhones trong category Điện thoại
    (1000, 100, 1, 1),
    (1001, 100, 1, 2),
    
    -- Samsung phones
    (1002, 100, 1, 3),
    (1003, 100, 1, 4),
    
    -- Xiaomi
    (1004, 100, 1, 5),
    
    -- Laptops Dell
    (1005, 101, 1, 1),
    (1006, 101, 0, 2),
    
    -- Asus laptop
    (1007, 101, 1, 3),
    
    -- iPad
    (1008, 102, 1, 1),
    
    -- Phụ kiện tai nghe
    (1009, 105, 1, 1),
    (1009, 103, 0, 1), -- Cũng trong Phụ kiện chính
    (1010, 105, 0, 2),
    (1010, 103, 0, 2),
    
    -- Sạc dự phòng
    (1011, 106, 0, 1),
    (1011, 103, 0, 3);
GO

-- ============================================================================
-- 5. THÊM COUNTRIES & STATES (Việt Nam)
-- ============================================================================

SET IDENTITY_INSERT Core_Country ON;

INSERT INTO Core_Country (Id, Name, Code3, IsBillingEnabled, IsShippingEnabled, IsCityEnabled, IsZipCodeEnabled, IsDistrictEnabled)
VALUES 
    (100, N'Việt Nam', 'VNM', 1, 1, 1, 0, 1);

SET IDENTITY_INSERT Core_Country OFF;
GO

SET IDENTITY_INSERT Core_StateOrProvince ON;

INSERT INTO Core_StateOrProvince (Id, CountryId, Code, Name, Type)
VALUES 
    (100, 100, 'SG', N'Hồ Chí Minh', 'City'),
    (101, 100, 'HN', N'Hà Nội', 'City'),
    (102, 100, 'DN', N'Đà Nẵng', 'City'),
    (103, 100, 'BD', N'Bình Dương', 'Province'),
    (104, 100, 'DNA', N'Đồng Nai', 'Province');

SET IDENTITY_INSERT Core_StateOrProvince OFF;
GO

-- ============================================================================
-- 6. THÊM DISTRICTS (Quận/Huyện HCM)
-- ============================================================================

SET IDENTITY_INSERT Core_District ON;

INSERT INTO Core_District (Id, StateOrProvinceId, Name, Type, Location)
VALUES 
    (100, 100, N'Quận 1', 'District', NULL),
    (101, 100, N'Quận 2', 'District', NULL),
    (102, 100, N'Quận 3', 'District', NULL),
    (103, 100, N'Quận 4', 'District', NULL),
    (104, 100, N'Quận 5', 'District', NULL),
    (105, 100, N'Quận 6', 'District', NULL),
    (106, 100, N'Quận 7', 'District', NULL),
    (107, 100, N'Quận 8', 'District', NULL),
    (108, 100, N'Quận 10', 'District', NULL),
    (109, 100, N'Quận 11', 'District', NULL),
    (110, 100, N'Thủ Đức', 'City', NULL),
    (111, 100, N'Bình Thạnh', 'District', NULL),
    (112, 100, N'Tân Bình', 'District', NULL);

SET IDENTITY_INSERT Core_District OFF;
GO

-- ============================================================================
-- 7. THÊM SAMPLE REVIEWS (Đánh giá sản phẩm)
-- ============================================================================

SET IDENTITY_INSERT Reviews_Review ON;

INSERT INTO Reviews_Review (Id, UserId, EntityId, EntityTypeId, Rating, Title, Comment, Status, CreatedOn)
VALUES 
    (100, 10, 1001, 'Product', 5, N'Sản phẩm tuyệt vời!', N'iPhone 14 Pro đẹp, mượt, camera đỉnh. Rất hài lòng!', 1, GETDATE()),
    (101, 10, 1001, 'Product', 4, N'Tốt nhưng hơi đắt', N'Chất lượng tốt, tuy nhiên giá hơi cao so với Android', 1, DATEADD(day, -2, GETDATE())),
    
    (102, 10, 1002, 'Product', 5, N'Flagship Android đỉnh', N'S24 Ultra quá đỉnh, S Pen rất hữu ích', 1, DATEADD(day, -5, GETDATE())),
    (103, 10, 1002, 'Product', 5, N'Camera 200MP siêu nét', N'Chụp ảnh đẹp xuất sắc, zoom xa vẫn rõ', 1, DATEADD(day, -3, GETDATE())),
    (104, 10, 1002, 'Product', 5, N'Xứng đáng số 1', N'Đắt nhưng xứng đáng, mượt mà bền bỉ', 1, DATEADD(day, -1, GETDATE())),
    (105, 10, 1002, 'Product', 4, N'Tốt nhưng pin hơi yếu', N'Mọi thứ đều tốt, nhưng pin có thể tốt hơn', 1, DATEADD(day, -7, GETDATE())),
    (106, 10, 1002, 'Product', 5, N'Recommend!', N'Ai có tiền nên mua, không hối hận', 1, DATEADD(day, -10, GETDATE())),
    
    (107, 10, 1004, 'Product', 5, N'Camera Leica tuyệt', N'Xiaomi 14 Ultra chụp ảnh đẹp như máy ảnh chuyên nghiệp', 1, DATEADD(day, -4, GETDATE())),
    (108, 10, 1004, 'Product', 4, N'Giá tốt, hiệu năng cao', N'Rẻ hơn iPhone nhưng vẫn rất mạnh', 1, DATEADD(day, -6, GETDATE())),
    (109, 10, 1004, 'Product', 5, N'Đáng tiền', N'Mua không phải suy nghĩ', 1, DATEADD(day, -8, GETDATE())),
    
    (110, 10, 1005, 'Product', 5, N'Laptop làm việc tốt', N'Dell XPS 15 màn hình đẹp, làm đồ họa mượt', 1, DATEADD(day, -12, GETDATE())),
    (111, 10, 1005, 'Product', 4, N'Thiết kế đẹp', N'Nhẹ, mỏng, thiết kế cao cấp', 1, DATEADD(day, -9, GETDATE())),
    
    (112, 10, 1007, 'Product', 5, N'Laptop gaming đỉnh', N'Chơi game mượt, màn hình 240Hz cực đã', 1, DATEADD(day, -11, GETDATE())),
    
    (113, 10, 1008, 'Product', 5, N'iPad tốt nhất', N'Dùng cho công việc rất tốt, chip M2 cực mạnh', 1, DATEADD(day, -13, GETDATE())),
    (114, 10, 1008, 'Product', 5, N'Màn hình đẹp', N'Màn hình XDR đỉnh, xem phim cực sướng', 1, DATEADD(day, -14, GETDATE())),
    (115, 10, 1008, 'Product', 4, N'Giá hơi cao', N'Tốt nhưng đắt, cân nhắc kỹ trước khi mua', 1, DATEADD(day, -15, GETDATE())),
    (116, 10, 1008, 'Product', 5, N'Recommend', N'Ai làm đồ họa nên mua', 1, DATEADD(day, -16, GETDATE())),
    
    (117, 10, 1009, 'Product', 5, N'Tai nghe ngon', N'AirPods Pro 2 chống ồn tốt, âm thanh đỉnh', 1, DATEADD(day, -17, GETDATE())),
    (118, 10, 1009, 'Product', 5, N'Spatial Audio hay', N'Nghe nhạc sống động, tính năng Spatial Audio rất hay', 1, DATEADD(day, -18, GETDATE())),
    (119, 10, 1009, 'Product', 4, N'Hơi đắt', N'Chất lượng tốt nhưng giá cao', 1, DATEADD(day, -19, GETDATE())),
    (120, 10, 1009, 'Product', 5, N'Xứng đáng', N'Dùng quen rồi không muốn đổi', 1, DATEADD(day, -20, GETDATE()));

SET IDENTITY_INSERT Reviews_Review OFF;
GO

-- ============================================================================
-- 8. CẬP NHẬT REVIEW COUNT & RATING AVERAGE
-- ============================================================================

-- iPhone 14 Pro: 2 reviews, avg 4.5
UPDATE Catalog_Product SET ReviewsCount = 2, RatingAverage = 4.5 WHERE Id = 1001;

-- Samsung S24 Ultra: 5 reviews, avg 4.8
UPDATE Catalog_Product SET ReviewsCount = 5, RatingAverage = 4.8 WHERE Id = 1002;

-- Xiaomi 14 Ultra: 3 reviews, avg 4.6
UPDATE Catalog_Product SET ReviewsCount = 3, RatingAverage = 4.67 WHERE Id = 1004;

-- Dell XPS 15: 2 reviews, avg 4.5
UPDATE Catalog_Product SET ReviewsCount = 2, RatingAverage = 4.5 WHERE Id = 1005;

-- Asus ROG: 1 review, avg 5.0
UPDATE Catalog_Product SET ReviewsCount = 1, RatingAverage = 5.0 WHERE Id = 1007;

-- iPad Pro: 4 reviews, avg 4.75
UPDATE Catalog_Product SET ReviewsCount = 4, RatingAverage = 4.75 WHERE Id = 1008;

-- AirPods Pro 2: 4 reviews, avg 4.75
UPDATE Catalog_Product SET ReviewsCount = 4, RatingAverage = 4.75 WHERE Id = 1009;
GO

-- ============================================================================
-- 9. THÊM SAMPLE NEWS
-- ============================================================================

SET IDENTITY_INSERT News_NewsCategory ON;

INSERT INTO News_NewsCategory (Id, Name, Slug, IsPublished, IsDeleted, CreatedOn, LatestUpdatedOn)
VALUES 
    (100, N'Tin công nghệ', 'tin-cong-nghe', 1, 0, GETDATE(), GETDATE()),
    (101, N'Đánh giá sản phẩm', 'danh-gia-san-pham', 1, 0, GETDATE(), GETDATE()),
    (102, N'Khuyến mãi', 'khuyen-mai', 1, 0, GETDATE(), GETDATE());

SET IDENTITY_INSERT News_NewsCategory OFF;
GO

SET IDENTITY_INSERT News_NewsItem ON;

INSERT INTO News_NewsItem (Id, Name, Slug, ShortContent, FullContent, IsPublished, PublishedOn, IsDeleted, CreatedOn, LatestUpdatedOn, CreatedById, LatestUpdatedById, ThumbnailImageId)
VALUES 
    (100, N'Ra mắt iPhone 15 Series', 'ra-mat-iphone-15-series',
     N'Apple vừa chính thức ra mắt dòng iPhone 15 với nhiều cải tiến',
     N'<h2>iPhone 15 Series</h2><p>Apple đã ra mắt iPhone 15 với chip A17 Pro...</p>',
     1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10, NULL),
     
    (101, N'Samsung Galaxy S24 Ultra - Flagship Android 2024', 'samsung-s24-ultra-flagship-2024',
     N'Samsung Galaxy S24 Ultra với camera 200MP và Snapdragon 8 Gen 3',
     N'<h2>S24 Ultra Review</h2><p>Smartphone Android mạnh nhất hiện nay...</p>',
     1, DATEADD(day, -3, GETDATE()), 0, DATEADD(day, -3, GETDATE()), DATEADD(day, -3, GETDATE()), 10, 10, NULL);

SET IDENTITY_INSERT News_NewsItem OFF;
GO

INSERT INTO News_NewsItemCategory (NewsItemId, CategoryId)
VALUES 
    (100, 100), -- Tin công nghệ
    (100, 101), -- Đánh giá
    (101, 100),
    (101, 101);
GO

-- ============================================================================
-- 10. THÊM SAMPLE CMS PAGES
-- ============================================================================

SET IDENTITY_INSERT Cms_Page ON;

INSERT INTO Cms_Page (Id, Name, Slug, MetaTitle, MetaKeywords, MetaDescription, IsPublished, PublishedOn, IsDeleted, CreatedOn, LatestUpdatedOn, CreatedById, LatestUpdatedById, Body)
VALUES 
    (100, N'Giới thiệu', 'gioi-thieu',
     N'Giới thiệu về SimplCommerce', N'giới thiệu, về chúng tôi', N'Trang giới thiệu về SimplCommerce',
     1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10,
     N'<h1>Về SimplCommerce</h1><p>SimplCommerce là nền tảng thương mại điện tử hiện đại...</p>'),
     
    (101, N'Chính sách bảo mật', 'chinh-sach-bao-mat',
     N'Chính sách bảo mật', N'bảo mật, privacy', N'Chính sách bảo mật thông tin khách hàng',
     1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10,
     N'<h1>Chính sách bảo mật</h1><p>Chúng tôi cam kết bảo vệ thông tin khách hàng...</p>'),
     
    (102, N'Điều khoản sử dụng', 'dieu-khoan-su-dung',
     N'Điều khoản sử dụng', N'điều khoản, terms', N'Điều khoản sử dụng dịch vụ',
     1, GETDATE(), 0, GETDATE(), GETDATE(), 10, 10,
     N'<h1>Điều khoản sử dụng</h1><p>Khi sử dụng dịch vụ, bạn đồng ý với các điều khoản...</p>');

SET IDENTITY_INSERT Cms_Page OFF;
GO

-- ============================================================================
-- HOÀN TẤT!
-- ============================================================================

PRINT '✅ Seed data completed successfully!';
PRINT '';
PRINT 'Summary:';
PRINT '- Categories: 7';
PRINT '- Brands: 9';
PRINT '- Products: 12';
PRINT '- Reviews: 21';
PRINT '- Country: Vietnam with provinces';
PRINT '- News: 2 items';
PRINT '- CMS Pages: 3';
PRINT '';
PRINT '🚀 Your database is ready for testing!';
GO

