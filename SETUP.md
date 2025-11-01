# SimplCommerce - Hướng Dẫn Setup Local Environment

## ⚠️ BẢO MẬT QUAN TRỌNG

**KHÔNG BAO GIỜ commit các API keys, secrets, hoặc connection strings thật vào Git!**

## 📋 Yêu Cầu Hệ Thống

### Windows với SQL Server
- **SQL Server** (Express hoặc Developer Edition)
- **Visual Studio 2022** 
- **.NET 8 SDK**
- **Node.js** (cho build frontend)

### Mac/Linux với PostgreSQL
- **PostgreSQL**
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Entity Framework Core Tools**: 
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 🚀 Các Bước Setup

### Bước 1: Clone Repository

```bash
git clone https://github.com/hoang-le-edu/dotnet-ecommerce.git
cd SimplCommerce
```

### Bước 2: Tạo File Configuration Local

#### 2.1. Copy template file:
```bash
cd src/SimplCommerce.WebHost
cp appsettings.Local.json.template appsettings.Local.json
```

#### 2.2. Cập nhật `appsettings.Local.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SimplCommerce;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Authentication": {
    "Facebook": {
      "AppId": "YOUR_FACEBOOK_APP_ID",
      "AppSecret": "YOUR_FACEBOOK_APP_SECRET"
    },
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "PaymentSettings": {
    "Stripe": {
      "PublicKey": "pk_test_YOUR_KEY",
      "SecretKey": "sk_test_YOUR_KEY"
    }
  }
}
```

### Bước 3: Cấu Hình Database

#### 3.1. Cập nhật Connection String

**SQL Server (Windows):**
```json
"DefaultConnection": "Server=.;Database=SimplCommerce;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

**SQL Server với username/password:**
```json
"DefaultConnection": "Server=localhost;Database=SimplCommerce;User Id=sa;Password=YourPassword;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

**PostgreSQL (Mac/Linux):**
```json
"DefaultConnection": "User ID=postgres;Password=yourpassword;Host=localhost;Port=5432;Database=SimplCommerce;Pooling=true;"
```

#### 3.2. Tạo Database

**Visual Studio (Windows):**
1. Mở `SimplCommerce.sln` trong Visual Studio
2. Set `SimplCommerce.WebHost` làm Startup Project
3. Mở **Package Manager Console** (Tools > NuGet Package Manager > Package Manager Console)
4. Chọn `SimplCommerce.WebHost` làm Default project
5. Chạy lệnh:
   ```powershell
   Update-Database
   ```

**Command Line (Mac/Linux/Windows):**
```bash
cd src/SimplCommerce.WebHost
dotnet ef database update
```

### Bước 4: Build và Run

#### Visual Studio:
1. Build solution: `Ctrl + Shift + B`
2. Run: `Ctrl + F5` (hoặc `F5` để debug)

#### Command Line:
```bash
# Từ thư mục root
./simpl-build.sh  # Mac/Linux
# hoặc
simpl-build.bat   # Windows

# Chạy ứng dụng
cd src/SimplCommerce.WebHost
dotnet run
```

### Bước 5: Truy Cập Ứng Dụng

- **Trang chủ**: http://localhost:5000 hoặc http://localhost:49206
- **Admin Panel**: http://localhost:5000/Admin
  - Email: `admin@simplcommerce.com`
  - Password: `1qazZAQ!`

## 🔑 Lấy API Keys (Tùy Chọn)

### Stripe Payment (để test thanh toán)
1. Đăng ký tài khoản tại [Stripe Dashboard](https://dashboard.stripe.com/register)
2. Chuyển sang **Test Mode**
3. Lấy keys tại: **Developers > API keys**
4. Copy **Publishable key** và **Secret key** vào `appsettings.Local.json`

### Facebook Login (tùy chọn)
1. Tạo app tại [Facebook Developers](https://developers.facebook.com/)
2. Lấy App ID và App Secret
3. Thêm vào `appsettings.Local.json`

### Google Login (tùy chọn)
1. Tạo project tại [Google Cloud Console](https://console.cloud.google.com/)
2. Enable **Google+ API**
3. Tạo **OAuth 2.0 Client ID**
4. Thêm vào `appsettings.Local.json`

## 🛠️ Troubleshooting

### Lỗi Database Connection
- Kiểm tra SQL Server/PostgreSQL đã chạy chưa
- Verify connection string trong `appsettings.Local.json`
- Kiểm tra firewall settings

### Lỗi Build
```bash
# Clean và rebuild
dotnet clean
dotnet restore
dotnet build
```

### Lỗi Migration
```bash
# Xóa database và tạo lại
dotnet ef database drop
dotnet ef database update
```

### Port đã được sử dụng
- Đổi port trong `Properties/launchSettings.json`
- Hoặc stop các process đang dùng port 5000/49206

## 📁 Cấu Trúc File Config

```
src/SimplCommerce.WebHost/
├── appsettings.json              # Config mặc định (KHÔNG chứa secrets)
├── appsettings.Development.json  # Config cho Development environment
├── appsettings.Local.json        # Config local (IGNORED by Git) ⚠️
└── appsettings.Local.json.template  # Template để tạo Local config
```

## 🔒 Best Practices

1. ✅ **LUÔN sử dụng `appsettings.Local.json` cho local development**
2. ✅ **KHÔNG commit file `appsettings.Local.json` vào Git**
3. ✅ **Sử dụng test keys cho Stripe** (keys bắt đầu với `pk_test_` và `sk_test_`)
4. ✅ **Định kỳ rotate API keys** nếu bị lộ
5. ✅ **Sử dụng Azure Key Vault hoặc AWS Secrets Manager** cho production

## 📚 Tài Liệu Thêm

- [Official Documentation](https://docs.simplcommerce.com/)
- [GitHub Repository](https://github.com/simplcommerce/SimplCommerce)
- [Demo Site](http://demo.simplcommerce.com)

## ❓ Cần Trợ Giúp?

- Tạo issue tại [GitHub Issues](https://github.com/hoang-le-edu/dotnet-ecommerce/issues)
- Join [Gitter Chat](https://gitter.im/simplcommerce/SimplCommerce)

---

**Lưu ý**: File này được tạo để hỗ trợ setup local environment an toàn và bảo mật.

