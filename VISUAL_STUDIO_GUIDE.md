# 🎓 Hướng Dẫn Visual Studio 2022 - SimplCommerce

## 📋 MỤC LỤC
1. [Mở Project](#1-mở-project)
2. [Start Project](#2-start-project)
3. [Debug Project](#3-debug-project)
4. [Xem Database (GUI)](#4-xem-database-gui)
5. [Troubleshooting](#5-troubleshooting)

---

## 1️⃣ MỞ PROJECT

### Bước 1: Mở Solution File
1. **Mở Visual Studio 2022**
2. Chọn **File → Open → Project/Solution**
3. Navigate tới folder: `D:\University\Semester7\Cloud\Project\SimplCommerce`
4. Chọn file: **`SimplCommerce.sln`**
5. Click **Open**

### Bước 2: Đợi Solution Load
- Visual Studio sẽ load tất cả projects trong solution
- Bạn sẽ thấy **Solution Explorer** bên phải hiển thị cấu trúc project
- **Đợi Restore NuGet packages hoàn tất** (xem thanh status dưới cùng)

### Bước 3: Set Startup Project
1. Trong **Solution Explorer**, tìm project **`SimplCommerce.WebHost`**
2. **Right-click** vào project này
3. Chọn **Set as Startup Project**
4. Project này sẽ được **bold** (in đậm) sau khi set

> ✅ **Check:** Project `SimplCommerce.WebHost` phải được **bold** trong Solution Explorer

---

## 2️⃣ START PROJECT (KHÔNG DEBUG)

### Cách 1: Dùng Keyboard Shortcut
```
Ctrl + F5
```
**Hoặc** Menu: **Debug → Start Without Debugging**

### Cách 2: Dùng Toolbar
1. Nhìn lên **Toolbar** phía trên
2. Tìm dropdown list bên cạnh nút ▶️ (Play button)
3. Chọn: **`SimplCommerce.WebHost`** hoặc **`IIS Express`**
4. Click nút **▶️ (Start Without Debugging)**

### ⏱️ Kết Quả
- Console window sẽ mở lên
- Application build và start
- Browser tự động mở: `https://localhost:49206`
- Trang web SimplCommerce hiển thị

> 💡 **Start Without Debugging** = Chạy nhanh hơn, không bị pause tại breakpoints

---

## 3️⃣ DEBUG PROJECT

### Bước 1: Set Breakpoint
1. Mở file bất kỳ trong `SimplCommerce.WebHost` (VD: `Controllers/HomeController.cs`)
2. Click vào **left margin** (viền bên trái số dòng) để set breakpoint
3. Sẽ xuất hiện **chấm tròn màu đỏ** 🔴

**Ví dụ đặt breakpoint trong Controller:**
```
Mở: src/SimplCommerce.WebHost/Controllers/HomeController.cs
Đặt breakpoint tại dòng đầu tiên trong method Index()
```

### Bước 2: Start Debug Mode
```
F5
```
**Hoặc** Menu: **Debug → Start Debugging**

**Hoặc** Click nút: **▶️ SimplCommerce.WebHost** (nút xanh)

### Bước 3: Khi Breakpoint Hit
Application sẽ **pause** tại breakpoint. Bạn sẽ thấy:

#### 📊 **Locals Window** (Ctrl + Alt + V, L)
- Hiển thị **tất cả biến local** trong scope hiện tại
- Values của các biến

#### 📺 **Watch Window** (Debug → Windows → Watch)
- Add biến/expression để theo dõi
- VD: `user.Email`, `HttpContext.Request.Path`

#### 🔍 **Immediate Window** (Ctrl + Alt + I)
- Execute code trong khi debug
- VD: gõ `user.Email` để xem giá trị

#### 📞 **Call Stack** (Ctrl + Alt + C)
- Xem đường dẫn các method calls đến breakpoint

### Bước 4: Debug Navigation
| Phím tắt | Chức năng | Mô tả |
|----------|-----------|-------|
| **F10** | Step Over | Chạy dòng hiện tại, không vào method |
| **F11** | Step Into | Vào bên trong method call |
| **Shift + F11** | Step Out | Thoát khỏi method hiện tại |
| **F5** | Continue | Chạy tiếp đến breakpoint tiếp theo |
| **Shift + F5** | Stop Debugging | Dừng debug hoàn toàn |
| **Ctrl + Shift + F5** | Restart | Restart app trong debug mode |

### Bước 5: Inspect Variables
**Cách 1: Hover Mouse**
- Di chuột qua biến → popup hiện giá trị

**Cách 2: Locals Window**
- Xem tất cả local variables

**Cách 3: Watch Window**
- Add expression: Click "+" → nhập `user.FullName`

**Cách 4: Immediate Window**
- Gõ lệnh: `? user.Email` → Enter

---

## 4️⃣ XEM DATABASE (GUI)

### 🗄️ Method 1: SQL Server Object Explorer (RECOMMENDED)

#### Bước 1: Mở SQL Server Object Explorer
```
Menu: View → SQL Server Object Explorer
```
**Hoặc** phím tắt: **Ctrl + \, Ctrl + S**

#### Bước 2: Expand Server
1. Trong **SQL Server Object Explorer**, expand:
   ```
   📂 SQL Server
     └─ 📂 (localdb)\MSSQLLocalDB (SQL Server XX - xxxx)
   ```

2. Nếu **không thấy server**, click **Add SQL Server** (icon ➕)
   - Server name: `(localdb)\MSSQLLocalDB`
   - Authentication: **Windows Authentication**
   - Click **Connect**

#### Bước 3: Navigate to Database
```
📂 (localdb)\MSSQLLocalDB
  └─ 📂 Databases
      └─ 📂 SimplCommerce
          └─ 📂 Tables
```

#### Bước 4: View Table Data
1. Expand **Tables** → tìm table (VD: `dbo.Core_User`)
2. **Right-click** table → **View Data**
3. Data grid sẽ hiển thị trong tab mới

#### Bước 5: Query Database
1. **Right-click** database `SimplCommerce`
2. Chọn **New Query...**
3. Gõ SQL query:
   ```sql
   SELECT * FROM Core_User
   ```
4. Click **Execute** (hoặc Ctrl + Shift + E)

---

### 🗄️ Method 2: Server Explorer

#### Bước 1: Mở Server Explorer
```
Menu: View → Server Explorer
```

#### Bước 2: Add Connection
1. **Right-click** "Data Connections" → **Add Connection...**
2. **Data Source:** Microsoft SQL Server (SqlClient)
3. **Server name:** `(localdb)\MSSQLLocalDB`
4. **Database name:** `SimplCommerce`
5. Click **Test Connection** → Should show "Test connection succeeded"
6. Click **OK**

#### Bước 3: Explore Tables
```
📂 Data Connections
  └─ 📂 SimplCommerce.dbo
      └─ 📂 Tables
          └─ 📊 Core_User
```

**Right-click table → Show Table Data**

---

### 🗄️ Method 3: Cloud Explorer (Azure)
*(Only if using Azure SQL Database)*

---

## 5️⃣ TROUBLESHOOTING

### ❌ "Unable to start program ... Access is denied"
**Fix:**
1. **Run Visual Studio as Administrator**
2. Right-click Visual Studio icon → **Run as administrator**

### ❌ "Could not find a part of the path ... bin\Debug\net8.0"
**Fix:**
1. Clean solution: **Build → Clean Solution**
2. Rebuild: **Build → Rebuild Solution**
3. Try run again

### ❌ Browser không mở tự động
**Fix:**
1. Right-click project `SimplCommerce.WebHost`
2. Properties → Debug → General
3. Check **Launch browser**
4. Enter URL: `https://localhost:49206`

### ❌ "The target process exited without raising a CoreCLR started event"
**Fix:**
1. Close Visual Studio
2. Delete `bin` và `obj` folders:
   ```powershell
   cd D:\University\Semester7\Cloud\Project\SimplCommerce
   Get-ChildItem -Path . -Recurse -Include bin,obj | Remove-Item -Recurse -Force
   ```
3. Reopen Visual Studio → Rebuild

### ❌ Cannot connect to LocalDB
**Fix:**
1. Open PowerShell as Admin:
   ```powershell
   sqllocaldb start MSSQLLocalDB
   sqllocaldb info MSSQLLocalDB
   ```
2. Restart Visual Studio

---

## 📝 QUICK REFERENCE

### ⌨️ Keyboard Shortcuts
| Action | Shortcut |
|--------|----------|
| Start Without Debug | **Ctrl + F5** |
| Start Debugging | **F5** |
| Stop Debugging | **Shift + F5** |
| Restart | **Ctrl + Shift + F5** |
| Toggle Breakpoint | **F9** |
| Step Over | **F10** |
| Step Into | **F11** |
| Step Out | **Shift + F11** |
| SQL Server Object Explorer | **Ctrl + \, Ctrl + S** |
| Solution Explorer | **Ctrl + Alt + L** |
| Build Solution | **Ctrl + Shift + B** |

### 🎯 Common Debug Windows
- **Locals:** Debug → Windows → Locals
- **Watch:** Debug → Windows → Watch → Watch 1
- **Immediate:** Debug → Windows → Immediate
- **Call Stack:** Debug → Windows → Call Stack
- **Output:** View → Output
- **Error List:** View → Error List

---

## 🎓 VIDEO TUTORIALS (Recommended)
1. **Visual Studio 2022 Debugging Basics:**
   https://www.youtube.com/watch?v=2KVJAw8zTio

2. **SQL Server Object Explorer:**
   https://learn.microsoft.com/en-us/sql/ssdt/how-to-browse-objects-in-a-sql-server-database-project

---

## 💡 PRO TIPS

### Tip 1: Hot Reload
- Khi chạy debug (F5), sửa code → **Ctrl + Alt + F5** để apply changes mà không restart

### Tip 2: Multiple Startup Projects
- Nếu có nhiều services, right-click Solution → Properties → Startup Project → Multiple

### Tip 3: Debug Console Output
- View → Output → Show output from: Debug

### Tip 4: Quick Database Query
- **Ctrl + \, Ctrl + S** → Right-click DB → New Query

### Tip 5: DataTips
- Khi debug, hover qua object → click 📌 để pin variable display

---

**Happy Coding! 🚀**

