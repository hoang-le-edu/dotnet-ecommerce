# Fix Azure App Service Configuration

## 🐛 Vấn Đề Hiện Tại

Azure configuration **THIẾU** connection strings quan trọng:
- ❌ `DefaultConnection` (SQL Database)
- ❌ `RedisConnection` (Redis Cache)

Điều này khiến app crash với HTTP 500!

## ✅ Cấu Hình Đúng

### 1. Application Settings Tab

Giữ nguyên (đã đúng):

```json
{
  "name": "Azure__Blob__ContainerName",
  "value": "product-images"
},
{
  "name": "Azure__Blob__PublicEndpoint",
  "value": ""
},
{
  "name": "Azure__Blob__StorageConnectionString",
  "value": "DefaultEndpointsProtocol=https;AccountName=bakeryqueuestorage2;AccountKey=<YOUR_STORAGE_KEY>;EndpointSuffix=core.windows.net"
}
```

**Thêm Redis Enabled:**
```json
{
  "name": "Redis__Enabled",
  "value": "true"
}
```

### 2. Connection Strings Tab

**⚠️ XÓA TẤT CẢ** entries hiện tại (đang duplicate App Settings).

**THÊM MỚI** 2 connection strings này:

#### A. SQL Database Connection

```
Name: DefaultConnection
Value: Server=tcp:dotnet88.database.windows.net,1433;Initial Catalog=dbstorevipp;Persist Security Info=False;User ID=admin1204;Password=<YOUR_DB_PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
Type: SQLAzure
```

#### B. Redis Cache Connection

```
Name: RedisConnection
Value: redis-dotnet.redis.cache.windows.net:6380,password=<YOUR_REDIS_KEY>,ssl=True,abortConnect=False
Type: Custom
```

## 📝 Hướng Dẫn Chi Tiết

### Bước 1: Vào Azure Portal

```
Azure Portal 
→ App Service (storevippp-green)
→ Settings → Configuration
```

### Bước 2: Fix Connection Strings Tab

1. Click tab **"Connection strings"**
2. **XÓA** 3 entries hiện tại:
   - `Azure:Blob:ContainerName`
   - `Azure:Blob:PublicEndpoint`
   - `Azure:Blob:StorageConnectionString`

3. Click **"+ New connection string"**
4. Thêm `DefaultConnection`:
   - **Name:** `DefaultConnection`
   - **Value:** 
     ```
     Server=tcp:dotnet88.database.windows.net,1433;Initial Catalog=dbstorevipp;Persist Security Info=False;User ID=admin1204;Password=<YOUR_DB_PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
     ```
   - **Type:** `SQLAzure`
   - Click **OK**

5. Click **"+ New connection string"** again
6. Thêm `RedisConnection`:
   - **Name:** `RedisConnection`
   - **Value:**
     ```
     redis-dotnet.redis.cache.windows.net:6380,password=<YOUR_REDIS_KEY>,ssl=True,abortConnect=False
     ```
   - **Type:** `Custom`
   - Click **OK**

### Bước 3: Add Redis Enabled Setting

1. Click tab **"Application settings"**
2. Click **"+ New application setting"**
3. Add:
   - **Name:** `Redis__Enabled`
   - **Value:** `true`
   - Click **OK**

### Bước 4: Save & Restart

1. Click **"Save"** ở top
2. Confirm save
3. Click **"Restart"** để apply changes

## 🎯 Kết Quả Mong Đợi

Sau khi cấu hình xong và restart:

### Application Settings (4 items)
```
- Azure__Blob__ContainerName = "product-images"
- Azure__Blob__PublicEndpoint = ""
- Azure__Blob__StorageConnectionString = "DefaultEndpointsProtocol=..."
- Redis__Enabled = "true"
```

### Connection Strings (2 items)
```
- DefaultConnection (SQLAzure) = "Server=tcp:dotnet88..."
- RedisConnection (Custom) = "redis-dotnet.redis.cache.windows.net:6380..."
```

## ✅ Verify Configuration

Sau khi save và restart, test app:

```
https://storevippp-green.azurewebsites.net
```

**Expected:**
- ✅ HTTP 200 
- ✅ Homepage loads
- ✅ No more 500 errors

**Check logs:**
```
App Service → Monitoring → Log stream
```

Tìm dòng:
```
[Info] Attempting to connect to Redis...
[Info] Redis connected: redis-dotnet.redis.cache.windows.net:6380
Now listening on: https://[::]:8080
```

## 🔒 Security Note

Connection strings trên Azure **override** values trong `appsettings.json`.

Vì vậy:
- ✅ Giữ secrets trên Azure Config
- ✅ Để empty trong `appsettings.json` (đã gitignore)
- ✅ Local dev dùng `appsettings.Local.json`

---

**Status:** Ready to configure!

