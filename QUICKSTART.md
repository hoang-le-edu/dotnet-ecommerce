# 🚀 Quick Start - SimplCommerce

## Bắt Đầu Ngay (5 phút)

### 1. Clone & Setup Config
```bash
git clone https://github.com/hoang-le-edu/dotnet-ecommerce.git
cd SimplCommerce/src/SimplCommerce.WebHost
cp appsettings.Local.json.template appsettings.Local.json
```

### 2. Sửa Connection String trong `appsettings.Local.json`

**SQL Server:**
```json
"DefaultConnection": "Server=.;Database=SimplCommerce;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

**PostgreSQL:**
```json
"DefaultConnection": "User ID=postgres;Password=yourpassword;Host=localhost;Port=5432;Database=SimplCommerce;"
```

### 3. Tạo Database & Run

**Visual Studio:**
```powershell
# Package Manager Console
Update-Database
# Sau đó nhấn F5
```

**Command Line:**
```bash
cd src/SimplCommerce.WebHost
dotnet ef database update
dotnet run
```

### 4. Truy Cập

- 🏠 **Trang chủ**: http://localhost:5000
- 👨‍💼 **Admin**: http://localhost:5000/Admin
  - Email: `admin@simplcommerce.com`
  - Pass: `1qazZAQ!`

---

📖 **Hướng dẫn chi tiết**: Xem [SETUP.md](SETUP.md)

⚠️ **Lưu ý**: File `appsettings.Local.json` đã được git ignore để bảo vệ secrets của bạn!

