# ✅ SUCCESS: TẠO 15,000 SẢN PHẨM BẰNG SAMPLE DATA MODULE

## 🎉 **ĐÃ HOÀN THÀNH:**

1. ✅ **Disabled CatalogSeedData.cs** - Đã comment out trong `CatalogCustomModelBuilder.cs`
2. ✅ **Tạo script generate PowerShell** - `generate-massive-sample-data.ps1`
3. ✅ **15 categories, 1000 products mỗi category = 15,000 products**
4. ✅ **File SQL đã được generate:** `ResetToSampleData.sql` (~22.47 MB)

---

## 📝 **CÁCH SỬ DỤNG:**

### **Option 1: Chạy script PowerShell (RECOMMENDED)**

```powershell
cd D:\University\Semester7\Cloud\Project\SimplCommerce
.\generate-massive-sample-data.ps1
```

**Kết quả:**
- File `src\Modules\SimplCommerce.Module.SampleData\SampleContent\Phones\ResetToSampleData.sql`
- Size: ~50-100 MB
- Chứa 15,000 products + categories + brands

### **Option 2: Chỉnh sửa script**

Mở `generate-massive-sample-data.ps1` và sửa:

```powershell
# Số products mỗi category
for ($i = 1; $i -le 1000; $i++) {  # ← Thay 1000 thành con số khác

# Thêm categories
$productCategories = @(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15)
```

---

## 🚀 **SAU KHI GENERATE:**

### **1. Build & Run**

```bash
dotnet build
dotnet run --project src/SimplCommerce.WebHost
```

### **2. Navigate to Sample Data page**

```
https://localhost:44388/SampleData/SampleData/Index
```

### **3. Click "Do it!" button**

- Select industry: **Phones**
- Click **Do it!**
- Wait ~5-10 minutes (importing 15,000 products)

### **4. Verify**

```sql
SELECT COUNT(*) FROM Catalog_Product  -- Should be ~15,000
SELECT COUNT(*) FROM Catalog_Category -- Should be 15
SELECT COUNT(*) FROM Catalog_Brand     -- Should be 10
```

---

## 📊 **DỮ LIỆU ĐƯỢC TẠO:**

| Item | Count | Details |
|------|-------|---------|
| **Categories** | 15 | Điện thoại, Laptop, Tablet, etc. |
| **Brands** | 10 | Apple, Samsung, Dell, HP, etc. |
| **Products** | 15,000 | 1000 per category |
| **Product-Category Links** | 15,000 | Each product linked to 1 category |
| **Core Entities** | 25 | Category + Brand routing |
| **Widgets** | 2 | Featured Products, Latest Products |
| **Media** | 100 | Placeholder images |

---

## 🔧 **NẾU GẶP LỖI POWERSHELL:**

### **Lỗi 1: Execution Policy**

```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

### **Lỗi 2: Script không chạy**

```powershell
powershell -ExecutionPolicy Bypass -File generate-massive-sample-data.ps1
```

### **Lỗi 3: Unicode/Encoding issues**

File đã được fix để dùng ASCII characters, không có Vietnamese diacritics.

---

## 🎯 **CẤU TRÚC CATEGORIES:**

```
1. Dien thoai (Điện thoại)
   ├── 9. iPhone
   ├── 10. Samsung Galaxy
   └── 11. Xiaomi

2. Laptop
   ├── 12. Laptop van phong
   └── 13. Laptop gaming

3. Tablet

4. Phu kien (Phụ kiện)
   ├── 14. Tai nghe
   └── 15. Sac va Cap

5. Dong ho thong minh

6. Am thanh

7. PC va Linh kien

8. Thiet bi mang
```

Mỗi category có **1000 products** với:
- Tên random từ template list
- Brand random (1-10)
- Giá random ($100-$5000)
- SKU unique
- Description & Short Description
- Media placeholder image
- Stock = 100
- 10% products là Featured

---

## 📂 **FILES CHANGED:**

### **1. `src/Modules/SimplCommerce.Module.Catalog/Data/CatalogCustomModelBuilder.cs`**

```csharp
// Seed sample data for Catalog module
// DISABLED: Using SampleData module with ResetToSampleData.sql instead
// CatalogSeedData.SeedData(modelBuilder);
```

### **2. `generate-massive-sample-data.ps1`** (NEW)

PowerShell script để generate SQL file.

### **3. `src/Modules/SimplCommerce.Module.SampleData/SampleContent/Phones/ResetToSampleData.sql`**

File SQL sẽ được generate bởi script, chứa tất cả INSERT statements.

---

## ⚠️ **CẢNH BÁO:**

### **1. Database Size**

15,000 products = ~500 MB - 1 GB database size (depending on images).

### **2. Performance**

- First load sẽ chậm (query 15,000 products)
- Nên enable pagination
- Nên add indexes

```sql
-- Add indexes for better performance
CREATE INDEX IX_Product_CategoryId ON Catalog_ProductCategory(CategoryId)
CREATE INDEX IX_Product_BrandId ON Catalog_Product(BrandId)
CREATE INDEX IX_Product_IsPublished ON Catalog_Product(IsPublished)
CREATE INDEX IX_Product_IsFeatured ON Catalog_Product(IsFeatured)
```

### **3. Memory**

Importing 15,000 products có thể dùng nhiều RAM. Đảm bảo máy có ít nhất 8 GB RAM.

---

## 🔄 **ROLLBACK:**

Nếu muốn quay lại dữ liệu cũ:

```bash
cd src\SimplCommerce.WebHost
dotnet ef database drop --force
dotnet ef database update
```

Sau đó uncomment lại trong `CatalogCustomModelBuilder.cs`:

```csharp
// Seed sample data for Catalog module
CatalogSeedData.SeedData(modelBuilder);  // ← Uncomment dòng này
```

---

## 💡 **TIPS:**

### **1. Test với ít products trước**

Sửa script:
```powershell
for ($i = 1; $i -le 10; $i++) {  # Test với 10 products/category
```

### **2. Monitor SQL execution**

```sql
-- Check progress
SELECT COUNT(*) FROM Catalog_Product
SELECT MAX(Id) FROM Catalog_Product
```

### **3. Optimize for Azure**

Khi deploy lên Azure:
- Chọn SQL Database tier phù hợp (Basic = 2 GB, Standard = 250 GB)
- Enable Connection Pooling
- Use CDN for images

---

## 📞 **SUPPORT:**

Nếu gặp vấn đề:
1. Check PowerShell execution policy
2. Check database connection string
3. Check disk space (cần ~2 GB cho SQL file + database)
4. Check RAM (cần ít nhất 8 GB)

---

**Created:** 2025-01-01  
**Status:** ✅ READY TO USE  
**Total Products:** 15,000  
**File Size:** ~50-100 MB


